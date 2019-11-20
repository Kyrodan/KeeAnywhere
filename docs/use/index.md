---
layout: default
title: Use KeeAnywhere
description: Documentation starts here
---

KeeAnywhere is a KeePass 2.x plugin (2.35 or newer) that provides access to cloud storage providers (cloud drives). The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

# Key features
* Support for multiple cloud storage providers
* Support for multiple accounts for each cloud storage provider
* Open/Save databases with a feature rich user interface
* Easy and intuitive configuration
* Access to cloud stored databases via URL ("Open from URL"/"Save to URL")
* Full support of recent files list
* Deep integration into KeePass' core functions (synchroniziation, triggers, scripting, etc.)
* No need for technical configuration entries in a database
* No need to have cloud storage provider's native client be installed
* Proxy support
* Simple Automatic Backup
* Offline Caching

# Supported Providers
* Amazon Drive (Experimental)
* Amazon AWS S3
* Box
* Dropbox
* Google Drive
* HiDrive
* hubiC
* OneDrive

# Prerequisites
* KeePass 2.40 or newer
* Windows 7 or newer
* .Net Framework 4.5.2 or newer

# Known noteworthy issues
* Amazon Drive: Due to unclear Amazon Policy this Provider may stop working at any time. Please do not rely on this functionality!
* Dropbox: Registering a Dropbox account on Windows 7 opens the default browser at the end of the registration process. This window/tab could be ignored/closed. See details in [#13](https://github.com/Kyrodan/KeeAnywhere/issues/13)
* KpScript is not supported, because it does not support plugins. See details in [#66](https://github.com/Kyrodan/KeeAnywhere/issues/66)