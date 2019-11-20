---
title: Contribute 
description: Giving back to the community 
---
Everyone is invited to contribute - every help is welcome.

How can **you** help:
* You are a user? Test, test, test and [write bug reports]({{ site.github.issues_url }}).
* You have an idea? [File a feature request](({{ site.github.issues_url }})) (pleas take a look at the Blacklisted Features first).
* You can code? Read the next section, fork the respository, complete a feature request/fix a bug and create a pull request.
* You like KeeAnywhere and are willing to spend some money: [Donate](donate)


# Blacklisted Features 
* Saving provider credentials in a KeePass database
* Cloud Storage Providers where I have to pay for the usage of KeeAnywhere (traffic, developer account, etc.).
* Support for Private Cloud solutions (for which I have to manage my own instance)
* All Providers that already support WebDav: KeePass natively supports WebDav (without File Picker, I know :-))

Especially `OwnCloud` and `Pydio` both are covered by the last two topics.

# Source Code Contribution
Source code contribution is welcome. Please keep in mind that this project has a vision and that your contribution should fit to this. It would be the best to talk about new ideas first (creating a new issue).

## General Rules
1. Take a look at the list of "blacklistet features"
2. Take a look at the issues, maybe your idea is already discussed
3. Fork the repository and create a pull request
4. At least I will decide, whether your pull request is accepted - therefore please discuss your ideas first. If I don't integrate your code you can use the whole source code for your own project. It's licensed under the MIT license.

## New Cloud Storage Provider
If you like to implement a new provider, please use the official SDK at first choice. I prefer all dependencies as NuGet packages from the official NuGet.org site. 

Keep in mind that simplicity for the user is the highest goal. So avoid complicated dialogs, the need to write documentation etc.

If an app registration is required (like for OAuth2 authentication) please keep in mind that I need the possibility to get an app key by myself. Providers for which I have to pay for traffic, a developer account oder something like this is not acceptable (this project is developed in my spare time).

**Never** commit production keys/secrets. Always use temporary keys for development - the production keys are kept secret by me.


