namespace Gittor.Core;

/// <summary>
/// Represents a change to a file in a commit.
/// </summary>
public record CommitChange(
    ChangeType Type,
    string Path,
    string? OldPath,
    string? Content,
    string? Language)
{
    /// <summary>
    /// Gets a value indicating whether this change has content.
    /// </summary>
    public bool HasContent => !string.IsNullOrEmpty(Content);

    /// <summary>
    /// Gets a value indicating whether this change represents a rename.
    /// </summary>
    public bool IsRename => Type == ChangeType.Modified && !string.IsNullOrEmpty(OldPath) && OldPath != Path;
}

/// <summary>
/// Represents the type of change made to a file.
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// File was added.
    /// </summary>
    Added,

    /// <summary>
    /// File was deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// File was modified.
    /// </summary>
    Modified,

    /// <summary>
    /// Other type of change.
    /// </summary>
    Other
} 