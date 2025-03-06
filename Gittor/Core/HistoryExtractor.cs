using Gittor.Formatting;
using Gittor.Git;
using Gittor.Output;
using System.Text;

namespace Gittor.Core;

/// <summary>
/// Extracts Git history and generates markdown files.
/// </summary>
public class HistoryExtractor : IDisposable
{
    private readonly IRepositoryAnalyzer _repositoryAnalyzer;
    private readonly IFileFilter _fileFilter;
    private readonly IContentFormatter _contentFormatter;
    private readonly IOutputGenerator _outputGenerator;
    private readonly FormattingOptions _formattingOptions;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryExtractor"/> class.
    /// </summary>
    /// <param name="repositoryPath">The path to the Git repository.</param>
    public HistoryExtractor(string repositoryPath)
        : this(new GitRepositoryAnalyzer(repositoryPath), 
            new DefaultFileFilter(), 
            new MarkdownFormatter(), 
            new FormattingOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryExtractor"/> class.
    /// </summary>
    /// <param name="repositoryAnalyzer">The repository analyzer to use.</param>
    /// <param name="fileFilter">The file filter to use.</param>
    /// <param name="contentFormatter">The content formatter to use.</param>
    /// <param name="formattingOptions">The formatting options.</param>
    public HistoryExtractor(
        IRepositoryAnalyzer repositoryAnalyzer, 
        IFileFilter fileFilter, 
        IContentFormatter contentFormatter, 
        FormattingOptions formattingOptions)
    {
        _repositoryAnalyzer = repositoryAnalyzer ?? throw new ArgumentNullException(nameof(repositoryAnalyzer));
        _fileFilter = fileFilter ?? throw new ArgumentNullException(nameof(fileFilter));
        _contentFormatter = contentFormatter ?? throw new ArgumentNullException(nameof(contentFormatter));
        _formattingOptions = formattingOptions ?? throw new ArgumentNullException(nameof(formattingOptions));
        _outputGenerator = new MarkdownOutputGenerator(_contentFormatter, _formattingOptions);
    }

    /// <summary>
    /// Gets the total number of commits in the repository.
    /// </summary>
    /// <returns>The total number of commits.</returns>
    public int GetTotalCommitCount()
    {
        return _repositoryAnalyzer.GetTotalCommitCount();
    }

    /// <summary>
    /// Gets the total number of commits that match the author pattern.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <returns>The number of matching commits.</returns>
    public int GetMatchingCommitCount(string authorPattern)
    {
        return _repositoryAnalyzer.GetMatchingCommitCount(authorPattern);
    }

    /// <summary>
    /// Extracts Git history for the specified author and generates markdown files.
    /// </summary>
    /// <param name="authorPattern">The author pattern to match.</param>
    /// <param name="outputDirectory">The output directory.</param>
    /// <param name="progressCallback">An optional callback for progress updates.</param>
    /// <returns>Information about the extracted history.</returns>
    public ExtractionResult Extract(
        string authorPattern, 
        string outputDirectory, 
        Action<ExtractionProgress>? progressCallback = null)
    {
        // Ensure output directory exists
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Get total and matching commit counts
        int totalCommits = _repositoryAnalyzer.GetTotalCommitCount();
        int matchingCommits = _repositoryAnalyzer.GetMatchingCommitCount(authorPattern);
        
        // Create progress trackers
        int processedCommits = 0;
        int generatedFiles = 0;
        var startTime = DateTime.Now;
        
        // Report initial progress
        progressCallback?.Invoke(new ExtractionProgress(
            totalCommits,
            matchingCommits,
            processedCommits,
            generatedFiles,
            startTime,
            DateTime.Now,
            null));
        
        // Get commits
        var commits = _repositoryAnalyzer.GetCommits(authorPattern, _fileFilter);
        
        // Generate output
        var generatedFilePaths = _outputGenerator.Generate(
            commits,
            outputDirectory,
            progress =>
            {
                processedCommits = progress.ProcessedCommits;
                generatedFiles = progress.GeneratedFiles;
                
                progressCallback?.Invoke(new ExtractionProgress(
                    totalCommits,
                    matchingCommits,
                    processedCommits,
                    generatedFiles,
                    startTime,
                    DateTime.Now,
                    progress.CurrentFilePath));
            });
        
        // Create result
        var endTime = DateTime.Now;
        var result = new ExtractionResult(
            totalCommits,
            matchingCommits,
            generatedFiles,
            generatedFilePaths,
            startTime,
            endTime);
        
        // Report final progress
        progressCallback?.Invoke(new ExtractionProgress(
            totalCommits,
            matchingCommits,
            processedCommits,
            generatedFiles,
            startTime,
            endTime,
            null)
        {
            IsComplete = true
        });
        
        return result;
    }

    /// <summary>
    /// Disposes the resources used by the extractor.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources used by the extractor.
    /// </summary>
    /// <param name="disposing">true to dispose managed resources; otherwise, false.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_repositoryAnalyzer is IDisposable disposableAnalyzer)
                {
                    disposableAnalyzer.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="HistoryExtractor"/> class.
    /// </summary>
    ~HistoryExtractor()
    {
        Dispose(false);
    }
}

/// <summary>
/// Represents progress information for Git history extraction.
/// </summary>
public record ExtractionProgress(
    int TotalCommits,
    int MatchingCommits,
    int ProcessedCommits,
    int GeneratedFiles,
    DateTime StartTime,
    DateTime CurrentTime,
    string? CurrentFilePath)
{
    /// <summary>
    /// Gets a value indicating whether extraction is complete.
    /// </summary>
    public bool IsComplete { get; init; }
    
    /// <summary>
    /// Gets the percentage of completion.
    /// </summary>
    public double PercentComplete => MatchingCommits == 0 
        ? 100 
        : (double)ProcessedCommits / MatchingCommits * 100;
    
    /// <summary>
    /// Gets the elapsed time.
    /// </summary>
    public TimeSpan ElapsedTime => CurrentTime - StartTime;
}

/// <summary>
/// Represents the result of Git history extraction.
/// </summary>
public record ExtractionResult(
    int TotalCommits,
    int MatchingCommits,
    int GeneratedFiles,
    IReadOnlyList<string> GeneratedFilePaths,
    DateTime StartTime,
    DateTime EndTime)
{
    /// <summary>
    /// Gets the elapsed time.
    /// </summary>
    public TimeSpan ElapsedTime => EndTime - StartTime;
    
    /// <summary>
    /// Gets a value indicating whether any commits were found.
    /// </summary>
    public bool HasCommits => MatchingCommits > 0;
    
    /// <summary>
    /// Gets the summary of the extraction.
    /// </summary>
    /// <returns>A string containing the summary.</returns>
    public string GetSummary()
    {
        var builder = new StringBuilder();
        
        builder.AppendLine($"Total commits: {TotalCommits}");
        builder.AppendLine($"Matching commits: {MatchingCommits}");
        builder.AppendLine($"Generated files: {GeneratedFiles}");
        builder.AppendLine($"Elapsed time: {ElapsedTime.TotalSeconds:F2} seconds");
        
        return builder.ToString();
    }
} 