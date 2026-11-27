---
title: Advanced Topics
---
# Which Account Storage Location should I choose?
KeeAnywhere has several options where to store account details. These details **do not** contain your password. But it contains an user specific authorization key (secret) that is created when you allow KeeAnywhere to access your cloud storage. 

Options:
* `Local User Secure Store` (_default_) stores account details in `%APPDATA%\KeePass\KeeAnywhere.Accounts.json`. All secrets are encrypted and can only be accessed by the currently logged in user (like in `Windows Credential Manager` but without limitations in length).
* `KeePass Configuration` stores account details in `KeePass.config.xml` file. This option is dedicated to "portable solutions" (= without the need to install KeeAnywhere on your computer). If you connect KeeAnywhere to a cloud provider, a token (a kind of secret) is issued by the cloud provider which - in conjunction with a secret App-ID is used to access your cloud storage. This secret is **not** your password. But: **All tokens are stored in clear text (unencrypted). Keep this in mind if you share your `KeePass.config.xml`.** This is a trade-off-decision between simple usability and lower security.
* `Windows Credential Manager` (_deprecated_) stores account details in the credential manager of your current computer. Secrets are encrypted and can only be accessed by the currently logged in user. This option is deprecated since Version 1.3.0 and cannot be used anymore: It has limitations in the length of stored secrets which gets exceeded by some providers.

With `Local User Secure Store` the accounts belong to the current **Windows user**: if several Windows users (or computers) use the same KeePass installation, each of them has to add the account in KeeAnywhere on their own. For a shared (portable) installation you can use `KeePass Configuration` instead - keep in mind that the tokens are stored unencrypted then.

# Open Database by URL
All accessible databases can be opened by its special URL. This URL can be determined e.g. via `Open from Cloud Drive...` dialog. It can be used widely in KeePass for e.g. Synchronisation, Triggers, Open URL, etc. You do not need to provide Username and Password if possible - just leave these fields blank.

Details of this URL scheme can be found in the [URL Specification](url_specification).

# Opening databases automatically (KeeAutoExec, triggers, command line) {#open-automatically}
When KeePass opens a URL automatically, it may show the `Open URL` dialog or ask for server credentials, although KeeAnywhere doesn't need any (this behavior is part of KeePass, not of KeeAnywhere). Depending on how the database is opened:

* **KeeAutoExec plugin:** add the string fields `IocUserName` and `IocPassword` with any non-empty value (e.g. `.`) to the AutoOpen entry - KeeAutoExec then skips the dialog.
* **Command line or shortcut:** add the parameter `-ioiscomplete`, e.g. `KeePass.exe "dropbox:///Account/Database.kdbx" -ioiscomplete`.
* **Trigger:** instead of the action `Open database file` use `Execute command line / URL` with `KeePass.exe` as file and `"<KeeAnywhere URL>" -ioiscomplete` as arguments.

# Rename an Account
Steps to rename an account:

1. go to `Tools - KeeAnywhere Settings...`

2. select an account by single-click

3. single-click this selected account again (like in Windows Explorer). Do not be too fast - it will not be recognized as double-click.

4. The account name changes to an input field

5. Type your changes

6. Press <Enter> to save changes; Press <Esc> to cancel changes.

# Using a Proxy
KeeAnywhere supports KeePass's Proxy settings. Go to `Tools - Options...`, select `Advanced` tab and click on `Proxy...` in the lower right corner. Here you can set your proxy settings which are also used by KeeAnywhere.

# How KeeAnywhere accesses your cloud storage {#how-it-works}
* **When:** KeeAnywhere only accesses your cloud storage when KeePass asks for it - when you open, save or synchronize a database (also via URL, triggers or other plugins) and when you browse in the `Open from Cloud Drive...` dialog. Nothing happens in the background.
* **Connection:** There is no permanent connection. Each operation consists of single HTTPS requests to the provider's API. KeeAnywhere doesn't store your password - only an authorization token, which is used to request a short-lived access token when needed (for S3 and Azure: the access key).
* **Opening:** The complete database file is downloaded.
* **Saving:** Before saving, KeePass checks whether the database in the cloud has been changed in the meantime (e.g. on another computer) and offers to synchronize (merge) both versions. Then the complete database file is uploaded and replaces the file in the cloud - so a versioning of your provider keeps the previous versions. Afterwards KeePass reads the file again to verify it.
* **Merging** of two versions only happens via KeePass' synchronization (see [Synchronization](#synchronization)), never automatically by KeeAnywhere.
* **Offline Cache** and **Automatic Backup** (if enabled) add further steps when opening and saving - see [Offline Caching](#offline-caching) and [Automatic Backup](#automatic-backup).

# Synchronization
If you like to synchronize two databases in different locations, you can synchronize these two databases. This works with KeePass built-in functions and is described in details [here](https://keepass.info/help/v2/sync.html)

# Automatic Backup
KeeAnywhere offers a simple automatic backup. Activate this feature in KeeAnywhere settings.
You decide, whether you want a backup in your cloud drive (online) and/or locally stored on your database (offline).

Backups are created **each time you save** the database:

* **Online (remote) backup:** before the new version is uploaded, the previous file is copied *in your cloud storage* (via the provider's API, so the database is not downloaded and uploaded again).
* **Local backup:** the saved database is copied to the local backup folder (one subfolder per account) - the previous copy is renamed to a backup file.

Backups automatically get rotated: only the given number of copies is kept.

The name of your backup files are auto-generated and cannot be changed: they are derived from your database filename ({yourdatabasename}_Backup_{timestamp}.kdbx - other files saved beside the database keep their extension).

# Offline Caching
If you frequently work offline, now you can activate offline caching. Activate this feature in KeeAnywhere settings. If you now open a database from a cloud drive which is not available (because you are disconnected), the cached database is opened for you. You can even do changes and save it! This completely works transparent for you: you don't have to care of anything. 

How it works:

* **Opening:** if your cloud storage is reachable, the database is downloaded and stored in the cache. Otherwise the cached database is opened and a message is shown.
* **Saving:** the database is saved to the cache first, then uploaded. If the upload fails, a message is shown - the changes are safe in the cache.
* **Back online:** on the next save, or when opening a database whose cloud version is unchanged, the cached changes are uploaded to your cloud storage.
* **Conflict:** if both the cached and the cloud version have been changed, KeeAnywhere opens the cached version and both versions have to be merged once - see [Troubleshooting](troubleshooting#offline-cache-conflict-opening-database).
