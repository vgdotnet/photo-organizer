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
            DestinationTree.Add(new FolderNode(drive));
        }
    }

    [ObservableProperty]
    public partial string Title { get; set; }

    public ObservableCollection<FolderNode> DestinationTree { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DestinationHint))]
    public partial FolderNode? SelectedDestination { get; set; }

    public string DestinationHint => SelectedDestination?.Path ?? "pick one folder";

    public async Task LoadChildrenAsync(FolderNode node) {
        if (node.IsLoaded) {
            return;
        }

        node.IsLoaded = true;

        var items = await Task.Run(() => _fileSystem.GetDirectories(node.Path));

        foreach (var item in items) {
            node.Children.Add(new FolderNode(item));
        }

        node.HasUnrealizedChildren = false;
    }
}
