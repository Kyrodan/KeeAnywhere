---
title: Frequently Asked Questions
---
# KeeAnywhere General

## What is KeeAnywhere **not**?
KeeAnywhere is **not** another synchronization plugin. KeePass itself provides many ways of synchronization so this topic is out of scope. In my opinion the user does not want synchronization in first place: the user wants simple access to his databases in the cloud. Offline access to your databases and backups are supported, though - see [Offline Caching](advanced_topics#offline-caching) and [Automatic Backup](advanced_topics#automatic-backup). How KeeAnywhere accesses your cloud storage is explained in [Advanced Topics](advanced_topics#how-it-works). The vision is to create THE ULTIMATE CLOUD PLUGIN for KeePass.

## Why another plugin?
Many so called "sync-plugins" encapsulate the access to cloud drives under the hood: you have to keep a local copy your database and configure a synchronization relationship. While this is at first glance a simple solution it lacks advanced features (especially because of the missing possibility to access a database via a URL). Additionally most of these plugins provide a user interface for picking a database from the cloud drive. So the simple approach leads to the need to read documentation at first.

On the other side there is KeeCloud which offers URL-Style database paths. While also missing a database picker user interface this plugin integrates well in KeePass' core functions. But to configure this plugin a technical knowledge is assumed. Therefore this plugin is hardly usable for not so experienced users and lacks full multi-account support.

## Why not use the native client's folders directly in KeePass without a plugin?
Modern cloud drives are more than just a simple file storage. Many of them provide file versioning and backups. The way KeePass standalone handles file integrity compromises these goals: it is creating a temporary file, deleting the original (production) and renaming the temporary file to the new production file. This is called "transacted file writes" and can be configured in options. While this is a very good behavior for any kind of non-versioning file systems, it is suboptimal for modern cloud drives.

Additionally most of the cloud provider's native clients do not support multiple accounts.

## Does KeeAnywhere work with portable KeePass (without installation)?
Yes! Starting with version 1.5.0 KeeAnywhere can be used portable. All settings are stored beside KeePass.exe.

## What happens if the connection is interrupted while opening or saving? {#timeout}
KeePass waits while a database is opened or saved. If the connection to the cloud storage is interrupted, KeeAnywhere cancels the request after **5 minutes** and shows an error message (before version 2.3.0 KeePass could wait forever for some storage providers).

* With the Offline Cache enabled (`Tools - KeeAnywhere Settings - General`), the database is saved locally anyway and uploaded with the next successful save.
* Without Offline Cache the database stays open in KeePass - just save it again when the connection is back.

If you want KeePass to give up earlier, you can set a shorter timeout per database URL: open the database with `File - Open - Open URL...`, enter the KeeAnywhere URL (e.g. `dropbox:///Account/Database.kdbx`) and set the property `Timeout` (in milliseconds, e.g. `30000` for 30 seconds) in the advanced settings. KeePass remembers it for this URL in the list of recently used files. Values above 5 minutes have no effect.

## Can you implement private cloud providers (Nextcloud, ownCloud, homelab, etc.)?
No, I won't: KeeAnywhere supports public cloud storage providers only (see [Contribute](/contribute#public-cloud-providers-only)). But you can use your self-hosted storage anyway:
* Most of these solutions support WebDAV, which is natively supported by KeePass. Okay, I know, the file picker is missing then ;-)
* With an S3 compatible server (e.g. MinIO) you can use KeeAnywhere's `S3 Object Storage` provider (up to version 2.3: `Amazon S3`) - including the file picker. See [How do I connect S3 compatible storage](#s3-compatible) below.

## How can I contribute?
Everyone is invited to [contribute](/contribute) - every help is welcome.

## Under which license is KeeAnywhere licensed?
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE). The compiled binaries can be used free of charge without any warranty.

# Amazon AWS S3

## How do I connect S3 compatible storage (MinIO, Garage, ...)? {#s3-compatible}
Use the provider `S3 Object Storage` (up to version 2.3: `Amazon S3`) - it works with public S3 compatible services as well as with self-hosted S3 servers (e.g. MinIO, Garage, SeaweedFS, Ceph):
1. Go to `Tools - KeeAnywhere Settings...`, click `Add...` and choose `S3 Object Storage` (up to version 2.3: `Amazon S3`).
2. Enter `Access Key` and `Secret Key` of your S3 user.
3. Under `Endpoint` select `Other URL` and enter the URL of your server, e.g. `https://minio.example.lan:9000`.
4. If your server expects a specific region for signing requests, enter it under `Override Auth Region` (e.g. `us-east-1`, which is the default of MinIO).
5. Click `Test`.

_Remarks:_
* KeeAnywhere uses path-style requests (`https://server/bucket/file`), so no DNS setup for buckets is required.
* With HTTPS the server certificate must be trusted by Windows. If your server uses a self-signed certificate or your own CA, import the (CA) certificate into the Windows certificate store - otherwise the connection fails.

