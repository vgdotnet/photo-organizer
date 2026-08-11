using PhotoOrganizer.Models;

namespace PhotoOrganizer.ViewModels;

public sealed partial class FolderTile : ContentTile {
    public FolderTile(FolderItem folder) : base(folder.Name, folder.Path) {
    }
}
