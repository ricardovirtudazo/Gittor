namespace Gittor.Core;

/// <summary>
/// Represents a Git commit with its metadata and changes.
/// </summary>
public record Commit(
    string Hash,
    string ShortHash,
    DateTime Date,
    string Author,
    string Message,
    string? Description,
    bool IsMergeCommit,
    IReadOnlyList<CommitChange> Changes)
{
    /// <summary>
    /// Gets a value indicating whether this commit has any changes.
    /// </summary>
    public bool HasChanges => Changes.Count > 0;

    /// <summary>
    /// Gets the title of the commit (first line of the message).
    /// </summary>
    public string Title => Message.Split('\n')[0].Trim();
} 