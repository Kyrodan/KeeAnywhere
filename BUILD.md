# Build
Just run *build.cmd*.
In folder *build* you now see two subfolders:

* bin: compiled output files
* dist: packages for distribution

Note: NuGet packages are restored via MSBuild (`msbuild -t:restore -p:RestorePackagesConfig=true`) - no separate *nuget.exe* needed.

# API keys
The OAuth client ids/secrets of the storage providers live in *KeeAnywhere\ApiKeys.cs*, which is **not** under version control (.gitignore).
On the first build it is created automatically from *KeeAnywhere\ApiKeys.Template.cs* with dummy values - enough to compile, but connecting to Box, Dropbox, Google Drive, HiDrive and OneDrive won't work.

* For release builds (and testing these providers) put the real keys into *KeeAnywhere\ApiKeys.cs*.
* Never put real keys into *ApiKeys.Template.cs* or the *...Helper.cs* files.
* When adding a key, add it to both *ApiKeys.Template.cs* (dummy value) and your local *ApiKeys.cs*.

# Binding redirects
KeePass does not evaluate binding redirects of plugins. At runtime, version mismatches between the bundled dependencies are resolved by the AssemblyResolve handler in *KeeAnywhereExt.cs* (the DLL shipped with the plugin wins).
*KeeAnywhere\app.config* is excluded from the PLGX and only keeps the build free of MSB3247/MSB3277 warnings. After updating NuGet packages, check the build for these warnings and add the redirects MSBuild suggests (only for strong-named assemblies, always pointing to the shipped version).

# Test
Run *test-plgx.ps1 [-Cleanup] [-Reset] [-KeePassDir <path to portable KeePass>]*.
It builds the PLGX, deploys it to KeePass, starts KeePass with *--debug* and reports whether KeePass could compile the plugin.
All KeePass instances must be closed before.

By default a test copy of the KeePass in *lib* (the version KeeAnywhere is built against) is used in *temp\test-plgx* (ignored by git, *lib* is never written):

* KeePass.exe, KeePass.XmlSerializers.dll and KeePass.exe.config are copied from *lib* if missing or changed.
* Everything else is kept between runs for repeatable tests: *KeePass.config.xml* (portable mode - your productive KeePass configuration is not touched), KeeAnywhere accounts and offline cache stored in the KeePass folder, and test databases (put them into *temp\test-plgx\Databases*).
* *-Reset* deletes *temp\test-plgx* first for a completely fresh test environment.
* *-Cleanup* wipes the KeePass PluginCache, so KeePass compiles the PLGX again.

Note: KeeAnywhere accounts in the Windows Credential Manager and the KeePass PluginCache (*%LOCALAPPDATA%\KeePass\PluginCache*) are per Windows user, so test and productive use share them.

# Prepare new release
Each released version is tagged with a version tag: this tag follows [Semantic Versioning](http://semver.org/).
When preparing for a new release the last steps is to change the version informations (e.g. 0.1.0-alpha):

* KeeAnywhere\Properties\AssemblyInfo.cs
  * AssemblyVersion and AssemblyFileVersion: numeric only (e.g. 0.1.0.0) - no suffix allowed
  * AssemblyInformationalVersion: full version including suffix (e.g. 0.1.0-alpha)
* build.cmd
* .github\workflows\build.yml (env: version)
* CHANGELOG.md
* chocolatey\keepass-plugin-keeanywhere.nuspec
  * version
  * releaseNotes (copy of the CHANGELOG.md section of this version - only for releases)
* version_manifest.txt (change only for production releases)

If the required KeePass version changes, update it consistently in:

* lib\KeePass.exe, lib\KeePass.XmlSerializers.dll and lib\KeePass.exe.config (all from the KeePass portable ZIP)
* KeeAnywhere\KeeAnywhere.csproj
  * Reference KeePass (Version)
  * PlgxConfiguration: KeePassVersion
* chocolatey\keepass-plugin-keeanywhere.nuspec: dependency keepass
* CHANGELOG.md

After having pushed and tagged the current version, then change version to **next** unstable version (e. g. 0.2.0-unstable) - just to make clear ths is a development snapshot and generally not released to public.

# Version examples
Versions are always counted upwards. Each version is unique. 
Additional informations like *alpha*, *beta* only describe the characteristics of the version. 
Until version 1.0.0 the minor version will be increased.
  
* 0.1.0-alpha: Alpha release for previewing or testing new features. Not for production use.
* 0.2.0-unstable: Current development snapshot. These versions will never be released to public.
* 0.2.0-beta: Beta releases are feature complete. Only bugfixing should take place before a new main release
* 1.0.0: production version

 