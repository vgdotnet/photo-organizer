using System;

namespace PhotoOrganizer.Models;

public sealed record PhotoFile {
    public required string Name { get; init; }

    public required string Path { get; init; }

    public required long Size { get; init; }

    public required DateTimeOffset Modified { get; init; }
}
