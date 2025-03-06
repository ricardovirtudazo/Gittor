using Gittor.Core;

namespace Gittor.Formatting;

/// <summary>
/// Defines methods for formatting Git history content as markdown.
/// </summary>
public interface IContentFormatter
{
    /// <summary>
    /// Formats a commit as markdown.
    /// </summary>
    /// <param name="commit">The commit to format.</param>
    /// <param name="options">The formatting options.</param>
    /// <returns>The formatted markdown.</returns>
    string FormatCommit(Commit commit, FormattingOptions options);
    
    /// <summary>
    /// Formats a commit's changes as markdown.
    /// </summary>
    /// <param name="commit">The commit containing the changes.</param>
    /// <param name="options">The formatting options.</param>
    /// <returns>The formatted markdown.</returns>
    string FormatChanges(Commit commit, FormattingOptions options);
    
    /// <summary>
    /// Truncates content if it exceeds the specified length.
    /// </summary>
    /// <param name="content">The content to truncate.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="language">The language of the content.</param>
    /// <returns>The truncated content.</returns>
    string TruncateContent(string content, int maxLength, string? language);
}

/// <summary>
/// Options for formatting content.
/// </summary>
public record FormattingOptions(
    /// <summary>
    /// Gets the maximum characters per file.
    /// </summary>
    int MaxCharactersPerFile = 700_000,
    
    /// <summary>
    /// Gets the percentage of the maximum characters to reserve for content.
    /// </summary>
    double ContentThresholdPercentage = 0.9,
    
    /// <summary>
    /// Gets the number of context lines to show in diffs.
    /// </summary>
    int ContextLines = 3,
    
    /// <summary>
    /// Gets a value indicating whether to show file content for merge commits.
    /// </summary>
    bool ShowMergeCommitContent = false)
{
    /// <summary>
    /// Gets the number of characters to reserve for content.
    /// </summary>
    public int ContentThreshold => (int)(MaxCharactersPerFile * ContentThresholdPercentage);
} 