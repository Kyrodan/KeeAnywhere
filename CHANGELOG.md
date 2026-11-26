## 2.3.0 (unreleased)

### New

### Fixed

- \#424, \#475 Amazon S3: saving to S3 compatible storage failed (e.g. Cloudflare R2, Synology C2)
- \#469 Amazon S3: Backup to Remote did not remove old backups if the database is stored in a folder
- \#351 Azure: adding a second Blob/File storage account replaced the first one ("This account already exists")
  _Note:_ Azure accounts added with an older version are not recognized when added again (e.g. to update the access key) - a second account is created instead. Please remove the old account in `Tools - KeeAnywhere Settings` in this case.
- \#322, \#325, \#346 Google Drive: "Invalid char in path" error when a file or folder name contains control characters (e.g. tab or newline)
- \#173 Backup: other files saved beside the database (e.g. the .xml file of KeeChallenge) were backed up as .kdbx files and rotated together with the database backups - backups now keep the extension of the saved file
- Backup: saving failed if old backups could not be removed, or - with "Backup to Remote" - if the file name contained characters like `"` or `|`; old remote backups of such files were not removed

### Improved

- Updated Dependencies (AWS, Dropbox)


## 2.2.1 (2026-11-24)

### Fixed

- \#429 OneDrive: Backup to Remote did not create backup files
- \#504 Google Drive: login failed with "Error 401: deleted_client" (outdated OAuth client in release 2.2.0)


## 2.2.0 (2026-11-24)

### New

- \#409 Remove old Auth2-Flow (integrated browser) - was only used for Box.com
- \#427 Fix to connect Amazon S3 compatible endpoints with custom auth region - Thanks to @ftorregrossa

### Fixed

- \#496 Plugin could not be loaded in KeePass 2.62 - Thanks to @GeheimagentNr1
- Chocolatey: detection of a portable KeePass installation (replaced deprecated Get-BinRoot)
- \#301, \#347, \#365 [Windows <10 only?] System.Exception: No unused port found! - Thanks to @Jackabomb
- \#451, \#464 NULL reference when connecting to OneDrive - Thanks to @D3-LucaPiombino
- \#459 "Index was outside the bounds of array" error in Dropbox - Thanks to @CarrieForle
- \#497 OneDrive: file path got lost when loading, saving, copying or listing files - Thanks to @Kelltomal
- \#491 Dropbox, Box: authorization URL was passed unescaped to the browser; browser sometimes showed a connection error after successful login - Thanks to @HarryHuy
- \#468, \#484 OneDrive: rotated refresh token was not stored, so the account had to be re-authorized after some time - Thanks to @Fulgan
- \#484 Box, OneDrive: after re-authorization the old credentials were still used until KeePass was restarted - Thanks to @Fulgan
- \#484 Google Drive: meaningful error message if a file is not found or the download fails - Thanks to @Fulgan
- Security: OneDrive libraries could leak Cookie/Proxy-Authorization headers on cross-host redirects ([GHSA-7j59-v9qr-6fq9](https://github.com/advisories/GHSA-7j59-v9qr-6fq9)) - updated Microsoft Graph to 5.105.0 and Kiota to 1.22.2 (replaces \#489)

### Improved

- Updated Dependencies (AWS, Box, Dropbox, Google Drive, Google Storage, OneDrive)
- Using .Net 4.8 now
- Using KeePass 2.62 now (minimum required version)
- Removed unused dependency log4net (was affected by [GHSA-4f7c-pmjv-c25w](https://github.com/advisories/GHSA-4f7c-pmjv-c25w), replaces \#482)
- \#486 Updated Google Cloud Storage dependencies to consistent versions (fixes assembly version mismatch) - Thanks to @Fulgan
- Build: OAuth API keys moved to *ApiKeys.cs*, which is not under version control (created from *ApiKeys.Template.cs* with dummy values on first build, see BUILD.md) - inspired by \#491, thanks to @HarryHuy
- Build: GitHub Actions workflow for building releases
- Build: test-plgx.ps1 for testing the PLGX with a test copy of KeePass (see BUILD.md) - based on \#486, thanks to @Fulgan
- Build: NuGet packages are restored via MSBuild, nuget.exe removed from repository


## 2.1.0 (2024-06-17)

### New

- \#269 New storage provider: Google Cloud Storage  
- \#377 Gdrive shortcuts 
- \#398 Limit access to KeeAnywhere files only and not the whole drive (like Dropbox-restricted)
- \#261 Added support for OneDrive shared folders
- \#369 Remove support for hubiC

### Fixed

- \#327 Remote backup doesn't work on google drive in the root path

### Improved

- Using KeePass 2.57.0 now


## 2.0.3 (2021-05-21)

### Fixed

- \#283 Port range for redirect URI calculated wrong


## 2.0.2 (2021-05-21)

### Fixed

- \#281 OneDrive cannot be accessed anymore
- \#280 Exception: Proxy Authentication required


## 2.0.0 (2021-05-18)

KeePass 2.48.1 or newer is required now (was 2.43).
Micrososft .Net 4.6.1 is required now (was 4.5.2).
Amazon Drive support has been dropped.
New Provider: Azure Blob and File storage - thanks to J. Peitsch (@spacedee)
Amazon S3: S3-Compatible providers can be used now (e. g. Wasabi, HiDrive S3, Ionos S3, Backblaze B2, MinIO)

### New

- \#244 Feature Request: nonAWS S3
- \#203 Adding Azure blob and file storage
- \#154 New Provider: BackBlaze B2
- \#130 Add Support to Provider Wasabi (would be included as S3 Compatible)

### Fixed

- \#246 Remove Amazon Drive support
- \#241 \[Google Drive\] Cannot Authorize (browser not supported) bug 
- \#227 "Couldn't sign you in" GDrive error bug 
- \#212 Can't connect to google drive account
- \#179 \[Dropbox\] \[Google Drive\] Open from Cloud Drive only list partial content
- \#196 HiDrive: Saving to root folder throws exception
- \#89 Reimplement Account Authentication not using internal Web Browser

### Improved

- Updated Dependencies (AWS, Box, Dropbox, Google Drive, OneDrive)
- Using KeePass 2.48.1 now
- Using .Net 4.6.1 now


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