## How to restrict access (not giving S3FullAccess Policy) with IAM Provider?
Here is a working sample - see details in [#187](https://github.com/Kyrodan/KeeAnywhere/issues/187):
```
{ 
    "Version": "2012-10-17", 
    "Statement": [ 
        { 
            "Effect": "Allow", 
            "Action": [ "s3:ListBucketVersions", "s3:ListBucket" ], 
            "Resource": "arn:aws:s3:::my-bucket-name" 
        }, 
        { 
            "Effect": "Allow", 
            "Action": [ "s3:PutObject", "s3:GetObject", "s3:GetObjectVersionTagging", "s3:DeleteObject", "s3:GetObjectVersion" ], 
            "Resource": "arn:aws:s3:::my-bucket-name/*" 
        }, 
        { 
            "Effect": "Allow", 
            "Action": "s3:ListAllMyBuckets", "Resource": "*" 
        } 
    ] 
}
```

# Azure

## How can I use a SAS token instead of the account key for Azure Blob storage? {#azure-sas}
The account key grants full access to the complete storage account. With a SAS token (Shared Access Signature) access can be restricted to a single container, with limited permissions and an expiry date:
1. Create a SAS token for your container (e.g. in the Azure Portal: `Storage account - Containers - <your container> - Shared access tokens`, preferably based on a stored access policy so it can be revoked). Required permissions: Read, Add, Create, Write, Delete and List.
2. In KeeAnywhere go to `Tools - KeeAnywhere Settings...`, click `Add...` and choose `Azure Blob Storage`.
3. Enter the name of your storage account, paste the SAS token (or the complete SAS URL) into `Access Key or SAS` and enter the container name as `Blob Path`.

_Remarks:_
* The container must already exist - KeeAnywhere cannot create it with a SAS token.
* When the SAS token has expired, create a new one and add the account again with the same storage account and container - only the token is updated then.
* SAS tokens are supported for Azure Blob storage only (not for Azure File storage).

# Google Drive

## What is the difference between `Google Drive` and `Google Drive (restricted)`?
`Google Drive` has full access to your complete Google Drive.
`Google Drive (restricted)` can only access files that were created (or opened) with KeeAnywhere. Files you uploaded with the browser or another app are **not visible** in KeeAnywhere - this is by design of Google's restricted access mode (`drive.file` scope), not a bug.

To use an existing database with `Google Drive (restricted)`:
1. Open your database from a local copy (e.g. download it from Google Drive first).
2. Use `File - Save As - Save to Cloud Drive...` and select your `Google Drive (restricted)` account.
3. From now on open this new file with `File - Open - Open from Cloud Drive...`.

## Google shows "This app is blocked" when adding an account
Google currently blocks the full access mode (`Google Drive`) for KeeAnywhere for many users, because Google requires an app verification for full access that hasn't been completed yet - see [#438](https://github.com/Kyrodan/KeeAnywhere/issues/438) for details and the current state. Please use `Google Drive (restricted)` instead - see the question above how to use an existing database with it.

## Error 403: Request had insufficient authentication scopes
On Google's consent page each permission has its own checkbox. If the checkbox for the access to Google Drive (e.g. "See, edit, create, and delete only the specific Google Drive files you use with this app") is not enabled, the account can be added, but every access fails with this error.

Solution: remove the account in `Tools - KeeAnywhere Settings`, add it again and enable the checkbox for Google Drive access. Starting with version 2.3.0 KeeAnywhere detects this when adding the account.

# Dropbox

## What is the difference between `Dropbox` and `Dropbox-Restricted`?
From KeeAnywhere's perspective Dropbox can be accessed in two modes: `Full Access` and `App`.
`Full Access` means, that KeeAnywhere can read and write to your complete Dropbox (even other Appfolders).
`App` means, that KeeAnywhere can only read and write from and to its own Appfolder within your Dropbox. With this option the access is restricted to its own data only and does not affect other data stored in your Dropbox.

*Remarks:* If you use another client (e.g. KeePass2Android on your smartphone) that also uses the restricted `App` mode, KeeAnywhere cannot read its data in restricted mode - you have to use an (unrestricted) Dropbox account in at least one of these Apps.

# OneDrive

## What is the difference between `OneDrive` and `OneDrive (restricted)`? {#onedrive-restricted}
`OneDrive` has access to all files of your OneDrive (including shared folders added to *My files*).
`OneDrive (restricted)` (available starting with version 2.4.0) has access to its own app folder `Apps/KeeAnywhere` only - the rest of your OneDrive is not accessible for KeeAnywhere. The folder is created when you add the account; move your database into it to use it.

OneDrive doesn't allow to grant access to a folder of your choice - the app folder is the only option. Shared folders can't be used with `OneDrive (restricted)`.

## How do I open a database someone shared with me? {#onedrive-shared}
KeeAnywhere shows the content of *My files* in your OneDrive. Items that only appear in the *Shared* view of OneDrive aren't visible - Microsoft has deprecated the interface behind this view, so KeeAnywhere doesn't offer it.

To use a shared database, add the shared **folder** to your own OneDrive:
1. The owner shares the **folder** that contains the database (`.kdbx`), not only the file itself - with edit permission, if you want to save changes.
2. Open OneDrive on the web, go to *Shared*, right-click the folder and choose `Add shortcut to My files`.
3. In KeePass choose `File - Open - Open from Cloud Drive...` and select your OneDrive account. The shared folder appears in the root of your OneDrive - open the database from there.

*Remarks:* OneDrive offers shortcuts for folders only, so a single shared file can't be opened this way - share the containing folder instead.

