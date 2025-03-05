# Feature Brief: Gittor - Git History Extractor

## Executive Summary

Gittor extracts meaningful Git repository history into structured markdown files, optimized for human and AI analysis.

## Problem Statement

- Manual Git history extraction is time-consuming and includes non-essential modifications
- Raw repository history is too verbose for effective AI analysis due to token limitations
- Difficult to accurately recall past project contributions when building portfolios

## Proposed Solution

- CLI tool that filters meaningful code changes into markdown files
- Memory-efficient processing with commit-by-commit analysis and threshold-based file generation
- Intelligent file splitting to manage AI token limits

### Key Capabilities

- Supports Excel VSTO/PIA Add-ins and Windows Desktop Apps (WPF, WinUI 3, WinForms)
- Splits history into multiple files based on 700,000 character threshold
- Organizes changes by category with language-specific formatting
- Excludes binary files, generated code, and other non-essential content

## Business Value

- Provides structured, readable Git history format
- Saves time compared to manual extraction
- Creates output optimized for both human and AI consumption
- Maintains files within AI token limits

## Success Metrics

- Time saved vs. manual extraction
- Filtering efficiency (% of non-essential files excluded)
- Output readability for human/AI analysis
- Processing completeness (% of history successfully processed)

## Resources & Timeline

- Effort: Initial implementation within 1-2 days using Cursor AI
- Dependencies: Git CLI, repository access, storage space
- Target delivery: Initial version within two days
- Error handling: Exception bubbling with console reporting

## Key Visual

````
# Git History (Part 1)
Period: 2024-01-01 to 2024-03-31
Total commits in this file: 248

## 1a2b3c4d (2024-03-31)
Feature: Add new Excel ribbon customization

### Added: 
#### SomeProject/ExcelRibbon.cs
```csharp
public class ExcelRibbon : Office.IRibbonExtensibility
{
    public string GetCustomUI(string ribbonId)
    {
        // Implementation details...
    }
}
````