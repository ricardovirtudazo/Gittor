using Gittor.Core;
using LibGit2Sharp;
using System.Text;
using System.Text.RegularExpressions;

namespace Gittor.Git;

/// <summary>
/// Implementation of <see cref="IRepositoryAnalyzer"/> that uses LibGit2Sharp.
/// </summary>
public class GitRepositoryAnalyzer : IRepositoryAnalyzer, IDisposable
{
    private readonly Repository _repository;
    private readonly string _repositoryPath;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitRepositoryAnalyzer"/> class.
    /// </summary>
    /// <param name="repositoryPath">The path to the Git repository.</param>
    public GitRepositoryAnalyzer(string repositoryPath)
    {
        if (string.IsNullOrEmpty(repositoryPath))
        {
            throw new ArgumentException("Repository path cannot be null or empty.", nameof(repositoryPath));
        }

        if (!Repository.IsValid(repositoryPath))
        {
            throw new ArgumentException($"The path '{repositoryPath}' is not a valid Git repository.", nameof(repositoryPath));
        }

        _repositoryPath = repositoryPath;
        _repository = new Repository(repositoryPath);
    }

    /// <summary>
    /// Gets the total number of commits in the repository.
    /// </summary>
    /// <returns>The total number of commits.</returns>
    public int GetTotalCommitCount()
    {
        return _repository.Commits.Count();
    }

    /// <summary>
    /// Gets the total number of commits that match the author pattern.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <returns>The number of matching commits.</returns>
    public int GetMatchingCommitCount(string authorPattern)
    {
        var regex = CreateAuthorRegex(authorPattern);
        return _repository.Commits.Count(c => regex.IsMatch(c.Author.Name) || regex.IsMatch(c.Author.Email));
    }

    /// <summary>
    /// Gets the commits that match the specified author pattern.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <param name="fileFilter">The file filter to apply to changes.</param>
    /// <returns>An enumerable of matching commits, ordered by date.</returns>
    public IEnumerable<Core.Commit> GetCommits(string authorPattern, IFileFilter fileFilter)
    {
        // Create regex for matching author name or email
        var regex = CreateAuthorRegex(authorPattern);
        
        // Get all commits that match the author pattern, ordered by date ascending
        foreach (var commit in _repository.Commits
            .Where(c => regex.IsMatch(c.Author.Name) || regex.IsMatch(c.Author.Email))
            .OrderBy(c => c.Author.When))
        {
            // Get parent commit (if any) for diffing
            var parent = commit.Parents.FirstOrDefault();
            
            // Determine if this is a merge commit
            bool isMergeCommit = commit.Parents.Count() > 1;
            
            // Get changes
            var changes = GetCommitChanges(commit, parent, fileFilter, isMergeCommit);
            
            // Extract commit message and description
            string message = commit.Message ?? string.Empty;
            string? description = null;
            
            // Split message into title and description (if there's more than one line)
            var messageParts = message.Split(new[] { '\n', '\r' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (messageParts.Length > 1)
            {
                message = messageParts[0].Trim();
                description = messageParts[1].Trim();
            }
            
            // Yield the commit
            yield return new Core.Commit(
                commit.Sha,
                commit.Sha.Substring(0, 8),
                commit.Author.When.DateTime,
                commit.Author.Name,
                message,
                description,
                isMergeCommit,
                changes.ToList()
            );
        }
    }

    private IEnumerable<CommitChange> GetCommitChanges(
        LibGit2Sharp.Commit commit, 
        LibGit2Sharp.Commit? parent, 
        IFileFilter fileFilter,
        bool isMergeCommit)
    {
        // If there's no parent, this is the first commit, so all files are added
        if (parent == null)
        {
            // For the initial commit, all files are considered added
            foreach (var entry in commit.Tree)
            {
                if (entry.TargetType == TreeEntryTargetType.Blob && fileFilter.ShouldInclude(entry.Path))
                {
                    var blob = (Blob)entry.Target;
                    string? content = GetBlobContent(blob);
                    string language = GetLanguageFromPath(entry.Path);
                    
                    yield return new CommitChange(
                        ChangeType.Added,
                        entry.Path,
                        null,
                        content,
                        language);
                }
            }
            
            yield break;
        }

        // For merge commits, we only show affected files without content if requested
        if (isMergeCommit)
        {
            // Get all changes between the commit and its parent
            // We won't include content for merge commits
            foreach (var change in _repository.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree))
            {
                if (fileFilter.ShouldInclude(change.Path))
                {
                    yield return new CommitChange(
                        MapChangeType(change.Status),
                        change.Path,
                        change.Status == ChangeKind.Renamed ? change.OldPath : null,
                        null, // No content for merge commits
                        GetLanguageFromPath(change.Path));
                }
            }
            
            yield break;
        }

        // For regular commits, show full diffs with content
        foreach (var change in _repository.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree))
        {
            if (!fileFilter.ShouldInclude(change.Path))
            {
                continue;
            }

            // Get content based on change type
            string? content = null;
            string language = GetLanguageFromPath(change.Path);
            
            switch (change.Status)
            {
                case ChangeKind.Added:
                    // For added files, get the content from the blob
                    var addedBlob = commit.Tree[change.Path].Target as Blob;
                    content = GetBlobContent(addedBlob);
                    break;
                    
                case ChangeKind.Deleted:
                    // For deleted files, get the content from the parent blob
                    var deletedBlob = parent.Tree[change.Path].Target as Blob;
                    content = GetBlobContent(deletedBlob);
                    break;
                    
                case ChangeKind.Modified:
                case ChangeKind.Renamed:
                    // For modified/renamed files, get the diff patch
                    var options = new CompareOptions { ContextLines = 3 };
                    
                    var patch = _repository.Diff.Compare<Patch>(
                        parent.Tree, 
                        commit.Tree, 
                        options);
                    content = patch.Content;
                    break;
            }
            
            yield return new CommitChange(
                MapChangeType(change.Status),
                change.Path,
                change.Status == ChangeKind.Renamed ? change.OldPath : null,
                content,
                language);
        }
    }

