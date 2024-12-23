# net_publish
An example how to publish a project for windows and linux without restoring and rebuilding twice.

I have a simple .net 8 console application which generates self signed certificate and writes public and private keys to standard ouput in PEM format.
The apllication is crossplatform, it utulizes System.Security.Cryptography.ProtectedData nuget package and it can work on windown and on linux.

Let's look at .csproj file.

I've explicitly set AppendTargetFrameworkToOutputPath and AppendRuntimeIdentifierToOutputPath to true to mirror build output structure.
<OutputPath>$(Platform)\$(Configuration)</OutputPath>
<AppendTargetFrameworkToOutputPath>true</AppendTargetFrameworkToOutputPath>
<AppendRuntimeIdentifierToOutputPath>true</AppendRuntimeIdentifierToOutputPath>
Build output is ./GenerateSelfSignedCertificate/x64/Debug/net8.0/

I've also explicitly specified multiple runtime indentifiers reflecting that I'd like my app working for both runtimes - win-x64 and linux-x64.
<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>
Now, it is possible to build and run the application on the same platform regardless if it is win-64 or linux-x64 form single folder.
./GenerateSelfSignedCertificate/x64/Debug/net8.0/
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
GenerateSelfSignedCertificate            GenerateSelfSignedCertificate.dll  GenerateSelfSignedCertificate.runtimeconfig.json  runtimes
GenerateSelfSignedCertificate.deps.json  GenerateSelfSignedCertificate.pdb  System.Security.Cryptography.ProtectedData.dll

*runtimes folder is right here in build out folder + univeral System.Security.Cryptography.ProtectedData.dll is placed into root build output folder*

I primarly use windows so i just build the app and run it. It works from /x64/Debug/net8.0 folder even without publish.
However I can just switch from windows to linux and do the same - just build and run and it works without publish.

There're 3 options how you can specify RID for you project:
1) no RID in csproj
2) single RID - <RuntimeIdentifier>win-x64</RuntimeIdentifier>
3) multiple RID - <RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers>
Honestly you don't need to specify RID in multuple form -option 2). It can be avoided cause option 1) and 3) equals.
When no RID is specified or multiple RID specified .net pulls all available runtimes for you project and copies them into single build output foldee.  deps.json file is orgonizae  correspondingly. *mention Radim and link to MS*
https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md
https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/corehost.md#assembly-resolution

However if you specify single RuntimeIdentifier - option 2) the output structure will be different.
ls ./GenerateSelfSignedCertificate/x64/Debug/net8.0/
linux-x64  runtimes  win-x64

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
apphost problem - error msg
binary log and Martin helps - screenshot
final version - scripts + output

to mention
1) it works for complex and enterprise projects
2) it works for selfcontained apps
3) conditional building is out of scope
