# Next Steps

## Issues resolved
- Transformed DocumentProcessor.Web.csproj to net8.0

## Summary

The solution has no build errors following the transformation. All projects compiled successfully, which indicates the migration to cross-platform .NET has completed without introducing any build-level issues.

## Validation Steps

### 1. Review the Target Framework

Open `src/DocumentProcessor.Web/DocumentProcessor.Web.csproj` and confirm the `<TargetFramework>` element is set to the intended .NET version (e.g., `net8.0`). Ensure this is consistent with any referenced class libraries in the solution.

### 2. Restore and Build from the Command Line

Run the following commands from the solution root to confirm the build is clean outside of any IDE context:

```bash
dotnet restore
dotnet build --configuration Release
```

Review the output for any warnings that may indicate deprecated APIs or compatibility concerns, even if they do not cause build failures.

### 3. Run the Test Suite

If the solution contains test projects, execute them to verify runtime behavior has not regressed:

```bash
dotnet test --configuration Release
```

Review any failing tests carefully, as they may point to behavioral differences between the legacy .NET Framework APIs and their cross-platform equivalents.

### 4. Check for Runtime-Only Issues

Some incompatibilities do not surface at build time. Pay particular attention to the following areas at runtime:

- **File system paths**: Ensure no hardcoded Windows-style paths (e.g., `C:\`) exist in configuration files or code.
- **Registry access**: Any use of `Microsoft.Win32.Registry` will fail on non-Windows platforms.
- **Windows-specific APIs**: Review usage of APIs such as `System.Drawing`, `System.Security.Principal.WindowsIdentity`, or COM interop, which may not function correctly on Linux or macOS.
- **Configuration**: Verify that `appsettings.json` and any environment-specific configuration files are present and correctly structured.

### 5. Test the Web Application Locally

Start the application using the .NET CLI and confirm it responds as expected:

```bash
dotnet run --project src/DocumentProcessor.Web/DocumentProcessor.Web.csproj
```

Navigate to the URL printed in the console output and exercise the primary application workflows, including any document processing functionality.

### 6. Review NuGet Package Compatibility

Check that all NuGet packages referenced in the project support the target framework. Packages that were designed for .NET Framework may have limited or no support on .NET 6/7/8. Use the following command to identify outdated packages:

```bash
dotnet list package --outdated
```

Update packages where newer, cross-platform compatible versions are available.

### 7. Validate Static Assets and Views

If the web project uses Razor views or static files, confirm that:

- All view files render without runtime errors.
- Static assets (CSS, JavaScript, images) are served correctly.
- Any bundling or minification tooling is compatible with the current setup.

### 8. Deployment

Once local validation is complete, publish the application using:

```bash
dotnet publish --configuration Release --output ./publish
```

Review the contents of the `./publish` directory and deploy it to the target environment according to your existing hosting setup (e.g., IIS, Kestrel behind a reverse proxy, or a Linux host).

On IIS, ensure the ASP.NET Core Hosting Bundle is installed and the application pool is set to **No Managed Code**.