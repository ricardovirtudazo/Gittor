namespace Gittor.Core;

/// <summary>
/// Defines methods for filtering files based on their paths.
/// </summary>
public interface IFileFilter
{
    /// <summary>
    /// Determines whether a file at the specified path should be included.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <returns>true if the file should be included; otherwise, false.</returns>
    bool ShouldInclude(string path);
} 