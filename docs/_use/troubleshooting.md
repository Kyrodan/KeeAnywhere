---
title: Troubleshooting
---
# Could not read account configuration
You use `Local User Secure Store` for your Account Storage Location and the following error appears:
> Unable to parse the plugin's account configuration for the KeeAnywhere plugin from local secure store:
> Decryption of screts failed.

This is beacuse the secrets could not be decrypted from the `%APPDATA%\KeePass\KeeAnywhere.Accounts.json` file.
This mostly happens if you move this file from another Windows User or another machine: in both cases
the decryption is not possible anymore because it is tightly coupled to you Windows User.

More Details:
Internally [System.Security.Cryptography.ProtectedData](https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.protecteddata?view=netframework-4.7.2) is used which itself uses the Windows Data Protection API (DPAPI).


# Proxy settings not working
For registering an account, KeeAnywhere embeds Internet Explorer in the authorization window. So you have to manage system proxy settings (in Control Panel - Internetoptions), too.

After successfully registering an account, this setting is no longer needed, as KeeAnywhere then uses it's own proxy settings (from KeePass).

This is a drawback, I know, but currently there is no simple alternative.

# Google Drive: app passwords do not work
That's right: KeeAnywhere does not support app passwords - and it's not planned to implement it :-)

The reason is simple: KeeAnywhere uses a secure authentication mechanism called `OAuth2`. This fully supports Googles authentication mechanisms, especially 2-Factor-Authorization. The password you enter is **not** stored in KeeAnywhere (or elsewhere) - it is just used (by Google login) to login to your Google account in a webbrowser and generates an app- and user-specific OAuth token (a kind of password/secret). KeeAnywhere does not know neither username nor password and it does not even know about the complete login flow (with 2-Factor-Authorization, if turned on or captchas Google likes to be entered if it detects a possible security problem).

App Passwords have similar goals like OAuth2 but security is a little bit lower: in OAuth2 the secret is not sharable with other apps and is not general usable. See details here: https://duo.com/blog/bypassing-googles-two-factor-authentication

So there is no need to create an app password for KeeAnywhere because it already supports a more secure authentication mechanism.
