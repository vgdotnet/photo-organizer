using System.Collections.Generic;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.Services;

public interface IFileSystemService {
    IReadOnlyList<FolderItem> GetDrives();

    IReadOnlyList<FolderItem> GetDirectories(string path);
}
