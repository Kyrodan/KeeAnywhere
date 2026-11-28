---
title: Installation
---
# Prerequisites
Please install the following prerequisites first:
* KeePass 2.62 or newer
* Windows with .NET Framework 4.8 or newer

_Remarks:_ Linux/Mono and Wine are currently not supported - see [Known Issues](known_issues).

# Installation
Please choose between manual installation and an automatic installation.

## Manual Installation
{% assign plgx = site.github.latest_release.assets | where_exp: "asset", "asset.name contains '.plgx'" | first %}
{% assign zip = site.github.latest_release.assets | where_exp: "asset", "asset.name contains '.zip'" | first %}
KeePass plugins are installed in the `Plugins` folder next to `KeePass.exe` (e.g. `C:\Program Files\KeePass Password Safe 2\Plugins` or `C:\Program Files (x86)\KeePass Password Safe 2\Plugins`):
1. Close KeePass.
2. [Download the ZIP file]({{ zip.browser_download_url }}) (DLL version).
3. Unpack it into the subfolder `Plugins\KeeAnywhere` (create it, if it doesn't exist) - never directly into the `Plugins` folder or the KeePass program folder.

_Remarks:_ The ZIP file is the recommended distribution, as [recommended by KeePass](https://keepass.info/help/v2_dev/plg_index.html#plgx) for security reasons. The [PLGX file]({{ plgx.browser_download_url }}) is an alternative for custom KeePass builds (e.g. KeePass packages of Linux distributions): copy it into the `Plugins` folder - KeePass compiles it at the next start. Use only one of both, never both at the same time.

All files of the latest version (including SHA256 checksums) can be found on the [release page]({{ site.github.latest_release.html_url }}).

## Automatic Installation using Chocolatey/Scoop
[Chocolatey](https://chocolatey.org/) is a package manager for Windows. To install KeeAnywhere (and KeePass, if not present) just type `choco install keepass-plugin-keeanywhere` in a command shell.

[Scoop](https://scoop.sh/) is an [alternative to Chocolatey](https://github.com/ScoopInstaller/Scoop/wiki/Chocolatey-Comparison) on Windows. KeeAnywhere is available in Scoop's *extras* bucket, so add this bucket first. Type in PowerShell:

```
scoop bucket add extras
scoop install keepass-plugin-keeanywhere
```

_Remarks:_ The Scoop package is maintained by the Scoop community and may be updated later than the other distributions.


# Updating from an older version {#updating}
* **ZIP file:** close KeePass, delete the content of the folder `Plugins\KeeAnywhere` and unpack the new ZIP file into it.
* **PLGX file:** replace the old PLGX file in the `Plugins` folder with the new one.
* **Switching from the PLGX file to the ZIP file:** delete the old PLGX file (e.g. `KeeAnywhere.plgx` or `keepass-plugin-keeanywhere.plgx`) from the `Plugins` folder - otherwise KeeAnywhere is loaded twice.
* **Chocolatey/Scoop:** close KeePass and update the package as usual. Starting with version 2.4.0 the Chocolatey package installs the ZIP file (DLL version) into `Plugins\KeeAnywhere` and removes the PLGX file of older versions automatically.

If you used the **ZIP file** of an older version before somewhere else than in `Plugins\KeeAnywhere` (e.g. as workaround for KeeAnywhere 2.1.0 with KeePass 2.62), remove **all** of its files before installing the new version - otherwise old libraries may be loaded and KeeAnywhere cannot be loaded anymore (see [Troubleshooting](troubleshooting#plugin-cannot-be-loaded-after-an-update)):
1. Close KeePass.
2. Remove all files of the old version (`KeeAnywhere.dll` and about 60 other DLL files like `Microsoft.Graph.dll`, `Box.V2.dll`, `Dropbox.Api.dll`, `Google.Apis*.dll`, ...). Check the `Plugins` folder **including all subfolders** and the KeePass program folder itself (the folder containing `KeePass.exe`) - only KeePass' own files should be there.
3. Delete the KeePass plugin cache: the folder `%LOCALAPPDATA%\KeePass\PluginCache`.
4. Install the new version as described above.

# Verify correct installation
After installation please start KeePass (with the PLGX file, KeePass compiles KeeAnywhere now). If KeeAnywhere cannot be loaded, an error occurs. In this case please follow [Troubleshooting](troubleshooting). 

Verify that all conditions are met:
* Under `Tools - Plugins...` an entry for KeeAnywhere exists
* The menu entry `Tools - KeeAnywhere Settings...` exists
* The menu entry `File - Open - Open from Cloud Drive...` exists