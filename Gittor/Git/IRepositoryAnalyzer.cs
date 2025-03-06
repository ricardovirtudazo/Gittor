using Gittor.Core;

namespace Gittor.Git;

/// <summary>
/// Defines methods for analyzing a Git repository.
/// </summary>
public interface IRepositoryAnalyzer
{
    /// <summary>
    /// Gets the total number of commits in the repository.
    /// </summary>
    /// <returns>The total number of commits.</returns>
    int GetTotalCommitCount();
    
    /// <summary>
    /// Gets the total number of commits that match the author pattern.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <returns>The number of matching commits.</returns>
    int GetMatchingCommitCount(string authorPattern);

    /// <summary>
    /// Gets the commits that match the specified author pattern.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <param name="fileFilter">The file filter to apply to changes.</param>
    /// <returns>An enumerable of matching commits, ordered by date.</returns>
    IEnumerable<Commit> GetCommits(string authorPattern, IFileFilter fileFilter);
} 