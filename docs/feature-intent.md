# Feature Intent: Gittor - Git History Extractor

## Problem Definition

- Manual Git history extraction is time-consuming with non-essential modifications
- Raw repository history is too verbose for AI analysis due to token limitations
- Difficult to recall past project contributions for portfolio building

## User Stories

1. As a developer, I want to extract meaningful Git history into structured markdown files.
    
    - Acceptance Criteria:
        - CLI tool accepts repo path, output directory, author pattern
        - Filters non-essential files (binaries, generated code, assets)
        - Organizes output with proper syntax highlighting
        - Respects AI token limits with file splitting
    - Example: Repository with mixed content → Markdown files with only relevant code changes
    - CLI Usage: `gittor <repo-path> <output-dir> <author-pattern> [options]`
        
        ```
        Options:-h, --help      Show help message
        ```
        
2. As a job seeker, I want to create professional case studies from Git contributions.
    
    - Acceptance Criteria:
        - Chronological history with clear commit information
        - Includes only meaningful code changes
        - Properly formatted content for readability
    - Example: 6-month project history → Clean markdown showing feature development
3. As an AI user, I want repository history within token limitations.
    
    - Acceptance Criteria:
        - Files within 700,000 character limit
        - Intelligent file splitting at logical boundaries
        - Proper content truncation when necessary
    - Example: 3500+ commit repository → Multiple files under token limit

## Content Processing

### Essential Files

- Source: `.cs`, `.vb`, `.xaml`
- Config: `.csproj`, `.vbproj`, `app.config`
- Docs: `README.md`, `CHANGELOG.md`

### Excluded Content

- Binary/executables
- Generated code
- Build outputs
- Source control metadata
- Media assets
- Documents, Excel or data files
- Archives
- Git files

## Output Format

### Document Structure

- **H1**: File Information
    
    - Title: Git History (Part {number})
    - Period: {start_date} to {end_date}
    - Commit count: Total commits in file
- **H2**: Commit Details
    
    - Format: {hash} ({date})
    - Message: Title + Description (if present)
- **H3**: Change Category
    
    - Added
    - Deleted
    - Modified/Renamed
    - Other Types
- **H4**: File Details
    
    - Path (current)
    - Old path (if renamed)
    - Content with appropriate syntax highlighting

### File Organization

- Maximum file size: 700,000 characters
- Content threshold: 90% (630,000 characters) reserved for content
- Filename pattern: `git-history-{start_date}-to-{end_date}-part{sequence}.md`

### File Splitting Rules

- Start new file when next commit would exceed limit
- Single large commits:
    - Continue content in new file
    - Header format: `## {commit_hash} ({date}) Continued...`
    - Sequence starts at 2 (e.g., part2, part3)
- Only split at logical break points (between commits or file sections)

### Content Truncation

- File Snapshots (Added/Deleted):
    - Language-specific truncation markers:
        - C#: `// Content truncated...`
        - VB: `' Content truncated...`
        - XAML: `<!-- Content truncated -->`
- Diffs (Modified/Renamed):
    - Truncation marker: `... additional changes truncated ...`

### Special Commits

- Merge Commits:
    - Skip diff content
    - Show only commit message and affected files list

### Example Output

See the separate file `sample-output.md` for a complete example of the expected markdown output format, including code blocks with proper syntax highlighting and diff formatting.

## Console Feedback

### Progress Indicators

- Commit processing count
- File generated status
- Summary statistics:
    - Total files generated
    - Total commits processed
    - Status

### Example Console Output

```
Total commits: 3559
Generated: 2017-04-21 to 2017-04-24 (5 commits)
Summary:
- Total files generated: 61
- Total commits processed: 3559
Done!
```

## Edge Cases

1. Very large commits exceeding file limit
    
    - Behavior: Split across files with continuation markers
    - Example: 1M character commit split between Part 1 and Part 2
2. Non-standard encodings
    
    - Behavior: UTF-8 encoding, fallback to UTF-16 with surrogate pairs
    - Example: Repository with Japanese characters
3. Corrupt commits
    
    - Behavior: Log error, continue processing remaining commits
    - Example: Skip corrupted commit while processing others

## Project Type Support

- Excel VSTO/PIA Add-ins (VB.NET, C#)
- Windows Desktop Apps: WPF, WinUI 3, WinForms

## Success Definition

### Success Indicators

- Time efficiency: Noticeably faster than manual extraction process
- Filtering quality: Non-essential files consistently excluded
- Readability: Structured format with proper syntax highlighting
- Reliability: Successfully processes repository history with minimal errors

### Completion Criteria

- CLI processes specified project types
- Structured output with proper sections
- Files within 700,000 character limit
- Error handling: Exceptions bubble up with console reporting and exit (except for corrupt commits, which are reported to console and skipped)