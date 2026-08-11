using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.ViewModels;

public partial class FolderNode : ObservableObject {
    public FolderNode(FolderItem item, bool isSource) {
        Name = item.Name;
        Path = item.Path;
        IsDrive = item.IsDrive;
        IsSource = isSource;
        HasUnrealizedChildren = true;
    }

    public string Name { get; }

    public string Path { get; }

    public bool IsDrive { get; }

    public bool IsSource { get; }

    public bool IsLoaded { get; set; }

    public ObservableCollection<FolderNode> Children { get; } = new();

    public event Action? CheckedChanged;

    [ObservableProperty]
    public partial bool HasUnrealizedChildren { get; set; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    partial void OnIsCheckedChanged(bool value) {
        CheckedChanged?.Invoke();
    }

    public override string ToString() => Name;
}
