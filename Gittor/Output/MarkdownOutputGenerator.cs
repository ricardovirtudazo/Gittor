using Gittor.Core;
using Gittor.Formatting;
using System.Text;

namespace Gittor.Output;

/// <summary>
/// Implementation of <see cref="IOutputGenerator"/> that generates markdown files.
/// </summary>
public class MarkdownOutputGenerator : IOutputGenerator
{
    private readonly IContentFormatter _formatter;
    private readonly FormattingOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownOutputGenerator"/> class.
    /// </summary>
    /// <param name="formatter">The formatter to use.</param>
    /// <param name="options">The formatting options.</param>
    public MarkdownOutputGenerator(IContentFormatter formatter, FormattingOptions options)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _options = options;
    }

    /// <summary>
    /// Generates Git history output.
    /// </summary>
    /// <param name="commits">The commits to include.</param>
    /// <param name="outputDirectory">The output directory.</param>
    /// <param name="progressCallback">An optional callback for progress updates.</param>
    /// <returns>The generated file paths.</returns>
    public IReadOnlyList<string> Generate(
        IEnumerable<Commit> commits, 
        string outputDirectory, 
        Action<GenerationProgress>? progressCallback = null)
    {
        // Ensure output directory exists
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Track generated files
        var generatedFiles = new List<string>();
        
        // Convert commits to list for tracking progress
        var commitsList = commits.ToList();
        int totalCommits = commitsList.Count;
        int processedCommits = 0;
        
        // If there are no commits, return empty list
        if (totalCommits == 0)
        {
            progressCallback?.Invoke(new GenerationProgress(0, 0, 0, null));
            return generatedFiles;
        }
        
        // Track date range for file naming
        DateTime startDate = commitsList.Min(c => c.Date);
        DateTime endDate = commitsList.Max(c => c.Date);
        
        // Prepare first file
        int fileSequence = 1;
        string currentFilePath = GetFilePath(outputDirectory, startDate, endDate, fileSequence);
        generatedFiles.Add(currentFilePath);
        
        // Process each commit
        StreamWriter? writer = null;
        int currentFileCharCount = 0;
        
        try
        {
            // Initialize the first file writer
            writer = new StreamWriter(currentFilePath, false, Encoding.UTF8);
            
            // Get header for first file
            var header = GetFileHeader(startDate, endDate, totalCommits, 1);
            
            // Write header to file
            writer.Write(header);
            currentFileCharCount += header.Length;
            
            // Process each commit
            foreach (var commit in commitsList)
            {
                // Format commit as markdown
                string commitMarkdown = _formatter.FormatCommit(commit, _options);
                
                // Check if adding this commit would exceed the threshold
                if (currentFileCharCount + commitMarkdown.Length > _options.ContentThreshold)
                {
                    // Close current file
                    writer.Flush();
                    writer.Dispose();
                    
                    // Create new file
                    fileSequence++;
                    currentFilePath = GetFilePath(outputDirectory, startDate, endDate, fileSequence);
                    generatedFiles.Add(currentFilePath);
                    
                    // Create a new writer for the new file
                    writer = new StreamWriter(currentFilePath, false, Encoding.UTF8);
                    
                    // Get header for new file (continued)
                    var newHeader = GetFileHeader(startDate, endDate, totalCommits, fileSequence);
                    
                    // Write header to new file
                    writer.Write(newHeader);
                    currentFileCharCount = newHeader.Length;
                }
                
                // Write commit to current file
                writer.Write(commitMarkdown);
                currentFileCharCount += commitMarkdown.Length;
                
                // Update progress
                processedCommits++;
                progressCallback?.Invoke(new GenerationProgress(
                    totalCommits,
                    processedCommits,
                    generatedFiles.Count,
                    currentFilePath));
            }
        }
        finally
        {
            // Ensure the writer is disposed
            writer?.Dispose();
        }
        
        return generatedFiles;
    }

    private static string GetFileHeader(DateTime startDate, DateTime endDate, int totalCommits, int fileSequence)
    {
        var builder = new StringBuilder();
        
        // H1: File Information
        builder.AppendLine($"# Git History (Part {fileSequence})");
        builder.AppendLine();
        
        // Period and commit count
        builder.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} Total commits in this file: {totalCommits}");
        builder.AppendLine();
        
        return builder.ToString();
    }

    private static string GetFilePath(string outputDirectory, DateTime startDate, DateTime endDate, int sequence)
    {
        return Path.Combine(
            outputDirectory,
            $"git-history-{startDate:yyyy-MM-dd}-to-{endDate:yyyy-MM-dd}-part{sequence}.md");
    }
} 