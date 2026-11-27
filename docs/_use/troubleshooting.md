---
title: Troubleshooting
---
# Plugin cannot be loaded after an update
After updating, KeePass reports that KeeAnywhere cannot be loaded, e.g. with a message like
> Method 'get_AuthenticationProvider' in type 'Microsoft.Graph.GraphServiceClient' ... does not have an implementation.

or another `TypeLoadException`, `MissingMethodException` or `FileLoadException` mentioning a library (e.g. `Microsoft.Graph`).

This happens if files of an older version (especially of the ZIP file) are still present in the `Plugins` folder or in the KeePass program folder: KeePass then loads old libraries. Remove all old files and clear the plugin cache as described in [Updating from an older version](installation#updating).

# Could not read account configuration
You use `Local User Secure Store` for your Account Storage Location and the following error appears:
> Unable to parse the plugin's account configuration for the KeeAnywhere plugin from local secure store:
> Decryption of secrets failed.

This is because the secrets could not be decrypted from the `%APPDATA%\KeePass\KeeAnywhere.Accounts.json` file.
This mostly happens if you move this file from another Windows User or another machine: in both cases
the decryption is not possible anymore because it is tightly coupled to your Windows user.

More Details:
Internally [System.Security.Cryptography.ProtectedData](https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.protecteddata?view=netframework-4.7.2) is used which itself uses the Windows Data Protection API (DPAPI).


# Proxy settings not working
Typical symptom: adding an account in the browser works, but every access from KeePass fails with an error like `Unable to connect to the remote server`. Then your browser uses a proxy, but KeePass doesn't: set the proxy in KeePass (`Tools - Options - Advanced - Proxy...`, see [Advanced Topics](advanced_topics)) - KeeAnywhere uses these settings.

For registering an account, KeeAnywhere uses system's default browser for authorization. So you have to manage its proxy settings, too.

After successfully registering an account, this setting is no longer needed, as KeeAnywhere then uses its own proxy settings (from KeePass).

This is a drawback, I know, but currently there is no simple alternative.

# Offline Cache: Conflict opening database
This message appears if both the offline cached database and the database in your cloud storage have been changed since the last synchronization (e.g. you saved offline on this computer and online on another one). KeeAnywhere then opens the offline cached version - the changes in the cloud are **not** contained.

To merge both versions:
1. Download the database from your cloud storage (e.g. with the web browser).
2. In KeePass (with the database opened via KeeAnywhere) use `File - Synchronize - Synchronize with File...` and select the downloaded file.
3. Save the database - the merged database is uploaded to your cloud storage.

# Google Drive: app passwords do not work
That's right: KeeAnywhere does not support app passwords - and it's not planned to implement it :-)

The reason is simple: KeeAnywhere uses a secure authentication mechanism called `OAuth2`. This fully supports Googles authentication mechanisms, especially 2-Factor-Authorization. The password you enter is **not** stored in KeeAnywhere (or elsewhere) - it is just used (by Google login) to login to your Google account in a webbrowser and generates an app- and user-specific OAuth token (a kind of password/secret). KeeAnywhere does not know neither username nor password and it does not even know about the complete login flow (with 2-Factor-Authorization, if turned on or captchas Google likes to be entered if it detects a possible security problem).

App Passwords have similar goals like OAuth2 but security is a little bit lower: in OAuth2 the secret is not sharable with other apps and is not general usable. See details here: https://duo.com/blog/bypassing-googles-two-factor-authentication

So there is no need to create an app password for KeeAnywhere because it already supports a more secure authentication mechanism.
