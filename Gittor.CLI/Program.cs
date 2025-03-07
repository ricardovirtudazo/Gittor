using System.CommandLine;

var rootCommand = new RootCommand("Gittor - Extract meaningful Git history into structured markdown files");

var repoPathArg = new Argument<string>(
    name: "repo-path",
    description: "Path to the Git repository to analyze");

var outputDirArg = new Argument<string>(
    name: "output-dir",
    description: "Directory where markdown files will be generated");

var authorPatternArg = new Argument<string>(
    name: "author-pattern",
    description: "Pattern to filter commits by author (supports glob patterns)");

rootCommand.AddArgument(repoPathArg);
rootCommand.AddArgument(outputDirArg);
rootCommand.AddArgument(authorPatternArg);

// Add handler that will be implemented later
rootCommand.SetHandler((string repoPath, string outputDir, string authorPattern) =>
{
    Console.WriteLine($"Repository path: {repoPath}");
    Console.WriteLine($"Output directory: {outputDir}");
    Console.WriteLine($"Author pattern: {authorPattern}");
    
    // TODO: Implement actual functionality
    return Task.FromResult(0);
}, repoPathArg, outputDirArg, authorPatternArg);

return await rootCommand.InvokeAsync(args);
