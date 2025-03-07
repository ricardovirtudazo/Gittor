# Gittor Dev Container

This directory contains configuration for Visual Studio Code's Dev Containers feature, which allows you to develop inside a Docker container with a fully configured .NET 9.0 environment.

## Quick Setup (2 Steps)

### 1. Create your personal configuration
```bash
# Copy the template file to create your local configuration
cp .devcontainer/devcontainer.template.json .devcontainer/devcontainer.json
```

### 2. Configure your local paths
Edit the `.devcontainer/devcontainer.json` file with your actual paths:

```json
"mounts": [
  "source=/Users/yourname/path/to/repository,target=/repo,type=bind,consistency=cached",
  "source=/Users/yourname/path/to/output,target=/output-root,type=bind,consistency=cached"
]
```

Then open or reload the project in VS Code and select "Reopen in Container" when prompted.

## Requirements

- Visual Studio Code
- Docker Desktop
- VS Code Remote - Containers extension

## Why This Approach?

- **Privacy**: Your local paths stay private and are never committed to Git
- **Simplicity**: No environment variables or complex setup needed
- **Consistency**: Clear documentation for all team members
- **Flexibility**: Each developer can use their own paths

## Path Examples

- For macOS/Linux:
  ```json
  "mounts": [
    "source=/Users/yourname/repos/GraphicSchedule,target=/repo,type=bind,consistency=cached",
    "source=/Users/yourname/output,target=/output-root,type=bind,consistency=cached"
  ]
  ```

- For Windows (use forward slashes):
  ```json
  "mounts": [
    "source=C:/Users/yourname/repos/GraphicSchedule,target=/repo,type=bind,consistency=cached",
    "source=C:/Users/yourname/output,target=/output-root,type=bind,consistency=cached"
  ]
  ```

## Container Directory Structure

The container mounts local directories at:
- Your repository path → Container `/repo`
- Your output path → Container `/output-root`
- Debug output will be saved to → Container `/output-root/debug-output`

## Debugging

1. Set breakpoints in your code
2. Press F5 or select the Debug icon in the Activity Bar
3. Choose either "Debug Gittor.CLI" or "Debug Gittor.Tests" from the dropdown
4. Debug output will be saved to `/output-root/debug-output`

## Features Included

- .NET 9.0 SDK
- Git
- C# extension for VS Code
- C# Dev Kit
- CSharpier code formatter
- GitHub Copilot (if you have a license)
- Code Spell Checker

## Troubleshooting

If you encounter issues:

1. **Rebuild the container**: F1 > "Remote-Containers: Rebuild Container"
2. **Check Docker logs**: Docker Desktop > Containers > [container-name] > Logs
3. **Docker permissions**:
   - On macOS: Docker Desktop > Settings > Resources > File Sharing
   - On Windows: Docker Desktop > Settings > Resources > File Access
4. **Path issues**:
   - Verify your paths in `devcontainer.json` are correct
   - Ensure the directories actually exist on your machine 