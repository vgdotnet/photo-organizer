using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PhotoOrganizer.Logging;

public sealed class LogFile {
    private const long MaxSize = 5 * 1024 * 1024;

    private const int KeptFiles = 5;

    private readonly object _gate = new();

    public LogFile() {
        FolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhotoOrganizer",
            "logs");

        FilePath = Path.Combine(FolderPath, "photo-organizer.log");
    }

    public string FolderPath { get; }

    public string FilePath { get; }

    public void Write(string message, Exception? exception) {
        var line = new StringBuilder();
        line.Append(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        line.Append(' ');
        line.Append(message);

        if (exception is not null) {
            line.AppendLine();
            line.Append(exception);
        }

        lock (_gate) {
            try {
                Directory.CreateDirectory(FolderPath);
                Roll();
                File.AppendAllText(FilePath, line.ToString() + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception) {
            }
        }
    }

    private void Roll() {
        var info = new FileInfo(FilePath);

        if (!info.Exists || info.Length < MaxSize) {
            return;
        }

        var oldest = $"{FilePath}.{KeptFiles}";

        if (File.Exists(oldest)) {
            File.Delete(oldest);
        }

        for (var index = KeptFiles - 1; index >= 1; index--) {
            var source = $"{FilePath}.{index}";

            if (File.Exists(source)) {
                File.Move(source, $"{FilePath}.{index + 1}");
            }
        }

        File.Move(FilePath, $"{FilePath}.1");
    }
}
