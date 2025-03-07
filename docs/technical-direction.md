# Technical Direction: Gittor - Git History Extractor

## Architecture Context

- **Components**
    - CLI Interface: Processes user arguments
    - Repository Analyzer: Extracts and filters Git commits
    - Content Processor: Handles file content with language formatting
    - Output Generator: Creates markdown files with splitting logic
- **Data Flow**
    1. User input → Git commit history retrieval
    2. Author filtering → Content filtering
    3. File content extraction → Markdown formatting
    4. Size threshold checking → File splitting
    5. Output generation

## Implementation Constraints

### Technology Stack

- C# (.NET 9) with C# 12 features
- Dependencies:
    - LibGit2Sharp for Git operations
    - System.CommandLine for CLI argument parsing
    - MarkDig for markdown generation
    - xUnit for unit testing

### Code Organization

Simple structure with namespace separation:
	- `Gittor`: Main library with all domain logic
	    - `Gittor.Core`: Essential domain concepts and interfaces
	    - `Gittor.Git`: Repository access and Git operations
	    - `Gittor.Formatting`: Language-specific formatting
	    - `Gittor.Output`: File generation and splitting logic
	- `Gittor.CLI`: Console application for user interaction

Benefits of this approach:
	- Reduced solution complexity
	- Simplified dependency management
	- Faster build times
	- Clear logical separation while maintaining cohesion


### Design Patterns

- Command pattern for CLI operations
- Strategy pattern for language-specific formatting
- Builder pattern for markdown generation
- Repository pattern for Git access

## Technical Boundaries

### Performance Requirements

- Two-phase processing:
    - Phase 1: Load filtered commit metadata by author
    - Phase 2: Process commit content with streaming approach
- Stream-based processing for all repository sizes
- Buffered writing with periodic flushing
- Content estimation to predict file size before processing

### Resource Limitations

- 700,000 character threshold per file (90% content, 10% formatting)
- Support for repositories of any size
- Linear execution time scaling with commit count

## Testing Approach

- Unit tests for core algorithms:
    - File splitting logic
    - Content threshold calculations
    - Language-specific formatting
    - Markdown generation
- Test coverage for boundary conditions:
    - Empty repositories
    - Large commits
    - Special characters in content
    - File splitting edge cases

## Design Decisions

### Git Access Method

- **Decision**: Use LibGit2Sharp
- **Rationale**: Better performance, type safety, exception handling
- **Implementation**: Repository wrapper with targeted queries

### File Filtering Implementation

- **Decision**: Pattern-based matching for file extensions and paths
- **Rationale**: Simple, flexible, and transparent approach to filtering
- **Implementation**:
    - Essential file patterns:
        - Source: `.cs`, `.vb`, `.xaml`
        - Config: `.csproj`, `.vbproj`, `app.config`
        - Docs: `README.md`, `CHANGELOG.md`
    - Global exclusions:
        - Binaries/executables: `.exe`, `.dll`, `.obj`, etc.
        - Generated code: auto-generated files
        - Build outputs: `bin/`, `obj/`, etc.
        - Source control metadata: `.git/`
        - Media assets: `.jpg`, `.png`, etc.
        - Office documents: `.docx`, `.xlsx`, etc.
        - Archives: `.zip`, `.rar`, etc.
    - Pattern matching using glob syntax for both inclusion and exclusion

### Merge Commit Handling

- **Decision**: Special processing for merge commits
- **Rationale**: Merge commit diffs are often noisy and less meaningful
- **Implementation**:
    - Detect merge commits via LibGit2Sharp
    - Skip diff content generation
    - Include only commit message and affected files list
    - Format as special section with merge information

### Content Processing Strategy

- **Decision**: Two-pass approach for content estimation and file splitting
- **Rationale**: Prevents mid-commit splits while maintaining size limits
- **Implementation**:
    - Pass 1: Calculate content size estimates
    - Pass 2: Generate actual content with pre-planned splits

### File Splitting Algorithm

- **Decision**: Threshold-based predictive splitting
- **Implementation**:
    - Track running character count before writing
    - Pre-calculate content size before adding to file
    - For large commits:
        - Break only at logical boundaries (file sections)
        - Add continuation markers between parts
        - Start new files with clear commit continuation headers

### Unicode Handling Approach

- **Decision**: Hybrid approach with multiple fallbacks
- **Implementation**:
    - Primary: UTF-8 encoding for all file operations
    - Secondary: UTF-16 fallback for unsupported characters
    - Last resort: Replace unsupported characters with standard placeholder
    - Error handling:
        - C#: `// Content contains unsupported characters...`
        - VB: `' Content contains unsupported characters...`
        - XAML: `<!-- Content contains unsupported characters -->`
        - diff: `... Content contains unsupported characters ...`

### Error Handling Approach

- **Decision**: Simple exception bubbling
- **Implementation**:
    - Repository/path errors: Bubble up and exit
    - No author matches: Show 0 commits and exit
    - Commit errors: Report to console and continue
    - Report full exception message and stack trace
    - No custom exception types

### Processing Implementation

- **Decision**: Simple streaming approach
- **Implementation**:
    - Process commits sequentially
    - Write output as processing occurs
    - Use basic buffering for output files
    - Keep memory usage reasonable for large repositories

### Console Output Strategy

- **Decision**: Simple progress reporting
- **Implementation**:
    - Display total commit count
    - Report generated file information
    - Show summary statistics (files generated, commits processed)
    - Report errors with exception details
    - Match format from Feature Intent's Console Feedback section