---
title: Advanced Topics
---
# Which Account Storage Location should I choose?
KeeAnywhere has several options where to store account details. These details **do not** contain your password. But it contains an user specific authorization key (secret) that is created when you allow KeeAnywhere to access your cloud storage. 

Options:
* `Local User Secure Store` (_default_) stores account details in `%APPDATA%\KeePass\KeeAnywhere.Accounts.json`. All secrets are encrypted and can only be accessed by the currently logged in user (like in `Windows Credential Manager` but without limitations in length).
* `KeePass Configuration` stores account details in `KeePass.config.xml` file. This option is dedicated to "portable solutions" (= without the need to install KeeAnywhere on your computer). If you connect KeeAnywhere to a cloud provider, a token (a kind of secret) is issued by the cloud provider which - in conjunction with a secret App-ID is used to access your cloud storage. This secret is **not** your password. But: **All tokens are stored in clear text (unencrypted). Keep this in mind if you share your `KeePass.config.xml`.** This is a trade-off-decision between simple usability and lower security.
* `Windows Credential Manager` (_deprecated_) stores account details in the credential manager of your current computer. Secrets are encrypted and can only be accessed by the currently logged in user. This option is deprecated since Version 1.3.0 and cannot be used anymore: It has limitations in the length of stored secrets which gets exceeded by some providers.

# Open Database by URL
All accessible databases can be opened by its special URL. This URL can be determined e.g. via `Open from Cloud Drive...` dialog. It can be used widely in KeePass for e.g. Synchronisation, Triggers, Open URL, etc. You do not need to provide Username and Password if possible - just leave these fields blank.

Details of this URL-Schema can be found [here](https://github.com/Kyrodan/KeeAnywhere/wiki/URL-Specification).

# Rename an Account
Steps to rename an account:

1. go to `Tools - KeeAnywhere Settings...`

2. select an account by single-click

3. single-click this selected account again (like in Windows Explorer). Do not be too fast - it will not be recognized as double-click.

4. The account name changes to an input field

5. Type your changes

6. Press <Enter> to save changes; Press <Esc> to cancel changes.

# Using a Proxy
KeeAnywhere supports KeePass's Proxy settings. Go to `Tools - Options...`, select `Advanced` tab und click on `Proxy...` in the lower right corner. Here you can set your proxy settings which are also used by KeeAnywhere.

# Synchronization
If you like to synchonize two databases in different locations, you can synchronize these two databases. This works with KeePass built-in functions and is described in details [here](http://keepass.info/help/v2/sync.html)

# Automatic Backup
KeeAnywhere offers a simple automatic backup. Activate this feature in KeeAnywhere settings.
You decide, whether you want a backup in your cloude drive (online) and/or locally stored on your database (offline).
Backups automatically get rotated (with the given number of copies to keep). Online backups are generated via cloud storage API's, so the database is not downloaded and re-uploaded again (with another filename).

The name of your backup files are auto-generated and cannot be changed: they are derived from your database filename ({yourdatabasename}_Backup_{timestamp}.kdbx).

# Offline Caching
If you frequently work offline, now you can activate offline caching. Activate this feature in KeeAnywhere settings. If you now open a database from a cloud drive which is not available (because you are disconnected), the cached database is opened for you. You can even do changes and save it! This completely works transparent for you: you don't have to care of anything. 

If your internet connection is back (on opening or saving the database), the cached database will be "synced" to your cloud storage: therefore it uses KeePass's syncing-mechanisms and dialogs.
