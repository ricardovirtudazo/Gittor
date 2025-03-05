# Git Commit Message Generation Instructions

## Structure
```
<type>(<scope>): <subject>

<body>

<footer>
```

## Core Rules
1. Subject: Under 50 characters, imperative mood ("Add" not "Added")
2. Body: Explain WHAT changed and WHY it was necessary
3. One logical change per commit

## Subject Line
1. Type: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, or `chore`
2. Scope: Optional, in parentheses (`feat(auth):`)
3. No period at end

## Body
1. Blank line after subject
2. Write continuous paragraphs without artificial line breaks
3. State motivation clearly and concisely
4. Use bullet points for multiple changes
5. Keep lists as part of body with no blank lines before or within list sections

## Footer
1. Blank line before footer
2. Reference issues: `Fixes #123`
3. Mark breaking changes: `BREAKING CHANGE: description`

## Examples

✅ **Good**:
```
feat(auth): implement JWT authentication

Replace cookies with JWT tokens to address scaling issues with current auth system. Benefits:
- Better API scalability for market expansion
- Reduced server memory usage
- Mobile client compatibility

Resolves: #123
```

✅ **Good body with list**:
```
docs: add commit message generation instructions

Create guidelines for generating effective commit messages to ensure clarity and consistency in the repository. This will help contributors understand the structure and importance of well-crafted messages.
Benefits:
- Provides a clear framework for writing commit messages
- Enhances collaboration and code review processes
- Reduces ambiguity in commit history
```

❌ **Avoid**:
```
made some changes to the login stuff
```

❌ **Missing WHY**:
```
fix(auth): update login form validation

Add email format checking and password strength meter.
```

❌ **Avoid empty lines before lists**:
```
docs: add commit message generation instructions

Create guidelines for generating effective commit messages to ensure clarity and consistency in the repository. This will help contributors understand the structure and importance of well-crafted messages.

Benefits:
- Provides a clear framework for writing commit messages
- Enhances collaboration and code review processes
- Reduces ambiguity in commit history
```