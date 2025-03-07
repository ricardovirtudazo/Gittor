# Gittor Dev Container

This directory contains configuration for Visual Studio Code's Dev Containers feature, which allows you to develop inside a Docker container with a fully configured .NET 9.0 environment.

## Requirements

1. Visual Studio Code
2. Docker Desktop
3. Visual Studio Code Remote - Containers extension

## Getting Started

1. Install Docker Desktop from [https://www.docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop)
2. Install the VS Code Remote - Containers extension from [https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
3. Open this project in VS Code
4. When prompted "Folder contains a Dev Container configuration file. Reopen folder to develop in a container", click "Reopen in Container"
   - Alternatively, press F1, type "Remote-Containers: Reopen in Container"

## Debugging

1. Set breakpoints in your code
2. Press F5 or select the Debug icon in the Activity Bar and click the green arrow
3. Choose either "Debug Gittor.CLI" or "Debug Gittor.Tests" from the configuration dropdown

## Features Included

- .NET 9.0 SDK
- Git
- C# extension for VS Code
- C# Dev Kit
- CSharpier code formatter
- GitHub Copilot (if you have a license)
- Code Spell Checker

## Container Commands

- The container automatically runs `dotnet restore` when it starts up
- Use the integrated terminal in VS Code to run additional commands

## Troubleshooting

If you encounter issues:

1. Rebuild the container: F1 > "Remote-Containers: Rebuild Container"
2. Check Docker logs: Docker Desktop > Containers > [container-name] > Logs
3. Ensure your Docker Desktop has sufficient resources allocated 