using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.ViewModels;

public partial class FolderNode : ObservableObject {
    public FolderNode(FolderItem item) {
        Name = item.Name;
        Path = item.Path;
        IsDrive = item.IsDrive;
        HasUnrealizedChildren = true;
    }

    public string Name { get; }

    public string Path { get; }

    public bool IsDrive { get; }

    public bool IsLoaded { get; set; }

    public ObservableCollection<FolderNode> Children { get; } = new();

    [ObservableProperty]
    public partial bool HasUnrealizedChildren { get; set; }

    public override string ToString() => Name;
}
