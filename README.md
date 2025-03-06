# Gittor - Git History Extractor

Gittor is a command-line tool that extracts Git history into structured markdown files, making it easy to review and share your contributions.

## Features

- Extract Git history for specific authors
- Filter out non-essential files (binaries, generated code, assets)
- Format code changes with proper syntax highlighting
- Split output into multiple files to respect token limits for AI analysis
- Handle merge commits with special formatting
- Support for various file types and languages

## Installation

### Prerequisites

- .NET 9.0 SDK or later

### Building from Source

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/Gittor.git
   cd Gittor
   ```

2. Build the project:
   ```
   dotnet build -c Release
   ```

3. Run the tool:
   ```
   dotnet run --project Gittor.CLI/Gittor.CLI.csproj -- <repo-path> <output-dir> <author-pattern>
   ```

## Usage

```
gittor <repo-path> <output-dir> <author-pattern> [options]
```

### Arguments

- `repo-path`: Path to the Git repository
- `output-dir`: Directory where markdown files will be generated
- `author-pattern`: Pattern to match author names or emails (supports wildcards)

### Options

- `--max-chars <value>`: Maximum characters per file (default: 700,000)
- `--content-threshold <value>`: Percentage of maximum characters to reserve for content (default: 0.9)
- `--show-merge-content`: Show file content for merge commits (default: false)

### Examples

Extract all commits by John Doe:
```
gittor C:\Projects\MyRepo C:\Output "John Doe"
```

Extract all commits by any Gmail user:
```
gittor C:\Projects\MyRepo C:\Output "*@gmail.com"
```

## Output Format

The generated markdown files follow this structure:

```markdown
# Git History (Part 1)

Period: 2024-01-01 to 2024-03-31 Total commits in this file: 248

## 1a2b3c4d (2024-03-31)

Feature: Add new Excel ribbon customization

### Added:

#### SomeProject/ExcelRibbon.cs

```csharp
public class ExcelRibbon : Office.IRibbonExtensibility
{
    // Implementation details...
}
```

### Modified/Renamed:

#### SomeProject/ThisAddIn.cs

```diff
+ using Microsoft.Office.Core;
- using Microsoft.Office.Interop.Excel;
  
  public partial class ThisAddIn
  {
+     private ExcelRibbon ribbon;
-     private bool initialized;
```
```

## License

This project is licensed under the MIT License - see the LICENSE file for details. 