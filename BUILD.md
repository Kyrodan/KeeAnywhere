# Build
Just run *build.cmd*.
In folder *build* you now see two subfolders:

* bin: compiled output files
* dist: packages for distribution

Note: NuGet packages are restored via MSBuild (`msbuild -t:restore -p:RestorePackagesConfig=true`) - no separate *nuget.exe* needed.

# CI (GitHub Actions)
*.github\workflows\build.yml* builds PLGX, ZIP, Chocolatey package and *SHA256SUMS.txt* (uploaded as workflow artifact):

* Every push and pull request on master (except changes in *docs*): build check with **dummy API keys** - artifact name ends with *-dummy-keys*, not for production use.
* Tags *v\** (e.g. *v2.2.0*): release build with the **real API keys**. The tag must match AssemblyInformationalVersion in *AssemblyInfo.cs* (tag *v2.2.0* for version *2.2.0*), otherwise the build fails.
* The version is read from AssemblyInformationalVersion - no version to maintain in the workflow.

For tags the job *release* then creates a **draft** GitHub release (only visible to maintainers) with all build artifacts and the CHANGELOG.md section of this version as release notes. The job fails if the CHANGELOG.md section is missing or has no release date (e.g. still "unreleased"), or if a release for this tag already exists.

Versions with suffix (e.g. *2.2.0-beta*) are marked as pre-release. For them the CHANGELOG.md section of the base version is used if there is no own section (e.g. *## 2.2.0 (unreleased)* for *2.2.0-beta*, no release date required), with a "not for production use" note on top. Pre-releases are not pushed to Chocolatey, and version_manifest.txt stays unchanged, so KeePass doesn't offer them as update.

Publishing the release (manually, after review) triggers *.github\workflows\chocolatey.yml*: it pushes the .nupkg attached to the release to chocolatey.org. Pre-releases are skipped. It can also be started manually for a given tag.

Repository secrets (Settings - Secrets and variables - Actions):

* **KEEANYWHERE_API_KEYS**: complete content of your local *KeeAnywhere\ApiKeys.cs*. Update it whenever *ApiKeys.cs* changes. A release build fails if it is missing or still contains dummy keys.
* **CHOCOLATEY_API_KEY**: API key of the chocolatey.org account maintaining *keepass-plugin-keeanywhere*.

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

Release steps:

1. Change the version information above, set the release date in CHANGELOG.md (e.g. *## 2.2.0 (2026-12-01)*), commit and push - but **not** version_manifest.txt yet (KeePass reads it directly from master for its update check).
2. Create and push the tag: `git tag v2.2.0` and `git push origin v2.2.0`.
3. Wait for the workflow: it builds with the real API keys and creates a draft GitHub release.
4. Review (and test) the draft release, then publish it - this pushes the package to Chocolatey.
5. Commit and push version_manifest.txt (production releases only), so KeePass offers the update.

After that, change version to **next** unstable version (e. g. 0.2.0-unstable) - just to make clear this is a development snapshot and generally not released to public.

# Version examples
Versions are always counted upwards. Each version is unique. 
Additional informations like *alpha*, *beta* only describe the characteristics of the version. 
Until version 1.0.0 the minor version will be increased.
  
* 0.1.0-alpha: Alpha release for previewing or testing new features. Not for production use.
* 0.2.0-unstable: Current development snapshot. These versions will never be released to public.
* 0.2.0-beta: Beta releases are feature complete. Only bugfixing should take place before a new main release
* 1.0.0: production version

 