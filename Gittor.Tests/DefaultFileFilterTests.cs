using Gittor.Core;

namespace Gittor.Tests;

public class DefaultFileFilterTests
{
    [Fact]
    public void ShouldInclude_SourceCodeFiles_ReturnsTrue()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.True(filter.ShouldInclude("src/Program.cs"));
        Assert.True(filter.ShouldInclude("src/MainWindow.xaml"));
        Assert.True(filter.ShouldInclude("src/Form1.vb"));
        Assert.True(filter.ShouldInclude("src/Component.razor"));
    }
    
    [Fact]
    public void ShouldInclude_ConfigFiles_ReturnsTrue()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.True(filter.ShouldInclude("src/Project.csproj"));
        Assert.True(filter.ShouldInclude("src/app.config"));
        Assert.True(filter.ShouldInclude("src/appsettings.json"));
    }
    
    [Fact]
    public void ShouldInclude_DocumentationFiles_ReturnsTrue()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.True(filter.ShouldInclude("README.md"));
        Assert.True(filter.ShouldInclude("docs/CHANGELOG.md"));
    }
    
    [Fact]
    public void ShouldInclude_BinaryFiles_ReturnsFalse()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.False(filter.ShouldInclude("bin/Debug/app.exe"));
        Assert.False(filter.ShouldInclude("bin/Debug/app.dll"));
        Assert.False(filter.ShouldInclude("obj/Debug/app.pdb"));
    }
    
    [Fact]
    public void ShouldInclude_MediaFiles_ReturnsFalse()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.False(filter.ShouldInclude("assets/logo.png"));
        Assert.False(filter.ShouldInclude("assets/background.jpg"));
        Assert.False(filter.ShouldInclude("assets/icon.ico"));
    }
    
    [Fact]
    public void ShouldInclude_OfficeDocuments_ReturnsFalse()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.False(filter.ShouldInclude("docs/report.docx"));
        Assert.False(filter.ShouldInclude("docs/data.xlsx"));
        Assert.False(filter.ShouldInclude("docs/presentation.pptx"));
        Assert.False(filter.ShouldInclude("docs/document.pdf"));
    }
    
    [Fact]
    public void ShouldInclude_ArchiveFiles_ReturnsFalse()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.False(filter.ShouldInclude("archives/data.zip"));
        Assert.False(filter.ShouldInclude("archives/backup.rar"));
        Assert.False(filter.ShouldInclude("archives/files.7z"));
    }
    
    [Fact]
    public void ShouldInclude_GeneratedCode_ReturnsFalse()
    {
        // Arrange
        var filter = new DefaultFileFilter();
        
        // Act & Assert
        Assert.False(filter.ShouldInclude("src/Form1.designer.cs"));
        Assert.False(filter.ShouldInclude("src/Resource.g.cs"));
        Assert.False(filter.ShouldInclude("src/TemporaryGeneratedFile.g.i.cs"));
    }
}
