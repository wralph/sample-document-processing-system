# Next Steps

## Issues resolved
- Transformed DocumentProcessor.Web.csproj to net8.0

## Summary

The solution has no build errors following the transformation. All projects compiled successfully, including `DocumentProcessor.Web`.

## Validation Steps

### 1. Review Target Framework

Open each `.csproj` file and confirm the target framework is set to a supported cross-platform .NET version (e.g., `net8.0`):

```xml
<TargetFramework>net8.0</TargetFramework>
```

If any project still references `net48` or `netcoreapp3.1`, update it to a current and supported TFM.

### 2. Restore and Build Locally

Run the following commands from the solution root to confirm a clean restore and build:

```bash
dotnet restore
dotnet build
```

Ensure there are no warnings that could indicate compatibility issues, such as deprecated APIs or platform-specific assemblies.

### 3. Run the Test Suite

If the solution contains test projects, execute them to verify that behavior has not changed during transformation:

```bash
dotnet test
```

Review any failing tests and determine whether they reflect regressions introduced by the migration or pre-existing issues.

### 4. Check for Runtime Dependencies

Some libraries that compiled successfully may have runtime issues on non-Windows platforms. Pay attention to:

- Any usage of `System.Drawing` (replaced by alternatives like `SkiaSharp` or `ImageSharp` for cross-platform use)
- P/Invoke calls or native interop that may be Windows-specific
- Registry access (`Microsoft.Win32.Registry`)
- Windows-specific file path assumptions (backslashes, drive letters)

Run the application on the target platform (Linux or macOS if applicable) and observe runtime behavior.

### 5. Review NuGet Package Compatibility

Check that all NuGet packages referenced in the project files are compatible with the target framework. Use the following command to identify outdated or potentially incompatible packages:

```bash
dotnet list package --outdated
```

Update packages where newer versions provide better cross-platform support.

### 6. Run the Web Application

Start the `DocumentProcessor.Web` project and verify it runs correctly:

```bash
dotnet run --project src/DocumentProcessor.Web/DocumentProcessor.Web.csproj
```

Navigate to the application in a browser and exercise the primary workflows, particularly any document processing features, to confirm end-to-end functionality.

### 7. Review Configuration Files

Ensure `appsettings.json` and any environment-specific configuration files (`appsettings.Development.json`, etc.) are present and correctly configured for the new hosting model. Confirm that any configuration previously stored in `Web.config` has been migrated to the appropriate `appsettings.json` entries or middleware configuration in `Program.cs`.

### 8. Validate Static Files and Middleware

If the project serves static files or uses middleware previously configured via `Web.config` (e.g., custom headers, URL rewriting), verify that equivalent configuration exists in the ASP.NET Core middleware pipeline in `Program.cs` or `Startup.cs`.

### 9. Publish a Release Build

Once local validation is complete, produce a release build to confirm there are no issues specific to the release configuration:

```bash
dotnet publish src/DocumentProcessor.Web/DocumentProcessor.Web.csproj -c Release -o ./publish
```

Review the output directory to confirm all expected files are present, including views, static assets, and configuration files.