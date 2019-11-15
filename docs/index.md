---
layout: default
---

KeeAnywhere is a [KeePass Password Safe](http://keepass.info) plugin that provides access to cloud storage providers (cloud drives). The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

Supported providers (in alphabetical order):
* Amazon Drive (Experimental: see reason [here](#known-noteworthy-issues))
* Amazon AWS S3
* Box
* Dropbox
* Google Drive
* HiDrive
* hubiC
* OneDrive

![KeeAnywhere in Action](images/KeeAnywhere_Teaser.png)


# Documentation
* [What is KeeAnywhere?](https://github.com/Kyrodan/KeeAnywhere/wiki)
* [Getting Started](https://github.com/Kyrodan/KeeAnywhere/wiki/Getting-Started)
* [FAQ](https://github.com/Kyrodan/KeeAnywhere/wiki/FAQ)
* [Contributing](https://github.com/Kyrodan/KeeAnywhere/wiki/Contributing)
* [What's new?](https://github.com/Kyrodan/KeeAnywhere/blob/master/CHANGELOG.md)
* [Donate](donate)
* [Privacy Statement](privacy)


# Prerequisites
* KeePass 2.40 or newer
* Windows 7 or newer
* .Net Framework 4.5.2 or newer


# Known noteworthy issues
* Amazon Drive: Due to unclear Amazon Policy this Provider may stop working at any time. Please do not rely on this functionality!
* Dropbox: Registering a Dropbox account on Windows 7 opens the default browser at the end of the registration process. This window/tab could be ignored/closed. See details in [#13](https://github.com/Kyrodan/KeeAnywhere/issues/13)
* KpScript is not supported, because it does not support plugins. See details in [#66](https://github.com/Kyrodan/KeeAnywhere/issues/66)


# License
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE).