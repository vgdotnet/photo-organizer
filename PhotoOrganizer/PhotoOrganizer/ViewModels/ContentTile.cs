using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoOrganizer.ViewModels;

public abstract partial class ContentTile : ObservableObject {
    protected ContentTile(string name, string path) {
        Name = name;
        Path = path;
    }

    public string Name { get; }

    public string Path { get; }
}
