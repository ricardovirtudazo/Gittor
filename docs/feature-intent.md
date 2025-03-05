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