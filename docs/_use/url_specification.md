---
title: URL Specification
---
KeeAnywhere URL's follow this sepcification:

`{provider}:///{accountname}/{path_to_database}` 

Valid values for `{provider}` are:
* adrive
* s3
* box
* dropbox
* dropbox-r
* gdrive
* hidrive
* hubic
* onedrive

The `{accountname}` is the name of the account you can see in KeeAnywhere Settings` account list.

The `{path_to_database}` specifies the relative path to your database within your account. It follows regular path syntax.

*Remarks:*
* For Amazon S3 the first path part is the bucket name. You can see all buckets, but you can only access the buckets within the region you selected during account creation.
* For Dropbox Restricted, the path is relative to the Apps/KeeAnywhere folder.  If your database file is located in Dropbox/Apps/KeeAnywhere/mydb.kdbx, your URL would be `dropbox-r:///{accountname}/mydb.kdbx`





