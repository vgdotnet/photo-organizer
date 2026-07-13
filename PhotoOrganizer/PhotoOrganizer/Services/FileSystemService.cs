using System;
using System.Collections.Generic;
using System.IO;
using PhotoOrganizer.Models;

namespace PhotoOrganizer.Services;

public sealed class FileSystemService : IFileSystemService {
    public IReadOnlyList<FolderItem> GetDrives() {
        var drives = new List<FolderItem>();

        foreach (var drive in DriveInfo.GetDrives()) {
            if (!drive.IsReady) {
                continue;
            }

            var letter = drive.Name.TrimEnd('\\', '/');
            var label = string.IsNullOrWhiteSpace(drive.VolumeLabel) ? "Local Disk" : drive.VolumeLabel;

            drives.Add(new FolderItem {
                Name = $"{label} ({letter})",
                Path = drive.RootDirectory.FullName,
                IsDrive = true,
            });
        }

        return drives;
    }

    public IReadOnlyList<FolderItem> GetDirectories(string path) {
        var result = new List<FolderItem>();

        IEnumerable<string> directories;
        try {
            directories = Directory.EnumerateDirectories(path);
        }
        catch (Exception) {
            return result;
        }

        foreach (var directory in directories) {
            try {
                var info = new DirectoryInfo(directory);

                if (info.Attributes.HasFlag(FileAttributes.Hidden) && info.Attributes.HasFlag(FileAttributes.System)) {
                    continue;
                }

                result.Add(new FolderItem {
                    Name = info.Name,
                    Path = info.FullName,
                    IsDrive = false,
                });
            }
            catch (Exception) {
            }
        }

        result.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

        return result;
    }
}
