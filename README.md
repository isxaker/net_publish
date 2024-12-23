# net_publish
An example how to publish a project for windows and linux without restoring and rebuilding twice.

Let's consier a simple .net 8 console application which generates self signed certificate and writes public and private keys to standard ouput in PEM format.
The apllication is crossplatform, it utulizes [System.Security.Cryptography.ProtectedData](https://www.nuget.org/packages/System.Security.Cryptography.ProtectedData/6.0.0) nuget package and it can work on windown and on linux.

Let's look at .csproj file.

I've explicitly set ``AppendTargetFrameworkToOutputPath`` and ``AppendRuntimeIdentifierToOutputPath`` to true to mirror build output structure.
```
<OutputPath>$(Platform)\$(Configuration)</OutputPath>
<AppendTargetFrameworkToOutputPath>true</AppendTargetFrameworkToOutputPath>
<AppendRuntimeIdentifierToOutputPath>true</AppendRuntimeIdentifierToOutputPath>
```
Build output is ``./GenerateSelfSignedCertificate/x64/Debug/net8.0/``
Obj folder location reflects this settnigs too ``./GenerateSelfSignedCertificate\obj\x64\Debug\net8.0``

I've also explicitly specified multiple runtime indentifiers reflecting that I'd like my app working for both runtimes - ``win-x64`` and ``linux-x64``.
```
<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>
```
Now, it is possible to build and run the application on the same platform regardless if it is ``win-64`` or ``linux-x64`` form single folder - ``./GenerateSelfSignedCertificate/x64/Debug/net8.0/``
```
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
GenerateSelfSignedCertificate            GenerateSelfSignedCertificate.dll  GenerateSelfSignedCertificate.runtimeconfig.json  runtimes
GenerateSelfSignedCertificate.deps.json  GenerateSelfSignedCertificate.pdb  System.Security.Cryptography.ProtectedData.dll
```

``*runtimes`` folder is right here in build output folder + univeral ``System.Security.Cryptography.ProtectedData.dll`` is placed into root build output folder*

I primarly use windows so i just build the app and run it. It works from ``/x64/Debug/net8.0`` folder even without publish.
And I can just switch from windows to linux and do the same - just build and run and it works without publish.

There're 3 options how you can specify RID for you project:
1) no RID in csproj
2) single RID - ``<RuntimeIdentifier>win-x64</RuntimeIdentifier>``
3) multiple RID - ``<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>``
Honestly you don't need to specify RID in multuple form -option 2). It can be avoided cause option 1) and 3) equals.
When no RID is specified or multiple RID specified .net pulls all available runtimes for you project and copies them into single build output folder.  deps.json file is orgonizae  correspondingly. *mention Radim and link to MS*
[](https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md)
[](https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md#assembly-resolution)


<details>
<summary>cat .\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfSignedCertificate.deps.json</summary>

```
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

However if you specify single RuntimeIdentifier - option 2) the output structure will be different.
```
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
linux-x64
``
```
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
<summary>cat .\GenerateSelfSignedCertificate\x64\Debug\net8.0\linux-x64\GenerateSelfSignedCertificate.deps.json</summary>
cat .\GenerateSelfSignedCertificate\x64\Debug\net8.0\linux-x64\GenerateSelfSignedCertificate.deps.json
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
</details>

Our goal is getting both windows and linux binaries.
I've added 2 publish profiles one per RID.
Publish would helps us to get only needed binaries for the target runtime.
The usual or even proper workflow to get ready to use application is restornig nugets, building binaries and then publishing them.
If we need out application to works on several runtimes we need to do all the steps twice.
This is the most strighth fowrward approach - if appraoch works for one target we need to repeat the same steps for every other target.
And this is correct,  everything will work. And it's even default behavior if you build\publish using VS UI.

However, two question we should ask ourselves:
1) why we need build our application more than once if the code is crossplatform ?
2) why we need to restore nugets more than once  if .net puts all binaries and runtimes folder into single build out folder during build for multiple RIDS why ? (assuming that MS allowed such a behaviour so it's supported)

Then let's look at dotnet build and dotnet publish cli documentation
dotnet build [--no-restore]
dotnet publish [--no-build] [--no-restore]
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build

Documentation kind of strengthens our assumption. MS allows building without restore and publishing without building and/or without restore.

--
attempt to do that which does not work + output
--

dotnet restore  .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
Build succeeded.
    0 Warning(s)
    0 Error(s)

dotnet build .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
CoreCompile:
Build succeeded.
    0 Warning(s)
    0 Error(s)

if you run build twice msbuild optimizes it's work
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
         GenerateSelfSignedCertificate -> C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\x64\Debug\net8.0\GenerateSelfS 
         ignedCertificate.dll

restore + build + publish works per RID
so for 2 RID we need to do that twice
RID win-x64
restore + build + publish works per RID
RID linux-x64
restore + build + publish works per RID

let's optimize
let's start with win-x64
how to publish w/out exta restore and w/out extra build ?

this does not work
dotnet publish --no-restore --no-build .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -p:publishprofile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml -c Debug -v n
Build FAILED.

C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug
       \net8.0\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSigne
       dCertificate\GenerateSelfSignedCertificate.csproj]

we need to help msbuild by specifying output directory with build binaries cause it uses the wrong one
which is obvious form the error message.
the problem that dotnet publish simply does not support it
it is possible to specify [-o|--output <OUTPUT_DIRECTORY>] for dotnet publish but it is not what we want.
this is just a target location for our published binaries where we need to help dotnetms msbuild with source folder whitch contains build binaries.
https://github.com/dotnet/sdk/issues/9012
so let's switch to msbuild which allow that and much more.
msbuild allows publishing without build and restore as well -  /p:RestorePackages=false /p:NoBuild=true

CoreCompile: is called when /p:NoBuild=false
CoreCompile: is skipped when /p:NoBuild=true

msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug
that does not work too
C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug\net8.0 
\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\G 
enerateSelfSignedCertificate.csproj]

msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug /p:AppendRuntimeIdentifierToOutputPath=false
that does not work too
C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(351,5): error MSB3030: Could not copy the file "obj\x64\Debug\net8.0 
\win-x64\GenerateSelfSignedCertificate.dll" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\G 
enerateSelfSignedCertificate.csproj]

msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_windows.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false
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
As you can see no CoreCompile taget was called. Cimpilation was skipped.
Ok we've just publish the app w/out restore and w/out build for windows.
(or to way that differertly we restore nugets and build the app only once during the build itself not during publish)

Let's run publish for linux.
 msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n

Build FAILED.

"C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj" (publish target) (1) ->
(_CopyResolvedFilesToPublishAlways target) ->
  C:\Program Files\dotnet\sdk\8.0.403\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Publish.targets(380,5): error MSB3030: Could not copy the file "C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\obj\x64\Debug\net8.0\apphost" because it was not found. [C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate\GenerateSelfSignedCer 
tificate.csproj]

    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:00.77

We encountered the problem with apphost.

And there 2 ways how it can be sokved.
In .NET Core 3.0 and later, when you publish an application, an executable file (apphost) is created by default for convenience so that you can run your application without specifying dotnet and the DLL name. This feature creates a platform-specific binary that simplifies launching your application.
If you want to disable the creation of the apphost and instead have the traditional approach of running your application using the dotnet command with your DLL, you can modify your project file (*.csproj) to achieve this.
Disable apphost for our project adding <UseAppHost>false</UseAppHost> to csproj file.
With this apphost is not created for our executable so you have to run the executable using dotnet which is not convineint and even my not be acceptable for some scenarious.
Ppublish for linux now works as expected.

--
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

But lack of apphost is not our case. Let's go a bit deeper.
We have just proved that apphost is the final problem which separates us from publishing for 2 different platforms using single build and restore.
In order to takle the apphost problem we need to analyze msbuild ooutput or binary log.
https://learn.microsoft.com/en-us/visualstudio/msbuild/obtaining-build-logs-with-msbuild?view=vs-2022#save-a-binary-log
https://msbuildlog.com/
My teammate @Martin Balous were succefully reserached the problem and found out the msbuild target responsible for apphost creation.

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

This what @Martin wrote to me:
--
after long time of reverse engineering the binary logs...
![screenshot](imageFolder/apphost_investigation.png)



Final commands
--
1) remove folders

MBryksin ❯ Remove-Item -Path .\GenerateSelfSignedCertificate\obj\, .\GenerateSelfSignedCertificate\x64\ -Recurse -Force
                                                                                                                                             net_publish   main ≡  ?1 ~2  
MBryksin ❯ ls .\GenerateSelfSignedCertificate\

    Directory: C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish\GenerateSelfSignedCertificate

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----           11/2/2024  6:43 PM                Properties
-a---          12/13/2024  3:49 PM            966 GenerateSelfSignedCertificate.csproj
-a---           9/13/2024 10:15 PM            334 GenerateSelfSignedCertificate.csproj.user
-a---           11/2/2024  6:52 PM           1249 Program.cs

rmdir /s /q .\GenerateSelfSignedCertificate\obj\ & rmdir /s /q .\GenerateSelfSignedCertificate\x64\
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

2) clean project
msbuild clean .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj
MSBuild version 17.11.9+a69bbaaf5 for .NET Framework
MSBUILD : error MSB1008: Only one project can be specified.
    Full command line: 'msbuild  clean .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj'
  Switches appended by response files:
'' came from 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.rsp'
Switch: .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj

For switch syntax, type "MSBuild -help"

C:\Users\mbryksin\Desktop\linkedIn\publish\Project\net_publish>msbuild /t:clean .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj 
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

3) restore
MBryksin ❯ dotnet restore  .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj -v n
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
                                      

4) build project
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

5) publish windows
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

intermidiate step
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

only one apphost.exe wich is windows one

6) create apphost for linux

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

run ls one more time
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

now we have 2 apphost files inside the same folder

7) publish linux

>msbuild .\GenerateSelfSignedCertificate\GenerateSelfSignedCertificate.csproj /t:publish /p:RestorePackages=false /p:NoBuild=true /p:PublishProfile=.\GenerateSelfSignedCertificate\Properties\PublishProfiles\FolderProfile_linux.pubxml /p:OutDir=.\x64\Debug\net8.0 /p:AppendRuntimeIdentifierToOutputPath=false -v:n   
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

8) let ensure that app works

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



worth mentioning:
--
1) The solution above works for entreprise comples solutions.
2) Conditional compilation is out of scope of this article.


**
[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin", 'Machine')

"terminal.integrated.defaultProfile.windows": "VSDev_PowerShell",
"terminal.integrated.profiles.windows": {
    "VsDevCmd (2022)": {
        "path": [
            "${env:windir}\\Sysnative\\cmd.exe",
            "${env:windir}\\System32\\cmd.exe"
        ],
        "args": [
            "/k",
            "C:/Program Files/Microsoft Visual Studio/2022/Professional/Common7/Tools/VsDevCmd.bat",
            "-arch=x64",
            "-host_arch=x64"
        ],
        "overrideName": true,
        "icon": "terminal-cmd"
    },
    //