## 1.7.0 (unpublished)

### Fixed

- \#179 \[Dropbox\] \[Google Drive\] Open from Cloud Drive only list partial content
- \#196 HiDrive: Saving to root folder throws exception

### Improved

- Updated Dependencies (AWS, Box, Dropbox, Google Drive)


## 1.6.0 (2020-01-08)

KeePass 2.43 or newer is required now (was 2.40).

### New

- \#116 KeeAnywhere not works with Temporary Access and Secret Keys (PR \#182)

### Fixed

- \#123 KeeAnywhere ignore KeePass Proxy configuration (PR \#163)
- \#169 KeeAnywhere.dll v1.5.1 plugin incompatible with Keepass 2.41
- \#184 KeeAnywhere 1.5.1 doesn't work with KeePass 2.43
- \#189 Adjust links to new Homepage

### Improved

- \#99 About the beer message
- \#190 Improve Menu Creation for KeePass >= 2.41
- Updated Dependencies (KeePass)


## 1.5.1 (2018-10-19)

This is a bug-fixing release only.

### Fixed
- \#139 Fails to save big DB to remote [tested on GDrive & OneDrive]
- \#147 KeeAnywhere-1.5.0 plugin incompatible with Keepass 2.40 (Improved Library Loading)
- \#148 KeeAnywhere Offline Cache path (Improved determination of portability)
- \#152 Box: Enable TLS 1.1+


## 1.5.0 (2018-09-20)

This is mainly a bug-fixing release.
KeePass 2.40 or newer is required now (was 2.35).

Noteworthy: 
Default Storage Location for settings is now in User's Roaming AppData instead of User's Local AppData.

### Fixed

- \#90 Update OnHelpMeChooseAccountStorage uri
- \#96 default folder did not create automatically
- \#103 Caching database makes KeePass non-portable
- \#113 Adjust account storage location
- \#122 Fixes syntax in donation text
- \#138 Simple Spelling Correction
- \#141 Incompatible with KeePass 2.40

### Improved

- Updated Dependencies (AWS, Box, Dropbox, Google Drive, OneDrive)


## 1.4.1 (2017-06-02)

This is a bug-fixing release only

### Fixed

- \#86 Error when opening KeeAnywhere Settings 1.4.0
- \#87 Authentication failed with Google Drive
- \#88 Google Drive: can't open/save database to/from root folder


## 1.4.0 (2017-05-31)

This release brings two new features: Offline-Caching and Simple Automatic Backup.

KeePass 2.35 or newer is required now (was 2.31).

Micrososft .Net 4.5.2 is required now (was 4.5.1).


### New

- \#5 Cache databases for disconnected (offline) usage
- \#61 Simple Automatic Backup (Remote and/or Local)
- Activated experimental Amazon Drive support

### Fixed

- \#58 Amazon S3: Cannot save/write database file to root of bucket
- \#63 KeePass tray menu disabled and window restoration fails when account creation is canceled
- \#68 Ctrl+Alt+A Gives Error cannot access disposed object "DonationForm"
- \#74 Google Drive Authentication failed
- \#77 OneDrive: Microsoft.Graph.ServiceException

### Improved

- \#62 Allow pasting in the embedded browser
- \#72 IconHelper leaves unsed API allocated memory - Completely removed IconHelper
- Updated Dependencies
- Replaced Micrososft OneDriveSDK with GraphSDK
- Using .Net 4.5.2 now


## 1.3.1 (2017-01-02)

### Fixed

- \#67 Dropbox: can't authenticate after introduction of new Login Page


## 1.3.0 (2016-09-01)

### New

- \#10 Access to Amazon Cloud Drive
- \#11 Access to Box
- \#16 Proxy support (with auth) feature request
- \#26 AWS S3 support
- \#52 HiDrive support
- Possibility to donate

### Fixed

- \#14 Google Drive: Registering new account takes place in default browser
- \#49 After Updating to 1.2.0 account type is wrong, if using KeePass Configuration as Storage Location
- \#50 Long Refresh Tokens cause exception when saving in Windows Credential Store

### Improved

- \#2 Replace currently used OneDrive client with the official one
- \#51 Improve Authorization Dialog
- Updated Provider Client Libraries to latest Versions and .Net 4.5.1


## 1.2.0 (2016-07-28)

### New

- \#41 Settings Dialog: New Button "Check Account" enhancement
- \#24 Dropbox - access to subfolder only enhancement provider
- \#36 File Chooser: save last used account enhancement

### Fixed

- \#32 hubiC: Save with a full path name in a non-existent folder works but could not be reopened bug
- \#44 Sometimes can't load kdbx file (Dropbox, hubiC)


## 1.1.0 (2016-06-15)

### New

- \#25 Add "Save Copy to Cloud Drive" support enhancement

### Fixed

- \#40 Dropbox: Registering a new account fails bug
- \#34 Open or Save in hubiC doesn't work bug
- \#31 Incompatibility with DataBaseBackup extension: Exception during save bug
- \#23 Error when special characters are in filenames bug
- \#22 Change account name leads to exception, when not changing the name bug
- \#19 "Help me choose" help link incorrect bug



## 1.0.0 (2016-01-28)

First production ready release


## 0.2.0-alpha (2015-12-19)

Second Alpha-Release


## 0.1.0-alpha (2015-12-06)

First Alpha-Release