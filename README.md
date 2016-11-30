# KeeAnywhere [![GitHub release](https://img.shields.io/github/release/Kyrodan/KeeAnywhere.svg)](https://github.com/Kyrodan/KeeAnywhere/releases/latest) [![Chocolatey](https://img.shields.io/chocolatey/v/keepass-plugin-keeanywhere.svg)](https://chocolatey.org/packages/keepass-plugin-keeanywhere) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/Kyrodan/KeeAnywhere/master/LICENSE)
KeeAnywhere is a KeePass plugin that provides access to cloud storage providers (cloud drives). The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

Supported providers (in alphabetical order):
* ~~Amazon Drive~~ (see reason [here](#known-noteworthy-issues))
* Amazon AWS S3
* Box
* Dropbox
* Google Drive
* HiDrive
* hubiC
* OneDrive

![KeeAnywhere in Action](doc/screenshots/KeeAnywhere_Teaser.png)


# Documentation
* [What is KeeAnywhere?](https://github.com/Kyrodan/KeeAnywhere/wiki)
* [Download](https://github.com/Kyrodan/KeeAnywhere/releases)
* [Getting Started](https://github.com/Kyrodan/KeeAnywhere/wiki/Getting-Started)
* [FAQ](https://github.com/Kyrodan/KeeAnywhere/wiki/FAQ)
* [Contributing](https://github.com/Kyrodan/KeeAnywhere/wiki/Contributing)
* [What's new?](CHANGELOG.md)
* [Donate](DONATE.md)


# Prerequisites
* KeePass 2.35 or newer
* Windows 7 or newer
* .Net Framework 4.5.1 or newer


# Known noteworthy issues
* Amazon Drive: The support for Amazon Drive is completely implemented and tested. But I'm not allowed to release this because Amazon denied the App Approval Request.
* Dropbox: Registering a Dropbox account on Windows 7 opens the default browser at the end of the registration process. This window/tab could be ignored/closed.


# Building
Run *build.cmd* and see output in folder *build*. For further details see [BUILD.md](BUILD.md).


# License
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE).
