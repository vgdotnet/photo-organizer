using System;
using System.Diagnostics;

namespace PhotoOrganizer.Services;

public sealed class ShellService : IShellService {
    public void OpenFile(string path) {
        try {
            var info = new ProcessStartInfo(path) {
                UseShellExecute = true,
            };

            Process.Start(info);
        }
        catch (Exception) {
        }
    }
}
