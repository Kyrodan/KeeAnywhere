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
The standard way of installing KeePass plugins is to place them in the `Plugins` folder next to `KeePass.exe` (e.g. `C:\Program Files\KeePass Password Safe 2\Plugins` or `C:\Program Files (x86)\KeePass Password Safe 2\Plugins`). [Download the PLGX file]({{ plgx.browser_download_url }}) and copy it to that folder.

_Remarks:_ The preferred distribution is the PLGX file. The [ZIP file]({{ zip.browser_download_url }}) is only needed for special cases. If you need it, unpack the downloaded file into the `Plugins` folder.

All files of the latest version (including SHA256 checksums) can be found on the [release page]({{ site.github.latest_release.html_url }}).

## Automatic Installation using Chocolatey/Scoop
[Chocolatey](https://chocolatey.org/) is a package manager for Windows. To install KeeAnywhere (and KeePass, if not present) just type `choco install keepass-plugin-keeanywhere` in a command shell.

[Scoop](https://scoop.sh/) is an [alternative to Chocolatey](https://github.com/ScoopInstaller/Scoop/wiki/Chocolatey-Comparison) on Windows. KeeAnywhere is available in Scoop's *extras* bucket, so add this bucket first. Type in PowerShell:

```
scoop bucket add extras
scoop install keepass-plugin-keeanywhere
```

_Remarks:_ The Scoop package is maintained by the Scoop community and may be updated later than the other distributions.


# Verify correct installation
After installation please start KeePass. It now compiles KeeAnywhere. If it fails, an error occurs. In this case please follow [Troubleshooting](troubleshooting). 

Verify that all conditions are met:
* Under `Tools - Plugins...` an entry for KeeAnywhere exists
* The menu entry `Tools - KeeAnywhere Settings...` exists
* The menu entry `File - Open - Open from Cloud Drive...` exists