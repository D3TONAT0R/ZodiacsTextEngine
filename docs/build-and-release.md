# ZodiacsTextEngine Build and Release

This repository includes a GitHub Actions workflow that automatically builds and creates releases for the ZodiacsTextEngine project.

## Automated Release Process

The workflow (`/.github/workflows/build-and-release.yml`) is triggered in two ways:

1. **Automatic Release**: When a version tag (starting with 'v') is pushed to the repository
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. **Manual Release**: By manually triggering the workflow from the GitHub Actions tab

## What Gets Built and Released

The workflow performs the following steps:

1. **Builds ZodiacsTextEngine Library**: Compiles the core .NET Standard 2.0 library
2. **Creates Library Zip**: Packages the library DLL and dependencies into `ZodiacsTextEngine-{version}.zip`
3. **Builds ZodiacsTextConsole**: Compiles the console application
4. **Publishes All Profiles**: Runs all five publish profiles:
   - `portable` - Framework-dependent, cross-platform
   - `win-x64` - Windows 64-bit
   - `osx-x64` - macOS Intel 64-bit  
   - `osx-arm64` - macOS Apple Silicon
   - `linux-x64` - Linux 64-bit

5. **Creates GitHub Release**: Creates a new release with all zip files attached

## Release Assets

Each release will include these files:
- `ZodiacsTextEngine-{version}.zip` - The core library for developers
- `ZodiacsTextConsole-{version}-portable.zip` - Cross-platform console application 
- `ZodiacsTextConsole-{version}-win-x64.zip` - Windows 64-bit standalone application
- `ZodiacsTextConsole-{version}-osx-x64.zip` - macOS Intel standalone application
- `ZodiacsTextConsole-{version}-osx-arm64.zip` - macOS Apple Silicon standalone application
- `ZodiacsTextConsole-{version}-linux-x64.zip` - Linux 64-bit standalone application

Where `{version}` is the release tag (e.g., `v1.1.0`, `1.1.0`).

## Requirements

- The workflow runs on Windows to maintain compatibility with existing MSBuild targets
- Requires .NET 8.0 SDK
- Uses PowerShell for compression and file operations