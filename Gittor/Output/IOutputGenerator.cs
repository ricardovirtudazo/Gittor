using Gittor.Core;

namespace Gittor.Output;

/// <summary>
/// Defines methods for generating Git history output.
/// </summary>
public interface IOutputGenerator
{
    /// <summary>
    /// Generates Git history output.
    /// </summary>
    /// <param name="commits">The commits to include.</param>
    /// <param name="outputDirectory">The output directory.</param>
    /// <param name="progressCallback">An optional callback for progress updates.</param>
    /// <returns>The generated file paths.</returns>
    IReadOnlyList<string> Generate(
        IEnumerable<Commit> commits, 
        string outputDirectory, 
        Action<GenerationProgress>? progressCallback = null);
}

/// <summary>
/// Represents progress information for Git history generation.
/// </summary>
public record GenerationProgress(
    int TotalCommits,
    int ProcessedCommits,
    int GeneratedFiles,
    string? CurrentFilePath)
{
    /// <summary>
    /// Gets a value indicating whether generation is complete.
    /// </summary>
    public bool IsComplete => ProcessedCommits == TotalCommits;
    
    /// <summary>
    /// Gets the percentage of completion.
    /// </summary>
    public double PercentComplete => TotalCommits == 0 
        ? 100 
        : (double)ProcessedCommits / TotalCommits * 100;
} 