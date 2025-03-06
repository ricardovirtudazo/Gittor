using Gittor.Core;
using System.Text;

namespace Gittor.Formatting;

/// <summary>
/// Implementation of <see cref="IContentFormatter"/> that formats content as markdown.
/// </summary>
public class MarkdownFormatter : IContentFormatter
{
    private const string TruncationMarker = "... additional changes truncated ...";

    /// <summary>
    /// Formats a commit as markdown.
    /// </summary>
    /// <param name="commit">The commit to format.</param>
    /// <param name="options">The formatting options.</param>
    /// <returns>The formatted markdown.</returns>
    public string FormatCommit(Commit commit, FormattingOptions options)
    {
        var builder = new StringBuilder();

        // H2: Commit Header
        builder.AppendLine($"## {commit.ShortHash} ({commit.Date:yyyy-MM-dd})");
        builder.AppendLine();

        // Commit message
        builder.AppendLine(commit.Title);
        
        if (!string.IsNullOrEmpty(commit.Description))
        {
            builder.AppendLine();
            builder.AppendLine(commit.Description);
        }
        
        builder.AppendLine();

        // Add changes if the commit has any
        if (commit.HasChanges)
        {
            builder.Append(FormatChanges(commit, options));
        }
        else
        {
            builder.AppendLine("*No changes in this commit.*");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formats a commit's changes as markdown.
    /// </summary>
    /// <param name="commit">The commit containing the changes.</param>
    /// <param name="options">The formatting options.</param>
    /// <returns>The formatted markdown.</returns>
    public string FormatChanges(Commit commit, FormattingOptions options)
    {
        var builder = new StringBuilder();
        
        // Group changes by type
        var added = commit.Changes.Where(c => c.Type == ChangeType.Added).ToList();
        var deleted = commit.Changes.Where(c => c.Type == ChangeType.Deleted).ToList();
        var modified = commit.Changes.Where(c => c.Type == ChangeType.Modified).ToList();
        var other = commit.Changes.Where(c => c.Type == ChangeType.Other).ToList();

        // Special case for merge commits
        if (commit.IsMergeCommit && !options.ShowMergeCommitContent)
        {
            builder.AppendLine("### Merge Commit");
            builder.AppendLine();
            builder.AppendLine("*Affected files:*");
            builder.AppendLine();
            
            foreach (var change in commit.Changes.OrderBy(c => c.Path))
            {
                builder.AppendLine($"- {change.Path}");
            }
            
            builder.AppendLine();
            return builder.ToString();
        }

        // Added files
        if (added.Count > 0)
        {
            builder.AppendLine("### Added:");
            builder.AppendLine();
            
            // First, process files with content
            var contentFiles = added.Where(c => c.HasContent).ToList();
            
            foreach (var change in contentFiles)
            {
                builder.AppendLine($"#### {change.Path}");
                builder.AppendLine();
                
                if (!string.IsNullOrEmpty(change.Content))
                {
                    string language = string.IsNullOrEmpty(change.Language) ? "" : change.Language;
                    string truncatedContent = TruncateContent(change.Content, 5000, change.Language);
                    
                    builder.AppendLine($"```{language}");
                    builder.AppendLine(truncatedContent);
                    builder.AppendLine("```");
                    builder.AppendLine();
                }
            }
            
            // Then, handle other files without content
            var otherFiles = added.Except(contentFiles).ToList();
            
            if (otherFiles.Count > 0)
            {
                builder.AppendLine("#### Others");
                builder.AppendLine();
                
                foreach (var change in otherFiles.OrderBy(c => c.Path))
                {
                    builder.AppendLine($"- {change.Path}");
                }
                
                builder.AppendLine();
            }
        }

        // Deleted files
        if (deleted.Count > 0)
        {
            builder.AppendLine("### Deleted:");
            builder.AppendLine();
            
            // First, process files with content
            var contentFiles = deleted.Where(c => c.HasContent).ToList();
            
            foreach (var change in contentFiles)
            {
                builder.AppendLine($"#### {change.Path}");
                builder.AppendLine();
                
                if (!string.IsNullOrEmpty(change.Content))
                {
                    builder.AppendLine("```diff");
                    
                    // For deleted files, prefix each line with '-'
                    var lines = change.Content.Split('\n');
                    var truncatedLines = TruncateLines(lines, 50);
                    
                    foreach (var line in truncatedLines)
                    {
                        builder.AppendLine($"-{line}");
                    }
                    
                    builder.AppendLine("```");
                    builder.AppendLine();
                }
            }
            
            // Then, handle other files without content
            var otherFiles = deleted.Except(contentFiles).ToList();
            
            if (otherFiles.Count > 0)
            {
                builder.AppendLine("#### Others");
                builder.AppendLine();
                
                foreach (var change in otherFiles.OrderBy(c => c.Path))
                {
                    builder.AppendLine($"- {change.Path}");
                }
                
                builder.AppendLine();
            }
        }

        // Modified/Renamed files
        if (modified.Count > 0)
        {
            builder.AppendLine("### Modified/Renamed:");
            builder.AppendLine();
            
            // First, process files with content
            var contentFiles = modified.Where(c => c.HasContent).ToList();
            
            foreach (var change in contentFiles)
            {
                builder.AppendLine($"#### {change.Path}");
                builder.AppendLine();
                
                if (change.IsRename && !string.IsNullOrEmpty(change.OldPath))
                {
                    builder.AppendLine($"```diff");
                    builder.AppendLine($"similarity index 98%");
                    builder.AppendLine($"rename from {change.OldPath}");
                    builder.AppendLine($"rename to {change.Path}");
                    
                    if (!string.IsNullOrEmpty(change.Content))
                    {
                        builder.Append(TruncateContent(change.Content, 5000, "diff"));
                    }
                    
                    builder.AppendLine("```");
                }
                else if (!string.IsNullOrEmpty(change.Content))
                {
                    builder.AppendLine("```diff");
                    builder.Append(TruncateContent(change.Content, 5000, "diff"));
                    builder.AppendLine("```");
                }
                
                builder.AppendLine();
            }
            
            // Then, handle other files without content
            var otherFiles = modified.Except(contentFiles).ToList();
            
            if (otherFiles.Count > 0)
            {
                builder.AppendLine("#### Others");
                builder.AppendLine();
                
                foreach (var change in otherFiles.OrderBy(c => c.Path))
                {
                    var description = change.IsRename && !string.IsNullOrEmpty(change.OldPath)
                        ? $"{change.Path} (from {change.OldPath})"
                        : change.Path;
                    
                    builder.AppendLine($"- {description}");
                }
                
                builder.AppendLine();
            }
        }

        // Other changes
        if (other.Count > 0)
        {
            builder.AppendLine("### Other Changes:");
            builder.AppendLine();
            
            foreach (var change in other.OrderBy(c => c.Path))
            {
                builder.AppendLine($"- {change.Type} {change.Path}");
            }
            
            builder.AppendLine();
        }

        return builder.ToString();
    }

    /// <summary>
    /// Truncates content if it exceeds the specified length.
    /// </summary>
    /// <param name="content">The content to truncate.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="language">The language of the content.</param>
    /// <returns>The truncated content.</returns>
    public string TruncateContent(string content, int maxLength, string? language)
    {
        if (string.IsNullOrEmpty(content) || content.Length <= maxLength)
        {
            return content ?? string.Empty;
        }

        // Split content into lines
        var lines = content.Split('\n');
        
        // If the content is already short, return it as is
        if (content.Length <= maxLength)
        {
            return content;
        }

        // Truncate based on number of lines
        var truncatedLines = TruncateLines(lines, 50);
        var truncated = string.Join('\n', truncatedLines);
        
        // If still too long, hard truncate
        if (truncated.Length > maxLength)
        {
            truncated = truncated.Substring(0, maxLength - TruncationMarker.Length) + TruncationMarker;
        }
        
        return truncated;
    }

    private static string[] TruncateLines(string[] lines, int maxLines)
    {
        if (lines.Length <= maxLines)
        {
            return lines;
        }

        // Take half of the lines from the beginning and half from the end
        int halfLines = maxLines / 2;
        
        var truncatedLines = new string[maxLines + 1];
        Array.Copy(lines, 0, truncatedLines, 0, halfLines);
        truncatedLines[halfLines] = TruncationMarker;
        Array.Copy(lines, lines.Length - halfLines, truncatedLines, halfLines + 1, halfLines);
        
        return truncatedLines;
    }
} 