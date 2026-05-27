## AI Build Analysis - Commit cf58f44

**Branch:** main | **Status:** success | **Date:** 2026-05-27 16:53:32

---

### Build Summary
Build completed with status: **success**

### Notes
AI analysis could not be performed due to an API connectivity issue.
The build artifacts and test results should be reviewed manually.

### Available Data
```
--- MSBuild Compile Console Log ---
MSBuild version 17.14.40+3e7442088 for .NET Framework
Build started 5/27/2026 4:52:30 PM.

Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.sln" on node 1 (Rebuild target(s)).
ValidateSolutionConfiguration:
  Building solution configuration "Release|Any CPU".
Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.sln" (1) is building "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager\document-sharing-manager.csproj" (2) on node 1 (Rebuild target(s)).
CoreClean:
  Creating directory "obj\Release\".
Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager\document-sharing-manager.csproj" (2) is building "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\document-sharing-manager.Core.csproj" (3:2) on node 1 (Clean target(s)).
CoreClean:
  Creating directory "obj\Release\netstandard2.0\".
Done Building Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\document-sharing-manager.Core.csproj" (Clean target(s)).
PrepareForBuild:
  Creating directory "bin\Release\".
Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager\document-sharing-manager.csproj" (2) is building "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\document-sharing-manager.Core.csproj" (3:3) on node 1 (default targets).
PrepareForBuild:
  Creating directory "bin\Release\netstandard2.0\".
_GenerateSourceLinkFile:
  Updating Source Link file 'obj\Release\netstandard2.0\document-sharing-manager.Core.sourcelink.json'.
CoreCompile:
  Setting DOTNET_ROOT to 'C:\Program Files\dotnet'
  C:\Program Files\dotnet\sdk\10.0.108\Roslyn\binfx\..\bincore\csc.exe /noconfig /sdkpath:C:\Windows\Microsoft.NET\Framework\v4.0.30319\ /unsafe- /checked- /nowarn:1701,1702,1591,1701,1702 /fullpaths /nostdlib+ /errorreport:prompt /doc:obj\Release\netstandard2.0\document-sharing-manager.Core.xml /define:TRACE;RELEASE;NETSTANDARD;NETSTANDARD2_0;NETSTANDARD1_0_OR_GREATER;NETSTANDARD1_1_OR_GREATER;NETSTANDARD1_2_OR_GREATER;NETSTANDARD1_3_OR_GREATER;NETSTANDARD1_4_OR_GREATER;NETSTANDARD1_5_OR_GREATER;NETSTANDARD1_6_OR_GREATER;NETSTANDARD2_0_OR_GREATER /highentropyva+ /nullable:enable /reference:C:\Users\runneradmin\.nuget\packages\microsoft.bcl.asyncinterfaces\8.0.0\lib\netstandard2.0\Microsoft.Bcl.AsyncInterfaces.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\Microsoft.Win32.Primitives.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\mscorlib.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll /reference:C:\Users\runneradmin\.nuget\packages\newtonsoft.json\13.0.4\lib\netstandard2.0\Newtonsoft.Json.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.AppContext.dll /reference:C:\Users\runneradmin\.nuget\packages\system.buffers\4.6.1\lib\netstandard2.0\System.Buffers.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Collections.Concurrent.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Collections.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Collections.NonGeneric.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.Collections.Specialized.dll /reference:C:\Users\runneradmin\.nuget\packages\system.componentmodel.annotations\5.0.0\ref\netstandard2.0\System.ComponentModel.Annotations.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.ComponentModel.Composition.dll /reference:C:\Users\runneradmin\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\System.ComponentModel.dll /reference:C:

... [TRUNCATED MIDDLE MSBUILD LOGS] ...

r-api\bin\Release\net8.0\document-sharing-manager.Infrastructure.dll".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\bin\Release\netstandard2.0\document-sharing-manager.Core.pdb" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager.Core.pdb".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\bin\Release\netstandard2.0\document-sharing-manager.Core.xml" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager.Core.xml".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Infrastructure\bin\Release\net8.0\document-sharing-manager.Infrastructure.pdb" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager.Infrastructure.pdb".
  Creating "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\document.D23CF265.Up2Date" because "AlwaysCreate" was specified.
  Touching "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\document.D23CF265.Up2Date".
_CopyOutOfDateSourceItemsToOutputDirectory:
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\appsettings.json" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\appsettings.json".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\staticwebassets.build.endpoints.json" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.staticwebassets.endpoints.json".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\apphost.exe" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.exe".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\appsettings.Development.json" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\appsettings.Development.json".
CopyFilesToOutputDirectory:
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\document-sharing-manager-api.dll" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.dll".
  Copying reference assembly from "obj\Release\net8.0\refint\document-sharing-manager-api.dll" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\ref\document-sharing-manager-api.dll".
  document-sharing-manager-api -> D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.dll
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\document-sharing-manager-api.pdb" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.pdb".
  Copying file from "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\obj\Release\net8.0\document-sharing-manager-api.xml" to "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.xml".
CleanupEmptyRefsFolder:
  Directory "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\bin\Release\net8.0\refs" doesn't exist. Skipping.
Done Building Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager-api\document-sharing-manager-api.csproj" (Rebuild target(s)).
Done Building Project "D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.sln" (Rebuild target(s)).

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:21.58


--- NUnit Test Console Log ---
  Determining projects to restore...
  All projects are up-to-date for restore.
  document-sharing-manager.Core -> D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Core\bin\Release\netstandard2.0\document-sharing-manager.Core.dll
  document-sharing-manager -> D:\a\Document-Sharing\Document-Sharing\document-sharing-manager\bin\Release\document-sharing-manager.exe
  document-sharing-manager.Tests -> D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Tests\bin\Release\net48\document-sharing-manager.Tests.dll
Test run for D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Tests\bin\Release\net48\document-sharing-manager.Tests.dll (.NETFramework,Version=v4.8)
A total of 1 test files matched the specified pattern.

[dotMemory Unit]: The test method is run without the dotMemory Unit support and 'dotMemory.Check' is ignored according to the settings.

Results File: D:\a\Document-Sharing\Document-Sharing\document-sharing-manager.Tests\TestResults\test_results.trx

Passed!  - Failed:     0, Passed:     3, Skipped:     0, Total:     3, Duration: 4 s - document-sharing-manager.Tests.dll (net48)


Test Outcome: Passed

Tests: Total=3 Passed=3 Failed=0

--- NuGet Audit ---
  Determining projects to restore...
  All projects are up-to-date for restore.

The following sources were used:
   https://api.nuget.org/v3/index.json
   C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\

The given project `document-sharing-manager.Tests` has no vulnerable packages given the current sources.


--- Capacity Report ---
### DORA & Capacity Governance Report
> Generated at 2026-05-27 16:53:23

#### DORA Metrics (Delivery Performance)
| Metric | Value | Rating | Status |
| :--- | :--- | :--- | :--- |
| **Lead Time for Changes** | 1.95 min | Elite | [STABLE] |
| **Deployment Frequency** | TBD | N/A | [PENDING] |
| **Change Failure Rate** | TBD | N/A | [PENDING] |

#### Capacity & FinOps
| Metric | Value | Limit | Status |
| :--- | :--- | :--- | :--- |
| **Installer Size** | 6.14 MB | 50 MB | [OPTIMAL] |
| **Repo Source Size** | 259.39 MB | 500 MB | [OPTIMAL] |

---
*Recommendation: Current build duration is within Elite/High threshold. Continue optimizing assets to maintain lead time.*


--- Security Audit ---
## Security Audit Report

**BUILD_STATUS: PASS** (with warnings)

The AI security analysis could not be completed due to an API error.
Manual review of the following audit log is recommended:

```
  Determining projects to restore...
  All projects are up-to-date for restore.

The following sources were used:
   https://api.nuget.org/v3/index.json
   C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\

The given project `document-sharing-manager.Tests` has no vulnerable packages given the current sources.

```

---
*AI Analysis: Failed | Fallback Report | 2026-05-27 16:52:29*

```

---
*Fallback Report (AI unavailable) | Commit: cf58f44*
