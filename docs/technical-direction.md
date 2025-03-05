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

Domain-driven design with distinct boundaries:
- `Gittor.Core`: Domain models and core logic
- `Gittor.Git`: Repository access
- `Gittor.Formatting`: Language-specific formatting
- `Gittor.Output`: File generation and splitting

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

### Content Processing Strategy

- **Decision**: Two-pass approach for content estimation and file splitting
- **Rationale**: Prevents mid-commit splits while maintaining size limits

### File Splitting Algorithm

- **Decision**: Threshold-based predictive splitting
- **Implementation**:
    - Track running character count
    - Handle large commits across multiple files with continuation headers
    - Split at logical boundaries within large commits

### Error Handling Approach

- **Decision**: Exception bubbling with minimal handling
- **Implementation**:
    - Repository/path errors: Bubble up and exit
    - No author matches: Show 0 commits and exit
    - Commit errors: Console reporting and continue