    private static string? GetBlobContent(Blob? blob)
    {
        if (blob == null)
        {
            return null;
        }

        // For binary blobs, don't return content
        if (blob.IsBinary)
        {
            return null;
        }

        // Try to get content as string, falling back to different encodings
        try
        {
            using var contentStream = blob.GetContentStream();
            using var reader = new StreamReader(contentStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            return reader.ReadToEnd();
        }
        catch (Exception)
        {
            try
            {
                // Try UTF-16 as fallback
                using var contentStream = blob.GetContentStream();
                using var reader = new StreamReader(contentStream, Encoding.Unicode);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                // Last resort: replace unsupported characters
                using var contentStream = blob.GetContentStream();
                using var reader = new StreamReader(
                    contentStream, 
                    Encoding.UTF8, 
                    detectEncodingFromByteOrderMarks: true, 
                    bufferSize: 4096, 
                    leaveOpen: false);
                
                var content = new StringBuilder();
                
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var ch = (char)reader.Read();
                        content.Append(ch);
                    }
                    catch
                    {
                        content.Append('\uFFFD'); // Unicode replacement character
                    }
                }
                
                return content.ToString();
            }
        }
    }

    private static string GetLanguageFromPath(string path)
    {
        // Get file extension without the dot
        var extension = Path.GetExtension(path)?.TrimStart('.')?.ToLowerInvariant() ?? string.Empty;
        
        // Map extensions to language identifiers for syntax highlighting
        return extension switch
        {
            "cs" => "csharp",
            "vb" => "vb",
            "xaml" => "xml",
            "xml" => "xml",
            "json" => "json",
            "md" => "markdown",
            "html" => "html",
            "css" => "css",
            "js" => "javascript",
            "ts" => "typescript",
            "jsx" => "jsx",
            "tsx" => "tsx",
            "ps1" => "powershell",
            "sh" => "bash",
            "bat" => "batch",
            "cmd" => "batch",
            "sql" => "sql",
            "yaml" => "yaml",
            "yml" => "yaml",
            "diff" => "diff",
            "gitignore" => "gitignore",
            "gitattributes" => "gitattributes",
            _ => string.Empty // Default to no language identifier
        };
    }

    private static ChangeType MapChangeType(ChangeKind kind)
    {
        return kind switch
        {
            ChangeKind.Added => ChangeType.Added,
            ChangeKind.Deleted => ChangeType.Deleted,
            ChangeKind.Modified => ChangeType.Modified,
            ChangeKind.Renamed => ChangeType.Modified, // Renamed is still a modification
            _ => ChangeType.Other
        };
    }

    private static Regex CreateAuthorRegex(string pattern)
    {
        // If pattern contains wildcards, treat it as a glob pattern
        if (pattern.Contains('*') || pattern.Contains('?'))
        {
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            
            return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        
        // Otherwise, treat it as a substring to match
        return new Regex(Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Disposes the repository.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the repository.
    /// </summary>
    /// <param name="disposing">true to dispose managed resources; otherwise, false.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _repository.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="GitRepositoryAnalyzer"/> class.
    /// </summary>
    ~GitRepositoryAnalyzer()
    {
        Dispose(false);
    }
} 