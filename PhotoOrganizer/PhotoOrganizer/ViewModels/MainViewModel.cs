using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizer.Services;

namespace PhotoOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject {
    private const string NoFolderMessage = "Click a folder in the sources tree to see its contents";
    private const string EmptyFolderMessage = "No subfolders or photos in this folder";

    private readonly IFileSystemService _fileSystem;

    private readonly IShellService _shell;

    private readonly ILogger<MainViewModel> _logger;

    private int _folderToken;

    public MainViewModel(IFileSystemService fileSystem, IShellService shell, ILogger<MainViewModel> logger) {
        _fileSystem = fileSystem;
        _shell = shell;
        _logger = logger;
        Title = "Photo Organizer";
        ShowNoFolder = true;
        ItemsSummary = string.Empty;
        EmptyMessage = NoFolderMessage;
        ShowEmpty = true;

        foreach (var drive in _fileSystem.GetDrives()) {
            DestinationTree.Add(new FolderNode(drive, false));
            SourcesTree.Add(CreateSourceNode(drive));
        }
    }

    [ObservableProperty]
    public partial string Title { get; set; }

    public ObservableCollection<FolderNode> DestinationTree { get; } = new();

    public ObservableCollection<FolderNode> SourcesTree { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DestinationHint))]
    public partial FolderNode? SelectedDestination { get; set; }

    public string DestinationHint => SelectedDestination?.Path ?? "pick one folder";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SourcesHint))]
    public partial int CheckedSourceCount { get; set; }

    public string SourcesHint => CheckedSourceCount > 0
        ? $"{CheckedSourceCount} selected"
        : "check folders to scan";

    [ObservableProperty]
    public partial object? SelectedSourceItem { get; set; }

    public ObservableCollection<ContentTile> Items { get; } = new();

    public ObservableCollection<PathSegment> Breadcrumbs { get; } = new();

    [ObservableProperty]
    public partial bool ShowNoFolder { get; set; }

    [ObservableProperty]
    public partial string ItemsSummary { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingItems { get; set; }

    [ObservableProperty]
    public partial string EmptyMessage { get; set; }

    [ObservableProperty]
    public partial bool ShowEmpty { get; set; }

    public Task ShowFolderAsync(FolderNode node) {
        return ShowFolderAsync(node.Name, node.Path);
    }

    public async Task OpenAsync(ContentTile tile) {
        switch (tile) {
            case FolderTile folder:
                await OpenFolderAsync(folder);
                break;
            case PhotoTile photo:
                _shell.OpenFile(photo.Path);
                break;
        }
    }

    public Task NavigateAsync(PathSegment segment) {
        return SelectAndShowAsync(segment.Name, segment.Path);
    }

    private Task OpenFolderAsync(FolderTile folder) {
        return SelectAndShowAsync(folder.Name, folder.Path);
    }

    private async Task SelectAndShowAsync(string name, string path) {
        var node = await FindNodeAsync(path);

        if (node is not null) {
            SelectedSourceItem = node;
            await ShowFolderAsync(node);
            return;
        }

        await ShowFolderAsync(name, path);
    }

    private async Task<FolderNode?> FindNodeAsync(string path) {
        var node = SourcesTree.FirstOrDefault(root => Contains(root.Path, path));

        while (node is not null && !string.Equals(node.Path, path, StringComparison.OrdinalIgnoreCase)) {
            await LoadChildrenAsync(node);

            var child = node.Children.FirstOrDefault(candidate => Contains(candidate.Path, path));

            if (child is null) {
                return null;
            }

            node.IsExpanded = true;
            node = child;
        }

        return node;
    }

    private static bool Contains(string folder, string path) {
        if (string.Equals(folder, path, StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        var prefix = folder.EndsWith(Path.DirectorySeparatorChar)
            ? folder
            : folder + Path.DirectorySeparatorChar;

        return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    private async Task ShowFolderAsync(string name, string path) {
        var token = ++_folderToken;

        Breadcrumbs.Clear();

        foreach (var segment in BuildBreadcrumbs(name, path)) {
            Breadcrumbs.Add(segment);
        }

        ShowNoFolder = false;
        ItemsSummary = string.Empty;
        ShowEmpty = false;
        IsLoadingItems = true;
        Items.Clear();

        var content = await Task.Run(() => (
            Folders: _fileSystem.GetDirectories(path),
            Photos: _fileSystem.GetPhotos(path)));

        if (token != _folderToken) {
            return;
        }

        foreach (var folder in content.Folders) {
            Items.Add(new FolderTile(folder));
        }

        foreach (var photo in content.Photos) {
            Items.Add(new PhotoTile(photo, _logger));
        }

        IsLoadingItems = false;
        ItemsSummary = BuildSummary(content.Folders.Count, content.Photos.Count);
        EmptyMessage = EmptyFolderMessage;
        ShowEmpty = Items.Count == 0;

        _logger.LogDebug("Opened {Path}: {Folders} folders, {Photos} photos",
            path, content.Folders.Count, content.Photos.Count);
    }

    public async Task LoadChildrenAsync(FolderNode node) {
        if (node.IsLoaded) {
            return;
        }

        node.IsLoaded = true;

        var items = await Task.Run(() => _fileSystem.GetDirectories(node.Path));

        foreach (var item in items) {
            var child = node.IsSource ? CreateSourceNode(item) : new FolderNode(item, false);
            node.Children.Add(child);
        }

        node.HasUnrealizedChildren = false;
    }

    private FolderNode CreateSourceNode(Models.FolderItem item) {
        var node = new FolderNode(item, true);
        node.CheckedChanged += UpdateSourcesSummary;
        return node;
    }

    private void UpdateSourcesSummary() {
        CheckedSourceCount = CountChecked(SourcesTree);
    }

    private static string BuildSummary(int folders, int photos) {
        var parts = new List<string>();

        if (folders > 0) {
            parts.Add(folders == 1 ? "1 folder" : $"{folders} folders");
        }

        if (photos > 0) {
            parts.Add(photos == 1 ? "1 photo" : $"{photos} photos");
        }

        return string.Join(" · ", parts);
    }

    private static IReadOnlyList<PathSegment> BuildBreadcrumbs(string name, string path) {
        var parts = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) {
            return new[] { new PathSegment { Name = name, Path = path } };
        }

        var segments = new List<PathSegment>(parts.Length);
        var current = string.Empty;

        foreach (var part in parts) {
            current = current.Length == 0
                ? part + Path.DirectorySeparatorChar
                : Path.Combine(current, part);

            segments.Add(new PathSegment { Name = part, Path = current });
        }

        return segments;
    }

    private static int CountChecked(IEnumerable<FolderNode> nodes) {
        var count = 0;

        foreach (var node in nodes) {
            if (node.IsChecked) {
                count++;
            }

            count += CountChecked(node.Children);
        }

        return count;
    }
}
