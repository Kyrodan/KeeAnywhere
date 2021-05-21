---
title: KeeAnywhere
description: A cloud storage provider plugin for KeePass Password Safe
---
# Noteworthy for updates from 1.x:
* Authorization now takes place in your system's default browser. After logging in and accepting the terms of your account, it redirects to "localhost" (with changing ports). It then states, that you can close this window (= tab) now.
* OneDrive-Accounts have to be deleted and recreated. Sync all offline-files *before* deleting your account!
* Dropbox-Accounts have to be re-authenticated


KeeAnywhere is a [KeePass Password Safe](http://keepass.info) plugin that provides access to cloud storage providers (cloud drives). The main goal is to offer a simple UI while integrating deeply into KeePass. This covers all kind of users: novices, experienced and power users.

![KeeAnywhere in Action](assets/images/KeeAnywhere_Teaser.png)

# Supported providers (in alphabetical order):
* Amazon AWS S3 (and compatible)
* Azure (Blob and File storage)
* Box
* Dropbox
* Google Drive
* HiDrive 
* hubiC 
* OneDrive  

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

# License
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE).