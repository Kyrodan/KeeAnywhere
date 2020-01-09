# Build
Just run *build.cmd*.
In folder *build* you now see two subfolders:

* bin: compiled output files
* dist: packages for distribution

# Prepare new release
Each released version is tagged with a version tag: this tag follows [Semantic Versioning](http://semver.org/).
When preparing for a new release the last steps is to change the version informations (e.g. 0.1.0-alpha):

* KeeAnywhere\Properties\AssemblyInfo.cs
* build.cmd
* CHANGELOG.md
* chocolatey\keepass-plugin-keeanywhere.nuspec
* version_manifest.txt (change only for production releases)

After having pushed and tagged the current version, then change version to **next** unstable version (e. g. 0.2.0-unstable) - just to make clear ths is a development snapshot and generally not released to public.

# Version examples
Versions are always counted upwards. Each version is unique. 
Additional informations like *alpha*, *beta* only describe the characteristics of the version. 
Until version 1.0.0 the minor version will be increased.
  
* 0.1.0-alpha: Alpha release for previewing or testing new features. Not for production use.
* 0.2.0-unstable: Current development snapshot. These versions will never be released to public.
* 0.2.0-beta: Beta releases are feature complete. Only bugfixing should take place before a new main release
* 1.0.0: production version

 