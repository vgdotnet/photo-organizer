using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PhotoOrganizer.Logging;

public sealed class FileLoggerProvider : ILoggerProvider {
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

    private readonly LogFile _file;

    public FileLoggerProvider(LogFile file) {
        _file = file;
    }

    public ILogger CreateLogger(string categoryName) {
        return _loggers.GetOrAdd(ShortName(categoryName), name => new FileLogger(name, _file));
    }

    public void Dispose() {
        _loggers.Clear();
    }

    private static string ShortName(string categoryName) {
        var index = categoryName.LastIndexOf('.');

        return index >= 0 && index < categoryName.Length - 1
            ? categoryName[(index + 1)..]
            : categoryName;
    }
}
