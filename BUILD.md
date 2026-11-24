# Build
Just run *build.cmd*.
In folder *build* you now see two subfolders:

* bin: compiled output files
* dist: packages for distribution

Note: NuGet packages are restored via MSBuild (`msbuild -t:restore -p:RestorePackagesConfig=true`) - no separate *nuget.exe* needed.

# Test
Run *test-plgx.ps1 [-KeePassDir <path to portable KeePass>] [-Cleanup]*.
It builds the PLGX, deploys it to KeePass, starts KeePass with *--debug* and reports whether KeePass could compile the plugin.
All KeePass instances must be closed before.

By default the KeePass in *lib* is used (the version KeeAnywhere is built against). It runs in portable mode via *lib\KeePass.config.xml* (created if missing), so the configuration of your productive KeePass installation is not touched. *lib\Plugins* and *lib\KeePass.config.xml* are ignored by git.
Note: KeeAnywhere accounts are stored per Windows user, so test and productive use share them.

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

* lib\KeePass.exe and lib\KeePass.XmlSerializers.dll
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

 