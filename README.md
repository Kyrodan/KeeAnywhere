# KeeAnywhere
KeeAnywhere is a KeePass plugin that provides access to cloud storage providers (cloud drives) like Dropbox, Google Drive or OneDrive. The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

![KeeAnywhere in Action](doc/screenshots/KeeAnywhere_Teaser.png)


# Getting started
* [What is KeeAnywhere?](https://github.com/Kyrodan/KeeAnywhere/wiki)
* [Screenshots](https://github.com/Kyrodan/KeeAnywhere/wiki/Screenshots)
* [Download](https://github.com/Kyrodan/KeeAnywhere/releases)


# Supported Providers
* Dropbox
* Google Drive
* OneDrive


# Status
This project is in very early state and **NOT** production ready. It depends on unreleased KeePass functions which are only available in snapshot builds. KeePass 2.31 is planned to be released in erly 2016. After this, KeeAnywhere 1.0 will be released contemporary.

Known noteworthy issues:
* Registering a Dropbox account on Windows 7 opens the default browser at the end of the registration process. This window/tab could be ignored/closed.
* Registration process for Google Drive takes place in default browser. At the end of the registration process an error occures: this does not interfere with KeeAnywhere. Just close the browser window/tab.  


# Building
Run *build.cmd* and see output in folder *build*. For further details see [BUILD.md](BUILD.md).


# Contributing
Contributing is welcome. Please read [Source Code Guidelines](https://github.com/Kyrodan/KeeAnywhere/wiki/Source-Code-Guidelines) first.


# License
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE)
