namespace PhotoOrganizer.Models;

public sealed record FolderItem {
    public required string Name { get; init; }

    public required string Path { get; init; }

    public bool IsDrive { get; init; }
}
