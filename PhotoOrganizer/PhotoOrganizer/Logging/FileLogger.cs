using System;
using Microsoft.Extensions.Logging;

namespace PhotoOrganizer.Logging;

public sealed class FileLogger : ILogger {
    private readonly string _category;

    private readonly LogFile _file;

    public FileLogger(string category, LogFile file) {
        _category = category;
        _file = file;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel)) {
            return;
        }

        var message = $"[{Format(logLevel)}] {_category}: {formatter(state, exception)}";

        _file.Write(message, exception);
    }

    private static string Format(LogLevel level) {
        return level switch {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "---",
        };
    }
}
