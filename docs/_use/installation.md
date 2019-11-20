---
title: Installation
---
# Prerequisites
Please install the following prerequisites first:
* KeePass 2.40 or newer
* Windows 7 or newer
* .Net Framework 4.5.2 or newer

# Installation
Please choose between manual installation and an automatic installation.

## Manual Installation
The standard-way of installing KeePass plugins is to place them in KeePass's Plugin folder under `C:\Program Files (x86)\KeePass Password Safe 2\plugins`. [Download the PLGX-File]({{ site.github.latest_release.assets[0].browser_download_url }}) and copy it to that folder.

_Remarks:_ The preferred distribution is the PLGX-File. The [ZIP-File]({{ site.github.latest_release.assets[1].browser_download_url }}) is only needed for special cases. If you have demand for it you have to unpack the downloaded file in the plugins folder under `C:\Program Files (x86)\KeePass Password Safe 2\plugins` first.

## Automatic Installation using Chocolatey/Scoop
[Chocolatey](http://chocolatey.org/) is a package manager for Windows. To install KeeAnywhere (and KeePass, if not present) just type `choco install keepass-plugin-keeanywhere` in a command shell. 

[Scoop](https://scoop.sh/) is an [alternative to Chocolatey](https://github.com/lukesampson/scoop/wiki/Chocolatey-Comparison) on Windows. To install KeeAnywhere with Scoop, type `scoop install keepass-plugin-keeanywhere` in PowerShell. 


# Verify correct installation
After installation please start KeePass. It now compiles KeeAnywhere. If it fails, an error occurs. In this case please follow [Troubleshooting](troubleshooting). 

Verify that all conditions are met:
* Under `Tools - Plugins...` and entry for KeeAnywhere exists
* The menu entry `Tools - KeeAnywhere Settings...` exists
* The menu entry `File - Open - Open from Cloud Drive...` exists