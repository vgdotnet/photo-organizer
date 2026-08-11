namespace PhotoOrganizer.ViewModels;

public sealed record PathSegment {
    public required string Name { get; init; }

    public required string Path { get; init; }

    public override string ToString() => Name;
}
