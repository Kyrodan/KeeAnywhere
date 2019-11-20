---
title: Installation
---
# Manual Installation
The standard-way of installing KeePass plugins is to place them in KeePass's Plugin folder under `C:\Program Files (x86)\KeePass Password Safe 2\plugins`. [Download the PLGX-File](https://github.com/Kyrodan/KeeAnywhere/releases/latest) and copy it to that folder.

_Remarks:_ The preferred distribution is the PLGX-File. The ZIP-File is only needed for special cases. If you have demand for it you have to unpack the downloaded file in the plugins folder under `C:\Program Files (x86)\KeePass Password Safe 2\plugins` first.

# Automatic Installation using Chocolatey/Scoop
[Chocolatey](http://chocolatey.org/) is a package manager for Windows. To install KeeAnywhere (and KeePass, if not present) just type `choco install keepass-plugin-keeanywhere` in a command shell. 

[Scoop](https://scoop.sh/) is an [alternative to Chocolatey](https://github.com/lukesampson/scoop/wiki/Chocolatey-Comparison) on Windows. To install KeeAnywhere with Scoop, type `scoop install keepass-plugin-keeanywhere` in PowerShell. 


# Verify correct installation
After installation please start KeePass. It now compiles KeeAnywhere. If it fails, an error occurs. In this case please follow [Troubleshooting](Troubleshooting). 

Verify that all conditions are met:
* Under `Tools - Plugins...` and entry for KeeAnywhere exists
* The menu entry `Tools - KeeAnywhere Settings...` exists
* The menu entry `File - Open - Open from Cloud Drive...` exists