using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PhotoOrganizer.Services;

public sealed class ShellService : IShellService {
    private readonly ILogger<ShellService> _logger;

    public ShellService(ILogger<ShellService> logger) {
        _logger = logger;
    }

    public void OpenFile(string path) {
        try {
            var info = new ProcessStartInfo(path) {
                UseShellExecute = true,
            };

            Process.Start(info);

            _logger.LogDebug("Opened {Path} in the default application", path);
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Cannot open {Path} in the default application", path);
        }
    }
}
