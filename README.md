# KeeAnywhere [![GitHub version](https://badge.fury.io/gh/kyrodan%2Fkeeanywhere.svg)](https://badge.fury.io/gh/kyrodan%2Fkeeanywhere)
KeeAnywhere is a KeePass plugin that provides access to cloud storage providers (cloud drives). The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

Supported providers (in alphabetical order):
* Amazon Drive
* Dropbox
* Google Drive
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

# Prerequisites
* KeePass 2.31 or newer
* Windows 7 or newer
* .Net Framework 4.5


# Known noteworthy issues:
* Registering a Dropbox account on Windows 7 opens the default browser at the end of the registration process. This window/tab could be ignored/closed.
* Registration process for Google Drive takes place in default browser. At the end of the registration process an error occures: this does not interfere with KeeAnywhere. Just close the browser window/tab.  


# Building
Run *build.cmd* and see output in folder *build*. For further details see [BUILD.md](BUILD.md).


# License
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE).
