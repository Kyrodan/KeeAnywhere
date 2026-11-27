---
title: Privacy Statement
description: Your privacy is respected!
---
*Last updated: 2026-11-27*

## Data Usage, Advertisements, Costs
KeeAnywhere and me - the author - respects your privacy. I do not collect, store, transmit or use any of your data. KeeAnywhere is free from advertisements. You can use KeeAnywhere free of any costs or license fees.

## Granting Access
To grant KeeAnywhere access to your cloud provider, KeeAnywhere opens the login page of your cloud provider in your default web browser, where you enter your username and password. KeeAnywhere never sees the entered username and password - they are neither used nor stored by KeeAnywhere in any way in any place. If you log in to your cloud provider and allow KeeAnywhere to access your data, a unique access token is created and transmitted to KeeAnywhere. This access token needs to be stored for loading and saving your files (especially your KeePass Password Safe databases). The storage location and encryption level for this access token depends on your selection of the account storage location. Use 'Local User Secure Store' for maximum possible security. For most storage providers the access token has a limited validity and expires after a time period if not used. It is also possible to revoke access for KeeAnywhere in all cloud providers (see [Revoking Access](#revoking-access)).

For storage providers using access keys instead of a login (e.g. Amazon S3, Azure, Google Cloud Storage), the entered keys are stored in the same way as the access tokens.

## Your Data in Cloud Storage

This section applies to all storage providers.

### Data Usage

Your data is used only to provide KeeAnywhere's features you use: selecting, opening and saving files in your cloud storage, and creating backup copies of your databases (in your cloud storage next to the database, or on your computer) if you enable this feature. KeeAnywhere only reads or writes a file when you open, save or select it - it does not scan, analyze or index your cloud storage. Your data is not used for advertising, profiling or any other purpose.

### Data Storage

KeeAnywhere runs entirely on your computer. Your files are transferred directly between your computer and your cloud provider - there is no server of KeeAnywhere or its author in between.

- The access token or access keys are stored on your computer as described in [Granting Access](#granting-access).
- If you enable the offline cache or local backups, copies of your files are stored on your computer only.
- No data is stored anywhere else.

### Data Sharing

Your data is not shared with, transferred to or sold to the author of KeeAnywhere or any third party. The author has no access to your cloud accounts, your files, your access tokens or your access keys.

### Revoking Access

Removing an account in the KeeAnywhere settings deletes the stored access token or access keys from your computer. Additionally, you can revoke KeeAnywhere's access at any time at your cloud provider:

| Provider | Where to revoke access |
|---|---|
| Box | Box web interface: *Account Settings - Apps* |
| Dropbox | [Connected apps](https://www.dropbox.com/account/connected_apps) |
| Google Drive | [Third-party apps & services](https://myaccount.google.com/connections) |
| HiDrive | HiDrive web interface: account settings |
| OneDrive | Personal account: [Apps and services](https://account.live.com/consent/Manage), work or school account: [My Apps](https://myapps.microsoft.com) |
| Amazon S3, Azure, Google Cloud Storage | Delete or rotate the access key or SAS token in the provider's console |

## Permissions per Provider

KeeAnywhere requests only the permissions needed to browse your storage and to open and save your files.

| Provider | Requested permissions | Purpose |
|---|---|---|
| Box | Read and write files and folders | Browse folders, open and save files |
| Dropbox | `account_info.read`, `files.metadata.read`, `files.metadata.write`, `files.content.read`, `files.content.write` | Identify and name the account; browse folders, open and save files, create and rotate backups |
| Dropbox (restricted) | as Dropbox, but limited to the app folder `Apps/KeeAnywhere` | as Dropbox |
| Google Drive | see [Google User Data](#google-user-data) | |
| HiDrive | User role, read and write | Browse folders, open and save files |
| OneDrive | `openid`, `profile`, `offline_access`, `Files.ReadWrite` | Identify and name the account; stay signed in without logging in again; browse all folders of your OneDrive, open and save files |
| Amazon S3, Azure, Google Cloud Storage | the permissions of the access key or SAS token you enter | Browse buckets/containers, open and save files |

## Google User Data

This section describes how KeeAnywhere accesses, uses, stores and shares Google user data when you add a Google Drive account.

### Data Accessed

KeeAnywhere requests the following permissions (scopes):

- **Basic profile information** (`openid`, `profile`): your Google account ID and name. They are used to identify the account and to name it in the KeeAnywhere settings.
- **Google Drive files**:
  - Account type *Google Drive*: `https://www.googleapis.com/auth/drive` - to browse your Google Drive (including folders and files created by other apps, e.g. KeePass2Android or the Google Drive web interface) and to open and save the files you select, especially your KeePass databases.
  - Account type *Google Drive (restricted)*: `https://www.googleapis.com/auth/drive.file` - only the files created or opened with KeeAnywhere.

### Data Usage, Storage and Sharing

Google user data is handled as described in [Your Data in Cloud Storage](#your-data-in-cloud-storage):

- It is used only to select, open and save the files in your Google Drive you choose, and to create backup copies of your databases if you enable this feature. It is not used for advertising, profiling or any other purpose.
- It is transferred directly between your computer and Google and stored only on your computer (access token, offline cache, local backups) - there is no server of KeeAnywhere or its author in between.
- It is not shared with, transferred to or sold to the author of KeeAnywhere or any third party.
- You can revoke the access at any time under [Third-party apps & services](https://myaccount.google.com/connections).

### Limited Use

KeeAnywhere's use and transfer of information received from Google APIs to any other app will adhere to the [Google API Services User Data Policy](https://developers.google.com/terms/api-services-user-data-policy), including the Limited Use requirements.
