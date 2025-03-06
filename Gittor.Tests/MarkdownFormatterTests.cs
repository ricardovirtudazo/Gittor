using Gittor.Core;
using Gittor.Formatting;

namespace Gittor.Tests;

public class MarkdownFormatterTests
{
    [Fact]
    public void FormatCommit_WithBasicCommit_ReturnsCorrectMarkdown()
    {
        // Arrange
        var formatter = new MarkdownFormatter();
        var options = new FormattingOptions();
        
        var commit = new Commit(
            "1234567890abcdef",
            "1234567",
            new DateTime(2023, 1, 1),
            "John Doe",
            "Add new feature",
            "This is a description of the feature.",
            false,
            new List<CommitChange>());
        
        // Act
        var result = formatter.FormatCommit(commit, options);
        
        // Assert
        Assert.Contains("## 1234567 (2023-01-01)", result);
        Assert.Contains("Add new feature", result);
        Assert.Contains("This is a description of the feature.", result);
        Assert.Contains("*No changes in this commit.*", result);
    }
    
    [Fact]
    public void FormatCommit_WithMergeCommit_FormatsCorrectly()
    {
        // Arrange
        var formatter = new MarkdownFormatter();
        var options = new FormattingOptions();
        
        var changes = new List<CommitChange>
        {
            new CommitChange(ChangeType.Modified, "file1.cs", null, "content", "csharp"),
            new CommitChange(ChangeType.Modified, "file2.cs", null, "content", "csharp")
        };
        
        var commit = new Commit(
            "1234567890abcdef",
            "1234567",
            new DateTime(2023, 1, 1),
            "John Doe",
            "Merge branch 'feature' into main",
            null,
            true,
            changes);
        
        // Act
        var result = formatter.FormatCommit(commit, options);
        
        // Assert
        Assert.Contains("## 1234567 (2023-01-01)", result);
        Assert.Contains("Merge branch 'feature' into main", result);
        Assert.Contains("### Merge Commit", result);
        Assert.Contains("- file1.cs", result);
        Assert.Contains("- file2.cs", result);
    }
    
    [Fact]
    public void FormatChanges_WithAddedFiles_FormatsCorrectly()
    {
        // Arrange
        var formatter = new MarkdownFormatter();
        var options = new FormattingOptions();
        
        var changes = new List<CommitChange>
        {
            new CommitChange(ChangeType.Added, "file1.cs", null, "public class Test {}", "csharp"),
            new CommitChange(ChangeType.Added, "file2.cs", null, null, "csharp")
        };
        
        var commit = new Commit(
            "1234567890abcdef",
            "1234567",
            new DateTime(2023, 1, 1),
            "John Doe",
            "Add new files",
            null,
            false,
            changes);
        
        // Act
        var result = formatter.FormatChanges(commit, options);
        
        // Assert
        Assert.Contains("### Added:", result);
        Assert.Contains("#### file1.cs", result);
        Assert.Contains("```csharp", result);
        Assert.Contains("public class Test {}", result);
        Assert.Contains("#### Others", result);
        Assert.Contains("- file2.cs", result);
    }
    
    [Fact]
    public void TruncateContent_WithLongContent_TruncatesCorrectly()
    {
        // Arrange
        var formatter = new MarkdownFormatter();
        var longContent = new string('a', 1000);
        
        // Act
        var result = formatter.TruncateContent(longContent, 500, "csharp");
        
        // Assert
        Assert.True(result.Length <= 500);
        Assert.Contains("... additional changes truncated ...", result);
    }
    
    [Fact]
    public void TruncateContent_WithShortContent_ReturnsOriginal()
    {
        // Arrange
        var formatter = new MarkdownFormatter();
        var shortContent = "This is a short content";
        
        // Act
        var result = formatter.TruncateContent(shortContent, 500, "csharp");
        
        // Assert
        Assert.Equal(shortContent, result);
    }
} 