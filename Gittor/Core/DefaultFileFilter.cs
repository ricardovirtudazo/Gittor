using System.Text.RegularExpressions;

namespace Gittor.Core;

/// <summary>
/// Default implementation of <see cref="IFileFilter"/> that filters files based on patterns.
/// </summary>
public class DefaultFileFilter : IFileFilter
{
    private readonly Regex[] _inclusionPatterns;
    private readonly Regex[] _exclusionPatterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultFileFilter"/> class.
    /// </summary>
    public DefaultFileFilter()
    {
        // Essential file patterns to include
        _inclusionPatterns = new[]
        {
            // Source code
            CreateRegexFromGlob("*.cs"),
            CreateRegexFromGlob("*.vb"),
            CreateRegexFromGlob("*.xaml"),
            CreateRegexFromGlob("*.xaml.cs"),
            CreateRegexFromGlob("*.razor"),
            CreateRegexFromGlob("*.razor.cs"),
            
            // Config files
            CreateRegexFromGlob("*.csproj"),
            CreateRegexFromGlob("*.vbproj"),
            CreateRegexFromGlob("*.sln"),
            CreateRegexFromGlob("app.config"),
            CreateRegexFromGlob("web.config"),
            CreateRegexFromGlob("*.json"),
            
            // Documentation
            CreateRegexFromGlob("README.md"),
            CreateRegexFromGlob("CHANGELOG.md"),
            CreateRegexFromGlob("*.md"),
            
            // Scripts
            CreateRegexFromGlob("*.ps1"),
            CreateRegexFromGlob("*.sh"),
            CreateRegexFromGlob("*.bat"),
            CreateRegexFromGlob("*.cmd"),
            
            // Web files
            CreateRegexFromGlob("*.html"),
            CreateRegexFromGlob("*.css"),
            CreateRegexFromGlob("*.js"),
            CreateRegexFromGlob("*.ts"),
            CreateRegexFromGlob("*.jsx"),
            CreateRegexFromGlob("*.tsx")
        };

        // Global exclusions
        _exclusionPatterns = new[]
        {
            // Binaries and executables
            CreateRegexFromGlob("*.exe"),
            CreateRegexFromGlob("*.dll"),
            CreateRegexFromGlob("*.pdb"),
            CreateRegexFromGlob("*.obj"),
            CreateRegexFromGlob("*.bin"),
            
            // Build outputs
            CreateRegexFromGlob("bin/*"),
            CreateRegexFromGlob("obj/*"),
            CreateRegexFromGlob("**/bin/**"),
            CreateRegexFromGlob("**/obj/**"),
            
            // Generated code
            CreateRegexFromGlob("*.designer.cs"),
            CreateRegexFromGlob("*.generated.cs"),
            CreateRegexFromGlob("*.g.cs"),
            CreateRegexFromGlob("*.g.i.cs"),
            
            // Source control
            CreateRegexFromGlob(".git/*"),
            CreateRegexFromGlob(".github/*"),
            CreateRegexFromGlob(".vs/*"),
            
            // Media assets
            CreateRegexFromGlob("*.jpg"),
            CreateRegexFromGlob("*.jpeg"),
            CreateRegexFromGlob("*.png"),
            CreateRegexFromGlob("*.gif"),
            CreateRegexFromGlob("*.bmp"),
            CreateRegexFromGlob("*.ico"),
            CreateRegexFromGlob("*.svg"),
            
            // Office documents
            CreateRegexFromGlob("*.doc"),
            CreateRegexFromGlob("*.docx"),
            CreateRegexFromGlob("*.xls"),
            CreateRegexFromGlob("*.xlsx"),
            CreateRegexFromGlob("*.ppt"),
            CreateRegexFromGlob("*.pptx"),
            CreateRegexFromGlob("*.pdf"),
            
            // Archives
            CreateRegexFromGlob("*.zip"),
            CreateRegexFromGlob("*.rar"),
            CreateRegexFromGlob("*.7z"),
            CreateRegexFromGlob("*.tar"),
            CreateRegexFromGlob("*.gz")
        };
    }

    /// <summary>
    /// Determines whether a file at the specified path should be included.
    /// </summary>
    /// <param name="path">The path of the file to check.</param>
    /// <returns>true if the file should be included; otherwise, false.</returns>
    public bool ShouldInclude(string path)
    {
        // Normalize path to use forward slashes
        path = path.Replace('\\', '/');
        
        // Check exclusions first (if any pattern matches, exclude the file)
        foreach (var pattern in _exclusionPatterns)
        {
            if (pattern.IsMatch(path))
            {
                return false;
            }
        }
        
        // Then check inclusions (if any pattern matches, include the file)
        foreach (var pattern in _inclusionPatterns)
        {
            if (pattern.IsMatch(path))
            {
                return true;
            }
        }
        
        // By default, exclude files that don't match any inclusion pattern
        return false;
    }

    private static Regex CreateRegexFromGlob(string pattern)
    {
        // Convert glob pattern to regex
        string regexPattern = Regex.Escape(pattern)
            .Replace("\\*\\*", ".*")          // ** matches any path
            .Replace("\\*", "[^/]*")          // * matches anything except path separator
            .Replace("\\?", "[^/]");          // ? matches a single character
        
        return new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
} 