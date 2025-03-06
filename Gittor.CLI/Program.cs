using Gittor.Core;
using Gittor.Formatting;
using System.CommandLine;

// Define commands and options
var rootCommand = new RootCommand("Gittor - Git History Extractor");

// Required arguments
var repoPathArgument = new Argument<string>(
    name: "repo-path",
    description: "The path to the Git repository");

var outputDirArgument = new Argument<string>(
    name: "output-dir",
    description: "The directory where markdown files will be generated");

var authorPatternArgument = new Argument<string>(
    name: "author-pattern",
    description: "The pattern to match author names or emails");

// Optional options with default values
var maxCharactersOption = new Option<int>(
    name: "--max-chars",
    description: "Maximum characters per file",
    getDefaultValue: () => 700_000);

var contentThresholdOption = new Option<double>(
    name: "--content-threshold",
    description: "Percentage of maximum characters to reserve for content (0.0-1.0)",
    getDefaultValue: () => 0.9);

var showMergeContentOption = new Option<bool>(
    name: "--show-merge-content",
    description: "Show file content for merge commits",
    getDefaultValue: () => false);

// Add options to root command
rootCommand.AddArgument(repoPathArgument);
rootCommand.AddArgument(outputDirArgument);
rootCommand.AddArgument(authorPatternArgument);
rootCommand.AddOption(maxCharactersOption);
rootCommand.AddOption(contentThresholdOption);
rootCommand.AddOption(showMergeContentOption);

// Define handler for the command
rootCommand.SetHandler((repoPath, outputDir, authorPattern, maxChars, contentThreshold, showMergeContent) =>
{
    // Create formatting options
    var options = new FormattingOptions(
        MaxCharactersPerFile: maxChars,
        ContentThresholdPercentage: contentThreshold,
        ContextLines: 3,
        ShowMergeCommitContent: showMergeContent);
    
    try
    {
        // Create extractor
        using var extractor = new HistoryExtractor(repoPath);
        
        // Display initial information
        Console.WriteLine($"Repository: {repoPath}");
        Console.WriteLine($"Output directory: {outputDir}");
        Console.WriteLine($"Author pattern: {authorPattern}");
        Console.WriteLine();
        
        int totalCommits = extractor.GetTotalCommitCount();
        int matchingCommits = extractor.GetMatchingCommitCount(authorPattern);
        
        Console.WriteLine($"Total commits: {totalCommits}");
        Console.WriteLine($"Matching commits: {matchingCommits}");
        Console.WriteLine();
        
        if (matchingCommits == 0)
        {
            Console.WriteLine("No commits found matching the author pattern.");
            return;
        }
        
        // Track progress
        Console.WriteLine("Processing commits...");
        
        // Extract history
        var result = extractor.Extract(
            authorPattern,
            outputDir,
            progress =>
            {
                // Update console with progress
                if (progress.IsComplete)
                {
                    // Final status
                    Console.WriteLine();
                    Console.WriteLine("Summary:");
                    Console.WriteLine($"- Total files generated: {progress.GeneratedFiles}");
                    Console.WriteLine($"- Total commits processed: {progress.ProcessedCommits}");
                    Console.WriteLine($"- Total time: {progress.ElapsedTime.TotalSeconds:F2} seconds");
                    Console.WriteLine("Done!");
                }
                else if (progress.CurrentFilePath != null)
                {
                    // Progress update
                    Console.Write($"\rProcessing commit {progress.ProcessedCommits}/{progress.MatchingCommits} " +
                                  $"({progress.PercentComplete:F2}%) - Generated: {progress.GeneratedFiles} files");
                }
            });
        
        if (result.GeneratedFiles > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Generated files in: {outputDir}");
            
            foreach (var filePath in result.GeneratedFilePaths)
            {
                Console.WriteLine($"- {Path.GetFileName(filePath)}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Console.Error.WriteLine(ex.StackTrace);
        Environment.Exit(1);
    }
}, repoPathArgument, outputDirArgument, authorPatternArgument, maxCharactersOption, contentThresholdOption, showMergeContentOption);

return await rootCommand.InvokeAsync(args);
