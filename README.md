<h1>How to publish a project for Windows and Linux, using a single restore and build operation.</h1>
<h2>The project and how it is configured.</h2>

Let's consider a simple .NET 8 console application that generates a self-signed certificate and writes the public and private keys to standard output in PEM format.

The application is cross-platform, it utilizes the, it utulizes [System.Security.Cryptography.ProtectedData](https://www.nuget.org/packages/System.Security.Cryptogr

Let's look at couple of important parts of ``.csproj`` file.

I've explicitly set ``AppendTargetFrameworkToOutputPath`` and ``AppendRuntimeIdentifierToOutputPath`` to ``true`` to mirror build output structure.
I've also explicitly specified multiple runtime indentifiers reflecting that I'd like my app to work on both runtimes - ``win-x64`` and ``linux-x64``.
```xml
<OutputPath>$(Platform)\$(Configuration)</OutputPath>
<AppendTargetFrameworkToOutputPath>true</AppendTargetFrameworkToOutputPath>
<AppendRuntimeIdentifierToOutputPath>true</AppendRuntimeIdentifierToOutputPath>
<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>
```
Build output folder is ``./GenerateSelfSignedCertificate/x64/Debug/net8.0/``.
Obj folder location reflects this settnigs too ``./GenerateSelfSignedCertificate\obj\x64\Debug\net8.0``
Now, it is possible to build and run the application on the same platform — whether ``win-64`` or ``linux-x64`` — from a single folder - ``./GenerateSelfSignedCertificate/x64/Debug/net8.0/``

The runtimes folder is located in the build output directory, and the universal ``System.Security.Cryptography.ProtectedData.dll`` is placed in the root of the build output folder.
```sh
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
GenerateSelfSignedCertificate            GenerateSelfSignedCertificate.dll  GenerateSelfSignedCertificate.runtimeconfig.json  runtimes
GenerateSelfSignedCertificate.deps.json  GenerateSelfSignedCertificate.pdb  System.Security.Cryptography.ProtectedData.dll
```

There're 3 options how you can specify RID for you project:
1. no RID in csproj
2. single RID - ``<RuntimeIdentifier>win-x64</RuntimeIdentifier>``
3. multiple RID - ``<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>``
Honestly you don't need to specify RID in multuple form. It can be avoided cause option 1. and 3. are equivalent.
When no RID is specified or multiple RID specified .net pulls all available runtimes for you project and copies them into single build output folder.
*.deps.json file is organized correspondingly.

<details>
<summary>GenerateSelfSignedCertificate.deps.json for multiple RIDs</summary>

```json
{
  "runtimeTarget": {
    "name": ".NETCoreApp,Version=v8.0",
    "signature": ""
  },
  "compilationOptions": {},
  "targets": {
    ".NETCoreApp,Version=v8.0": {
      "GenerateSelfSignedCertificate/1.0.0": {
        "dependencies": {
          "System.Security.Cryptography.ProtectedData": "6.0.0"
        },
        "runtime": {
          "GenerateSelfSignedCertificate.dll": {}
        }
      },
      "System.Security.Cryptography.ProtectedData/6.0.0": {
        "runtime": {
          "lib/net6.0/System.Security.Cryptography.ProtectedData.dll": {
            "assemblyVersion": "6.0.0.0",
            "fileVersion": "6.0.21.52210"
          }
        },
        "runtimeTargets": {
          "runtimes/win/lib/net6.0/System.Security.Cryptography.ProtectedData.dll": {
            "rid": "win",
            "assetType": "runtime",
            "assemblyVersion": "6.0.0.0",
            "fileVersion": "6.0.21.52210"
          }
        }
      }
    }
  },
  "libraries": {
    "GenerateSelfSignedCertificate/1.0.0": {
      "type": "project",
      "serviceable": false,
      "sha512": ""
    },
    "System.Security.Cryptography.ProtectedData/6.0.0": {
      "type": "package",
      "serviceable": true,
      "sha512": "sha512-rp1gMNEZpvx9vP0JW0oHLxlf8oSiQgtno77Y4PLUBjSiDYoD77Y8uXHr1Ea5XG4/pIKhqAdxZ8v8OTUtqo9PeQ==",
      "path": "system.security.cryptography.protecteddata/6.0.0",
      "hashPath": "system.security.cryptography.protecteddata.6.0.0.nupkg.sha512"
    }
  }
}
```
</details>

However if you specify single RuntimeIdentifier the output structure and deps.json file will be different.

```sh
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
linux-x64

ls .\GenerateSelfSignedCertificate\x64\Debug\net8.0\linux-x64\

    Directory: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\linux-x64

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---          12/23/2024 10:59 AM          72520 GenerateSelfSignedCertificate
-a---          12/23/2024 10:59 AM           1304 GenerateSelfSignedCertificate.deps.json
-a---          12/23/2024 10:59 AM           5632 GenerateSelfSignedCertificate.dll
-a---          12/23/2024 10:59 AM          11732 GenerateSelfSignedCertificate.pdb
-a---          12/23/2024 10:59 AM            268 GenerateSelfSignedCertificate.runtimeconfig.json
-a---          10/23/2021  1:51 AM          20592 System.Security.Cryptography.ProtectedData.dll
```

<details>
<summary>GenerateSelfSignedCertificate.deps.json for single RID</summary>

```json
{
  "runtimeTarget": {
    "name": ".NETCoreApp,Version=v8.0/linux-x64",
    "signature": ""
  },
  "compilationOptions": {},
  "targets": {
    ".NETCoreApp,Version=v8.0": {},
    ".NETCoreApp,Version=v8.0/linux-x64": {
      "GenerateSelfSignedCertificate/1.0.0": {
        "dependencies": {
          "System.Security.Cryptography.ProtectedData": "6.0.0"
        },
        "runtime": {
          "GenerateSelfSignedCertificate.dll": {}
        }
      },
      "System.Security.Cryptography.ProtectedData/6.0.0": {
        "runtime": {
          "lib/net6.0/System.Security.Cryptography.ProtectedData.dll": {
            "assemblyVersion": "6.0.0.0",
            "fileVersion": "6.0.21.52210"
          }
        }
      }
    }
  },
  "libraries": {
    "GenerateSelfSignedCertificate/1.0.0": {
      "type": "project",
      "serviceable": false,
      "sha512": ""
    },
    "System.Security.Cryptography.ProtectedData/6.0.0": {
      "type": "package",
      "serviceable": true,
      "sha512": "sha512-rp1gMNEZpvx9vP0JW0oHLxlf8oSiQgtno77Y4PLUBjSiDYoD77Y8uXHr1Ea5XG4/pIKhqAdxZ8v8OTUtqo9PeQ==",
      "path": "system.security.cryptography.protecteddata/6.0.0",
      "hashPath": "system.security.cryptography.protecteddata.6.0.0.nupkg.sha512"
    }
  }
}
```
</details>

We can omit the details about the deps.json file and its content since all of our application's binaries are in the same folder.
>A runtime configuration file is not required to successfully launch an application, but without it, all the dependent assemblies must be located within the same folder as the application.

More details about assembly resolution [here](https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md) and [here](https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md#assembly-resolution).

I primarly use windows so i just build the app and run it. It works from ``/x64/Debug/net8.0`` folder even without publish.
And I can just switch from windows to linux and do the same - just build and run and it works without publish.

<h2>Build and publish</h2>

Our goal is to obtain binaries for both Windows and Linux.
I have added two publish profiles, one for each RID.

```sh
tree /f .\GenerateSelfSignedCertificate 
Folder PATH listing for volume Windows
Volume serial number is CE37-B548
C:\USERS\MBRYKSIN\DESKTOP\LINKEDIN\PUBLISH\PROJECT\NET_PUBLISH\GENERATESELFSIGNEDCERTIFICATE
│   GenerateSelfSignedCertificate.csproj
│   GenerateSelfSignedCertificate.csproj.user
│   Program.cs
│
└───Properties
    └───PublishProfiles
            FolderProfile_linux.pubxml
            FolderProfile_windows.pubxml
```

Publishing helps us generate only the necessary binaries for the target runtime.

The usual or proper workflow to prepare a ready-to-use application involves restoring NuGet packages, building binaries, and then publishing them.
If we need our application to work on multiple platforms, we must perform all these steps twice.

This is the most straightforward approach - if the process works for one target, we repeat the same steps for every other target.
This approach is correct and ensures everything functions properly.
It's even the default behavior when building and publishing using the Visual Studio UI.

These are the commands used for succeful publising the application for windows and linux.

```sh
dotnet restore  .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
dotnet build .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
dotnet publish .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -p:publishprofile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml -c Debug -v
dotnet publish .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -p:publishprofile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml -c Debug -v n
```

Howerver, if you go through the MsBuild detaild output you would notice that ``Restore`` and ``CoreCompile`` targets are called 3 times - one for dotnet build, one for dotnet publish windows and one more for dotnet publish linux.
There are two questions we should ask ourselves:
1. Why do we need to build our application more than once if the code is cross-platform?
2. Why do we need to restore NuGet packages more than once if .NET consolidates all binaries and the runtimes folder into a single build output folder during builds for multiple RIDs? (Assuming that Microsoft has allowed such behavior, it should be supported)

MsBuild is smart enough and if e.g. you run dotnet build 2 times in row multiple targets if not all of them will be skipped.
Below is the output for the second build in a row for out project.
```sh
dotnet build .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj --no-restore  -v n
Build started 12/24/2024 3:45:22 PM.
     1>Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (default targets).
     1>GenerateTargetFrameworkMonikerAttribute:
       Skipping target "GenerateTargetFrameworkMonikerAttribute" because all output files are up-to-date with respect to the input files.
       CoreGenerateAssemblyInfo:
       Skipping target "CoreGenerateAssemblyInfo" because all output files are up-to-date with respect to the input files.
       _GenerateSourceLinkFile:
         Source Link file 'obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.sourcelink.json' is up-to-date.
       CoreCompile:
       Skipping target "CoreCompile" because all output files are up-to-date with respect to the input files.
       _CreateAppHost:
       Skipping target "_CreateAppHost" because all output files are up-to-date with respect to the input files.
       _CopyOutOfDateSourceItemsToOutputDirectory:
       Skipping target "_CopyOutOfDateSourceItemsToOutputDirectory" because all output files are up-to-date with respect to the input files.
       GenerateBuildDependencyFile:
       Skipping target "GenerateBuildDependencyFile" because all output files are up-to-date with respect to the input files.
       GenerateBuildRuntimeConfigurationFiles:
       Skipping target "GenerateBuildRuntimeConfigurationFiles" because all output files are up-to-date with respect to the input files.
       CopyFilesToOutputDirectory:
         GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.dll
     1>Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (default targets).

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.56
```
For some reason for 2 different publishing ``Restore`` and ``CoreCompile`` targrets  are called twice - one time per publish profile.
In total ``Restore`` and ``CoreCompile`` targets are called 3 times - build\publish windoes\publish linux.

Let's look at dotnet build and dotnet publish cli documentation.
```
dotnet build [--no-restore]
dotnet publish [--no-build] [--no-restore]
```
[dotnet-publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)
[dotnet-build](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build)

The documentation seems to support our assumption. Microsoft allows building without restoring, and publishing without building and/or restoring.
How to publish w/out exta restore and w/out extra build ? Let's try dotnet publish first.

```sh
dotnet publish --no-restore --no-build .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -p:publishprofile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml -c Debug -v n
Build FAILED.

C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug
       \net8.0\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSigne
       dCertificate\GenerateSelfSignedCertificate.csproj]
```

We need to assist MSBuild by specifying the output directory for the build binaries because it uses the wrong one, as indicated by the error message.
The problem is that dotnet publish simply does not support specifying an output directory.
It is possible to specify ``[-o|--output <OUTPUT_DIRECTORY>]`` for dotnet publish but it is not what we want.
This is merely the target location for our published binaries, whereas we need to assist MSBuild with specifying the source folder that contains the build binaries.
[dotnet publish does not set OutDir - issue](https://github.com/dotnet/sdk/issues/9012)
So let's switch to MSBuild which allow that and much more.
MSBuild allows publishing without build and restore as well -  ``/p:RestorePackages=false /p:NoBuild=true``

```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug

C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug\net8.0 
\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\G 
enerateSelfSignedCertificate.csproj]
```

That still does not work. OutDir value is wrong.

```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug /p:AppendRuntimeIdentifierToOutputPath=false

C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug\net8.0 
\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\G 
enerateSelfSignedCertificate.csproj]
```
Again it does not work, OutDir is still wrong.

Let's make one more attempt and correct OutDir looking at the error message.
```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n

MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/9/2024 11:15:27 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (publish targ
et(s)).
_CopyResolvedFilesToPublishPreserveNewest:
Skipping target "_CopyResolvedFilesToPublishPreserveNewest" because all output files are up-to-date with respect to the input files.
_CopyResolvedFilesToPublishAlways:
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\apphost.exe" to "C:\Users\mb
  ryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.exe".
Publish:
  GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish 
target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.52
```

Perfect now it works!
As you can see, the CoreCompile target was not called; the compilation was skipped. We’ve just published the app without restoring or building for Windows. (Or to put it differently, we restored NuGet packages and then built the app only once during the build itself, not during the publish process.)

Let's run publish for linux.
```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n

Build FAILED.

"C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish target) (1) ->
(_CopyResolvedFilesToPublishAlways target) ->
  C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(380,5): error MSB3030: Could not copy the file "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\apphost" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCer 
tificate.csproj]

    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:00.77
```

We encountered an issue with apphost. In .NET Core 3.0 and later, when you publish an application, an executable file (apphost) is created by default. This feature provides a platform-specific binary that allows you to run your application without needing to specify dotnet and the DLL name, simplifying the launch process.

If you prefer to disable the creation of the apphost and follow the traditional approach of running your application using the dotnet command with your DLL, you can adjust your project file (*.csproj) to do so. To disable apphost for your project, add <UseAppHost>false</UseAppHost> to the .csproj file.

By doing this, the apphost is not created for your executable, meaning you'll need to run the executable using dotnet, which might not be convenient or even acceptable for certain scenarios.

Let's disable apphost creation and run the previous command once again. Publishing for Linux now works as expected.

```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n  
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 3:38:01 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (publish target(s)).
_CopyResolvedFilesToPublishPreserveNewest:
Building target "_CopyResolvedFilesToPublishPreserveNewest" partially, because some output files are out of date with respect to their input files.
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.runtimeconfig.json" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.runtimeconfig.json".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.pdb" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.pdb".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.dll" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.dll".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.deps.json" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.deps.json".
Publish:
  GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.59
```

However, the absence of apphost is not our desired solution. Let's delve a bit deeper. We have just demonstrated that apphost is the key obstacle preventing us from publishing for two different platforms using a single build and restore process. To address the apphost problem, we need to analyze the MSBuild output or the [binary log](https://learn.microsoft.com/en-us/visualstudio/msbuild/obtaining-build-logs-with-msbuild?view=vs-2022#save-a-binary-log)([msbuild binary log viewer](https://msbuildlog.com/)).
My teammate @Martin_Balous were succefully reserached the problem and found out the msbuild target responsible for apphost creation. They are ``ResolveFrameworkReferences`` and ``_CreateAppHos``.
This what @Martin wrote to me:

>after long time of reverse engineering the binary logs...
![screenshot](./imageFolder/apphost_investigation.png)

The whole command is below.

```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /p:Configuration=Debug /t:ResolveFrameworkReferences;_CreateAppHost /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 4:38:21 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (ResolveFrameworkRe
ferences;_CreateAppHost target(s)).
GenerateTargetFrameworkMonikerAttribute:
Skipping target "GenerateTargetFrameworkMonikerAttribute" because all output files are up-to-date with respect to the input files.
CoreGenerateAssemblyInfo:
Skipping target "CoreGenerateAssemblyInfo" because all output files are up-to-date with respect to the input files.
_GenerateSourceLinkFile:
  Source Link file 'obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.sourcelink.json' is up-to-date.
CoreCompile:
Skipping target "CoreCompile" because all output files are up-to-date with respect to the input files.
_CreateAppHost:
Skipping target "_CreateAppHost" because all output files are up-to-date with respect to the input files.
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (ResolveFramewo 
rkReferences;_CreateAppHost target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.60
```
Afther executing this command publish to linux works as expected.


<h2>Final commands:</h2>


Now it's time to go from the bedinning to the end and publish the application for windows and for linux withot rebuild it.

1) Remove folders
```sh
Remove-Item -Path .\GenerateSelfSignedCertificate\obj\, .\GenerateSelfSignedCertificate\x64\ -Recurse -Force
ls .\GenerateSelfSignedCertificate\

    Directory: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----           11/2/2024  6:43 PM                Properties
-a---          12/13/2024  3:49 PM            966 GenerateSelfSignedCertificate.csproj
-a---           9/13/2024 10:15 PM            334 GenerateSelfSignedCertificate.csproj.user
-a---           11/2/2024  6:52 PM           1249 Program.cs
```

2) Clean project
```sh
msbuild /t:clean .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj

MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 5:29:37 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (clean target(s)).
CoreClean:
  Creating directory "obj\x64\Debug\net8.0\".
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (clean target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.50
```

3. Restore nuget packages
```sh
dotnet restore  .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
Build started 12/13/2024 5:53:46 PM.
     1>Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (Restore target(s)).
     1>_GetAllRestoreProjectPathItems:
         Determining projects to restore...
       Restore:
         X.509 certificate chain validation will use the default trust store selected by .NET for code signing.
         X.509 certificate chain validation will use the default trust store selected by .NET for timestamping.
         Restoring packages for C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj...
           GET https://api.nuget.org/v3/vulnerabilities/index.json
           OK https://api.nuget.org/v3/vulnerabilities/index.json 10ms
           GET https://api.nuget.org/v3-vulnerabilities/2024.12.13.05.09.02/vulnerability.base.json
           GET https://api.nuget.org/v3-vulnerabilities/2024.12.13.05.09.02/2024.12.13.05.09.02/vulnerability.update.json
           OK https://api.nuget.org/v3-vulnerabilities/2024.12.13.05.09.02/vulnerability.base.json 10ms
           OK https://api.nuget.org/v3-vulnerabilities/2024.12.13.05.09.02/2024.12.13.05.09.02/vulnerability.update.json 14ms
         Generating MSBuild file C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\GenerateSelfSignedCertificate.csproj.nuget.g.pr
         ops.
         Generating MSBuild file C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\GenerateSelfSignedCertificate.csproj.nuget.g.ta 
         rgets.
         Writing assets file to disk. Path: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\project.assets.json
         Restored C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj (in 353 ms).

         NuGet Config files used:
             C:\Users\mbryksin\AppData\Roaming\NuGet\NuGet.Config
             C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config

         Feeds used:
             https://api.nuget.org/v3/index.json
             https://artifactory.rd.veeam.dev/artifactory/api/nuget/nuget-group
             C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\
             C:\local
             https://tfs.veeam.dev/DefaultCollection/ExchangeExplorer/_packaging/internal-nugets/nuget/v3/index.json
     1>Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (Restore target( 
       s)).

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.92
```

4. Build project
```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /p:Configuration=Debug
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 5:20:17 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (default targets).
GenerateTargetFrameworkMonikerAttribute:
Skipping target "GenerateTargetFrameworkMonikerAttribute" because all output files are up-to-date with respect to the input files.
CoreGenerateAssemblyInfo:
Skipping target "CoreGenerateAssemblyInfo" because all output files are up-to-date with respect to the input files.
_GenerateSourceLinkFile:
  Source Link file 'obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.sourcelink.json' is up-to-date.
CoreCompile:
Skipping target "CoreCompile" because all output files are up-to-date with respect to the input files.
_CreateAppHost:
Skipping target "_CreateAppHost" because all output files are up-to-date with respect to the input files.
_CopyOutOfDateSourceItemsToOutputDirectory:
Skipping target "_CopyOutOfDateSourceItemsToOutputDirectory" because all output files are up-to-date with respect to the input files.
GenerateBuildDependencyFile:
Skipping target "GenerateBuildDependencyFile" because all output files are up-to-date with respect to the input files.
GenerateBuildRuntimeConfigurationFiles:
Skipping target "GenerateBuildRuntimeConfigurationFiles" because all output files are up-to-date with respect to the input files.
CopyFilesToOutputDirectory:
  GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.dl 
  l
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (default targets).      


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.78
```

5. Publish for Windows
```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 6:06:04 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (publish target(s)).
_CopyResolvedFilesToPublishPreserveNewest:
Building target "_CopyResolvedFilesToPublishPreserveNewest" partially, because some output files are out of date with respect to their input files.
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.dll" to "C:\
  Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.dll".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.pdb" to "C:\
  Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.pdb".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.deps.json" to "C
  :\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.deps.json".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.runtimeconfig.js
  on" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.runtimeconfig.json".
_CopyResolvedFilesToPublishAlways:
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\apphost.exe" to "C:\Users\mbryksin\Desktop
  \linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.exe".
Publish:
  GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_win\
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.72
```

6. Create apphost for linux

Only one apphost.exe wich is windows one

```sh
ls .\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\

    Directory: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          12/13/2024  6:04 PM                ref
d----          12/13/2024  6:04 PM                refint
d----          12/13/2024  6:04 PM                win-x64
-a---          12/13/2024  6:04 PM            198 .NETCoreApp,Version=v8.0.AssemblyAttributes.cs
-a---          12/13/2024  6:04 PM         139264 apphost.exe
-a---          12/13/2024  6:04 PM              0 Generate.0DD12D91.Up2Date
-a---          12/13/2024  6:04 PM           1061 GenerateSelfSignedCertificate.AssemblyInfo.cs
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.AssemblyInfoInputs.cache
-a---          12/13/2024  6:06 PM           1353 GenerateSelfSignedCertificate.assets.cache
-a---          12/13/2024  6:04 PM            555 GenerateSelfSignedCertificate.csproj.AssemblyReference.cache
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.csproj.CoreCompileInputs.cache
-a---          12/13/2024  6:04 PM           3033 GenerateSelfSignedCertificate.csproj.FileListAbsolute.txt
-a---          12/13/2024  6:04 PM           5632 GenerateSelfSignedCertificate.dll
-a---          12/13/2024  6:04 PM            660 GenerateSelfSignedCertificate.GeneratedMSBuildEditorConfig.editorconfig
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.genruntimeconfig.cache
-a---          12/13/2024  6:04 PM          11676 GenerateSelfSignedCertificate.pdb
-a---          12/13/2024  6:04 PM            189 GenerateSelfSignedCertificate.sourcelink.json
-a---          12/13/2024  6:06 PM            934 PublishOutputs.f16676fdf0.txt
```
Let's call MsBuild target for creating apphost for linux.

```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /p:Configuration=Debug /t:ResolveFrameworkReferences;_CreateAppHost /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 6:07:53 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (ResolveFrameworkReferences
;_CreateAppHost target(s)).
GenerateTargetFrameworkMonikerAttribute:
Skipping target "GenerateTargetFrameworkMonikerAttribute" because all output files are up-to-date with respect to the input files.
CoreGenerateAssemblyInfo:
Skipping target "CoreGenerateAssemblyInfo" because all output files are up-to-date with respect to the input files.
_GenerateSourceLinkFile:
  Source Link file 'obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.sourcelink.json' is up-to-date.
CoreCompile:
Skipping target "CoreCompile" because all output files are up-to-date with respect to the input files.
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (ResolveFrameworkRefere
nces;_CreateAppHost target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.75
```
Run ls one more time.

```sh
ls .\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\

    Directory: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          12/13/2024  6:04 PM                ref
d----          12/13/2024  6:04 PM                refint
d----          12/13/2024  6:04 PM                win-x64
-a---          12/13/2024  6:04 PM            198 .NETCoreApp,Version=v8.0.AssemblyAttributes.cs
-a---          12/13/2024  6:07 PM          72520 apphost
-a---          12/13/2024  6:04 PM         139264 apphost.exe
-a---          12/13/2024  6:04 PM              0 Generate.0DD12D91.Up2Date
-a---          12/13/2024  6:04 PM           1061 GenerateSelfSignedCertificate.AssemblyInfo.cs
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.AssemblyInfoInputs.cache
-a---          12/13/2024  6:07 PM           1269 GenerateSelfSignedCertificate.assets.cache
-a---          12/13/2024  6:04 PM            555 GenerateSelfSignedCertificate.csproj.AssemblyReference.cache
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.csproj.CoreCompileInputs.cache
-a---          12/13/2024  6:04 PM           3033 GenerateSelfSignedCertificate.csproj.FileListAbsolute.txt
-a---          12/13/2024  6:04 PM           5632 GenerateSelfSignedCertificate.dll
-a---          12/13/2024  6:04 PM            660 GenerateSelfSignedCertificate.GeneratedMSBuildEditorConfig.editorconfig
-a---          12/13/2024  6:04 PM             66 GenerateSelfSignedCertificate.genruntimeconfig.cache
-a---          12/13/2024  6:04 PM          11676 GenerateSelfSignedCertificate.pdb
-a---          12/13/2024  6:04 PM            189 GenerateSelfSignedCertificate.sourcelink.json
-a---          12/13/2024  6:06 PM            934 PublishOutputs.f16676fdf0.txt
```

Now we have 2 apphost files inside the same folder.

7. Publish for Linux
```sh
msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n   
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
Build started 12/13/2024 6:09:39 PM.

Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" on node 1 (publish target(s)).
PrepareForPublish:
  Creating directory "x64\Debug\publish_linux\".
_CopyResolvedFilesToPublishPreserveNewest:
  Copying file from "C:\Users\mbryksin\.nuget\packages\system.security.cryptography.protecteddata\6.0.0\lib\net6.0\System.Security.Cryptography.ProtectedData.dll" to "C:\Users\m
  bryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\System.Security.Cryptography.ProtectedData.dll".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.pdb" to "C:\
  Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.pdb".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.deps.json" to "C
  :\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.deps.json".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.runtimeconfig.js
  on" to "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.runtimeconfig.json".
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\GenerateSelfSignedCertificate.dll" to "C:\
  Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate.dll".
_CopyResolvedFilesToPublishAlways:
  Copying file from "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\apphost" to "C:\Users\mbryksin\Desktop\lin
  kedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\GenerateSelfSignedCertificate".
Publish:
  GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\publish_linux\
Done Building Project "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish target(s)).


Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:00.59
```

8. Let's ensure that the appliaction works on windons and on linux
```sh
 .\GenerateSelfSignedCertificate\x64\Debug\publish_win\GenerateSelfSignedCertificate.exe      
-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEA3wfn3eeyGMhCKBXQsG5NTr+8CpjT8PtCR/DLjzXACO8W+LZU2jhe
OtJ3a96nLNJsmgcUpFuD2lhzji4xyt9SnYoFOWtq+7MF6D5btClx9uhAjQeTPbzX
DDD+SCxVRbMaBsBJuBAWe0FVFITiVzsSp2+uMTtwB2ITBXKusM85kEmgH0HrUhqT
opGghhCGLiONhENU6DgPlMGUJdOcS0jJICEtXMN+6BK/KnRGl1CaImHqhy5K9quf
sDEQ68DKNOLexzhFcpeCF71DGkxAk+m+tySH1eP5HpnGjdYRW8u1nR/TImIPy+Oy
ioN/kSOJuGZgUrYCbauAowEXGvgASExd4QIDAQAB
-----END RSA PUBLIC KEY-----

-----BEGIN RSA PRIVATE KEY-----
MIIEpQIBAAKCAQEA3wfn3eeyGMhCKBXQsG5NTr+8CpjT8PtCR/DLjzXACO8W+LZU
2jheOtJ3a96nLNJsmgcUpFuD2lhzji4xyt9SnYoFOWtq+7MF6D5btClx9uhAjQeT
PbzXDDD+SCxVRbMaBsBJuBAWe0FVFITiVzsSp2+uMTtwB2ITBXKusM85kEmgH0Hr
UhqTopGghhCGLiONhENU6DgPlMGUJdOcS0jJICEtXMN+6BK/KnRGl1CaImHqhy5K
9qufsDEQ68DKNOLexzhFcpeCF71DGkxAk+m+tySH1eP5HpnGjdYRW8u1nR/TImIP
y+OyioN/kSOJuGZgUrYCbauAowEXGvgASExd4QIDAQABAoIBAQCGemHYb0f3oWgi
e2TlvNxoSzu6uAOzHg0NC6fxpdswh9k2BdqL6ckHdrgDFrF+WTKafQJ5R6TtFsA2
Tqw/QwCxdPNJd/d95Kf/LPpmW0cYNVoWryac5yxTcYchRXn1GNTslSzeCvVDRFVk
letT5Y6N3s0NamVWTlBhzJCAk7KFyH7ViUw9OVxsW2uncOUXcCq7Uz3ixehPTY4s
8iJPGZDOLCCfhNAdJOU0N9+B1U8CG0hDb2EsnWiB+qVALoLJn8wxsm6d+u/H6wFv
x/MDoRkxLalvXkkO7WiXmT4Odie57rZqg9f+rLtI+AY6z5LZ1XS+KyqrQCKlNZs5
tWf2S5sxAoGBAOzlSWr2vbl21iqUi3hcsQDZ+msehkVUoFU9IS6Za7hgbrOSyKTo
OTRN/0lBuCAQFj84fhwr/JICBpDD/UN1VBuGdetS15kDkirB2U3tTJSAV9QSMKc2
pYNT+Hn+Sru2sA4msMuMk4OmymR+G/qqzO0aGMRThNtxUF58V25opw27AoGBAPEE
YXCVyZMoFFgZMJknNpJKrUtkI/88YuAMldMjauQTVw9+br1GRroYJ6OwNcRECOaH
NT6z45lOeQYO6gpjZG8gvSrUQN7tl0aBYinmuSwQqYvnGaZZuX5n9DP9GqEepzji
9M6VXg9kwEn8RAe/1LKbrq1Oth3JfYNfKSzdUPsTAoGBANMShLTqfic2zCIKUq5+
oDNrBOXWv7ocafMo0Vzs0/7m5RvZKC9OvlFtQY3rIXxn+PqBglPlmVgat/Dav9kQ
PE1+I6j2GiU6+kxghhcZ3Ubfh+HsBy+l0BlQgy9nNP1GDF2/eX0TlwgRX3nkp1dU
QdzsiK68376Kmxpk3Z4BXv8rAoGBALg5bvmNpMx0wEguyPToAlCEAD42R0WbNMCp
HgSLd/LpzYwsSh0nEHzCZdo6oH5quprrEonhGsFeOCenUsGqA2TmE3IfV46O8SiV
USFSGIxUGCS1+ucqghza/NCYULiDI7LZ1+HoTkNZ8Zkb2CxMNxpm4XfbSF0wXF4E
aQbADFw9AoGADMObRPd7qCQyM64+V43R0I9pJQvEbYM37BFPilC9G8MBqWCbF9oz
+Vzjmf8TFUYCTB22BLV5oX2W1ja36+YEv+UPLDaiEriSyKH6lF9q07payBVxvk7Y
8vXoQwmjXw2hAou7lOlwbX03DRCAG6JLgEJNKlVI+k4nyNwl4wsJu/I=
-----END RSA PRIVATE KEY-----
```

```sh
user@PRG10086:/mnt/c/Users/mbryksin/Desktop/linkedIn/publish/Project/net_publish$ ./GenerateSelfSignedCertificate/x64/Debug/publish_linux/GenerateSelfSignedCertificate
-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEAv47bthtvbQRV35LNoq+9JPU0BQD1XnnclFysfwszFN426adDtGkq
4frMjSdtHEdEUvCkCZQmyt3gVBeYiO118Z0JGvjT3fn8sTjCIv8mSz+PzEpi7gdz
rkHH0a2OYNgHN3UJOFg//QiMV20+S86O2lhIr7vP4A/hQVIEeqk4LHgSXXaPOtzT
Ia6i52hu7dFP3bFmui/P9Updt5xN+Owxvw8OsZJAFeLiDbaGjXDepUV6BqXVdqJ/
E78sGNXa7MeoW5ZeKfk5gzu1VnUMZ2VeCclVoZfwtZfSK/QSkRizZ9KC98eAlJOw
bzeswVylEiT8+z2Z6NSqj2C8gyiqw+U/cQIDAQAB
-----END RSA PUBLIC KEY-----

-----BEGIN RSA PRIVATE KEY-----
MIIEoQIBAAKCAQEAv47bthtvbQRV35LNoq+9JPU0BQD1XnnclFysfwszFN426adD
tGkq4frMjSdtHEdEUvCkCZQmyt3gVBeYiO118Z0JGvjT3fn8sTjCIv8mSz+PzEpi
7gdzrkHH0a2OYNgHN3UJOFg//QiMV20+S86O2lhIr7vP4A/hQVIEeqk4LHgSXXaP
OtzTIa6i52hu7dFP3bFmui/P9Updt5xN+Owxvw8OsZJAFeLiDbaGjXDepUV6BqXV
dqJ/E78sGNXa7MeoW5ZeKfk5gzu1VnUMZ2VeCclVoZfwtZfSK/QSkRizZ9KC98eA
lJOwbzeswVylEiT8+z2Z6NSqj2C8gyiqw+U/cQIDAQABAoH/b+1z87Q+0d72o5bj
u2t0Lrls69TjijsqPTDpRFJcRpU7q3Wl2dnuyfFWvs41XZCcG4UAVEMnZ4y9cbqt
Jf+aKtMsaYDQSvkG7YZ8k8us/yA5+uR8FG5w6XZbiz4d3fhcPyCOLUi4xYt5H/HK
X1ZnogJzy5Bcpk9LucnH/uQMLplKowc5McTuruhdZEBZMiYtfJZwsU34Y41AJLBC
sHaaouoWx4VM9e9H0RtOKP4TZktt5lds3J1s6ZdHKHuehKUqLHKQq22afL9RZhTl
2PGzSVX/g7It2AwkSFhY33Rf0ZAXTJhsE0zjaIBkkJcbHtIlYOU/W/MvPYZd2Yvd
1LqRAoGBAPsPnQL6CObbeRSUHrWCdUa924BYl5IEMkLs5BCJM13j/8cBtkKE8pgc
eZ5qB8EYxamP0SI9w6oJRKClmva/j28ldg8WodTu83pm6QYmOuaglAKMGzVjfvwh
Hc/8h6l6PB7YCpNKyjLMt4xLXyq+sp7LfNYYbDgOIQPIw8qYGK3ZAoGBAMNTk+du
5/UyqLWain13oP57FcaGirMg1WfmWWu2E+Lx/VouURX8Wvt/6CAEGGrj3GGaKiSK
haHLRU6yP7GLm75MnEN3L5AUd1ejIfquZZJBpBRx/+UKHWynK1Lkxat5mooC5TG8
7VmfjSkMDbq21ZosQlT92cEMFglhHwzgwudZAoGACEZLdn2nLXSuWO9I09Ko6tv3
EBPbawSYY3xLSAA9oSqSk4yK5UZceIb3uzDjcInQefYzfl1qxX/osyLCKL/HinJB
od2tF8eIXtBlfb5k0pUYS70yaGAPH2A4C1LXZc6RjLKyJoiggWwd4JHbYR1H5dPv
GV9UscRFckp7qYG5zoECgYEAlJEllorM87ushzUc0YIEeou0bGQ5azY2G+khasP7
LEtToRxJoKdprJIFRRTYXbUjEznnhBO4wO+Tr9/0gl7l/0DWOpqVGsn2XDpAxiOQ
LkavPr3XHacr6lDcqxhHIb5ExWSeX86L2fd5rxbz+mjG35V6fpr23dcLi+bLb0gl
90ECgYAbB0BMEFHfjZ7NLcQeFwoouyKCReB7yYFsF9tzS0HzpilvMA1J7YyQKbqM
80s16Clpd14cGi/ca+xrVVUFPXHVAsbPvjM/KNhorB+94ndgFy75AreAl+NtvwOD
HcuVvwhpyJQWUkUDhKGh5rSvPfh4z5Nf1dfD6rS3c3rE0ud6IQ==
-----END RSA PRIVATE KEY-----
```

<h2>worth mentioning:</h2>

1) The solution above works for entreprise comples solutions.
2) Conditional compilation is out of scope of this article.
