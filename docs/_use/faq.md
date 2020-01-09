---
title: Frequently Asked Questions
---
# KeeAnywhere General

## What is KeeAnywhere **not**?
KeeAnywhere is **not** another synchronization plugin. KeePass itself provides many ways of synchronization so this topic is out of scope. In my opinion the user does not want synchronization in first place: the user wants simple access to his databases in the cloud. Advanced users may request disconnected (offline) access to his databases (with later syncing to the cloud) or have demand for a database backup strategy: these are topics I keep in mind and resepct in ongoing development. The vision is to create THE ULTIMATE CLOUD PLUGIN for KeePass.

## Why another plugin?
Many so called "sync-plugins" encapsulate the access to cloud drives under the hood: you have to keep a local copy your database and configure a synchronization relationship. While this is at first glance a simple solution it lacks advanced features (especially beacuse of the missing possibility to access a database via a URL). Additionally most of these plugins provide a user interface for picking a database from the cloud drive. So the simple approach leads to the need to read documentation at first.

On the other side there is KeeCloud which offers URL-Style database paths. While also missing a database picker user interface this plugin integrates well in KeePass' core functions. But to configure this plugin a technical knowledge is assumed. Therefore this plugin is hardly usable for not so experienced users and lacks full multi-account support.

## Why not use the native client's folders directly in KeePass without a plugin?
Modern cloud drives are more than just a simple file storage. Many of them provide file versioning and backups. The way KeePass standalone handles file integrity compromises these goals: it is creating a temporary file, deleting the original (production) and renaming the temporary file to the new production file. This is called "transacted file writes" and can be configured in options. While this is a very good behavior for any kind of non-versioning file systems, it is suboptimal for modern cloud drives.

Additionally most of the cloud provider's native clients do not support multiple accounts.

## Does KeeAnywhere works with portable KeePass (without installation)
Yes! Starting with version 1.5.0 KeeAnywhere can be used portable. All settings are stored beside KeePass.exe.

## Can you implement private cloud provider (OwnCloud, Nextcloud, etc.)
No, I won't. Most of these providers support native WebDav which is also natively supported by KeePass. Okay, I know, the file picker is missing then ;-)

## How can I contribute?
Everyone is invited to [contribute](/contribute) - every help is welcome.

## Under which license is KeeAnywhere licensed?
The source code is licensed under the [MIT license](https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE). The compiled binaries can be used free of charge without any warranty.

# Amazon AWS S3

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

# Dropbox

## What is the difference between `Dropbox` and `Dropbox-Restricted`
From KeeAnywhere's perspective Dropbox can be accessed in two modes: `Full Access` and `App`.
`Full Access` means, that KeeAnywhere can read and write to your complete Dropbox (even other Appfolders).
`App` means, that KeeAnywhere can only read and write from and to it's own Appfolder within your Dropbox. With this option the access is restricted to it's own data only and does not affect other data stored in your Dropbox.

*Remarks:* If you use another client (e.g. KeePass2Android on your smartphone) that also uses the restricted `App` mode, KeeAnywhere cannot read it's data in restricted mode - you have to use an (unrestricted) Dropbox account in at least one of these Apps.

