using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PhotoOrganizer.Services;

namespace PhotoOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject {
    private readonly IFileSystemService _fileSystem;

    public MainViewModel(IFileSystemService fileSystem) {
        _fileSystem = fileSystem;
        Title = "Photo Organizer";

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
