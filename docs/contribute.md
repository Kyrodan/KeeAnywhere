---
title: Contribute
description: Giving back to the community
---
Everyone is invited to contribute - every help is welcome.

How can **you** help:
* You are a user? Test, test, test and [report bugs](#reporting-bugs) - especially for [community-driven providers](#community-driven-providers), which I don't test myself.
* You have an idea? [File a feature request]({{ site.github.issues_url }}) - but please take a look at the [out of scope features](#out-of-scope-features) first.
* You can code? Read [Source Code Contribution](#source-code-contribution), fork the repository, fix a bug or implement a feature request and create a pull request.
* You found a typo or something outdated in this documentation? Corrections are welcome as well - see [Documentation](#documentation).
* You like KeeAnywhere and are willing to spend some money: [Donate](donate)

# Be Respectful
KeeAnywhere is developed in my spare time. Please keep issues, pull requests and discussions friendly and constructive - also when something doesn't work as expected:
* Be respectful to everyone - no insults, harassment or discrimination.
* Stay on topic. Instead of "+1" or "me too" comments, please use the 👍 reaction - or add new information that helps to solve the problem.
* There is no guaranteed support or response time.

Comments that don't follow these rules may be hidden or deleted, and discussions may be locked.

# Reporting Bugs
Before creating a new issue:
* Please check the [FAQ](use/faq), [Known Issues](use/known_issues) and [Troubleshooting](use/troubleshooting).
* Search the [existing issues]({{ site.github.issues_url }}) - maybe your problem is already known.
* Make sure you are using the [latest version]({{ site.github.latest_release.html_url }}).

Then [create a new issue]({{ site.github.issues_url }}/new/choose) using the bug report template and include:
* KeeAnywhere version, KeePass version and Windows version
* The storage provider (e.g. `OneDrive`, `Dropbox (restricted)`)
* The complete error message (the text of the error dialog, use the "Copy" button or Ctrl+C if available)
* The steps to reproduce the problem

**Security vulnerabilities must not be reported as public issues.** Please follow the [security policy]({{ site.github.repository_url }}/security/policy) instead.

# Out of Scope Features
These features won't be accepted:
* Saving provider credentials in a KeePass database
* Cloud storage providers where I have to pay for the usage of KeeAnywhere (traffic, developer account, etc.)
* Self-hosted solutions (private cloud, homelab, NAS apps) - see [Public cloud providers only](#public-cloud-providers-only)
* Providers that can already be accessed without a dedicated provider - see [Providers with WebDAV, FTP or S3 access](#providers-with-webdav-ftp-or-s3-access)

## Providers with WebDAV, FTP or S3 access
Many providers can already be used without a dedicated KeeAnywhere provider (without file picker, I know :-)):
* with the protocols built into KeePass: WebDAV, FTP, HTTP/HTTPS, local files and network shares (SMB/UNC paths, e.g. a NAS) - see [KeePass' synchronization documentation](https://keepass.info/help/v2/sync.html). SFTP, SCP and FTPS are not built into KeePass, but available via the [IOProtocolExt](https://keepass.info/plugins.html#ioprotocolext) plugin.
* with KeeAnywhere's `Amazon S3` provider, if the provider offers an S3 compatible API (e.g. Wasabi, Backblaze B2, Synology C2).

A dedicated provider for these is only added **if the provider's own API offers clear benefits**, for example:
* login via OAuth instead of a password or app password (passwords for WebDAV/FTP are stored in KeePass' configuration, and two-factor authentication often doesn't apply)
* access restricted to an app folder instead of the complete storage
* better reliability, or WebDAV/FTP being limited, throttled or deprecated by the provider
* features like versioning or shared folders that WebDAV/FTP don't offer

## Public cloud providers only
KeeAnywhere supports **public cloud storage providers** only. Self-hosted solutions - private cloud, homelab or NAS apps like `Nextcloud`, `ownCloud`, `Seafile` or `Pydio` - won't be added as dedicated providers, because I can't test and maintain all of these products, versions and configurations.

You can use your self-hosted storage anyway:
* with the protocols built into KeePass: WebDAV, FTP or network shares (most of these solutions support WebDAV), or
* with KeeAnywhere's `Amazon S3` provider and an S3 compatible server (e.g. MinIO, Garage, SeaweedFS, Ceph) - see the [FAQ](use/faq#s3-compatible).

# Source Code Contribution
Source code contribution is welcome. Please keep in mind that this project has a vision and that your contribution should fit to it. It would be best to talk about new ideas first by creating a new issue.

## General Rules
1. Take a look at the list of [out of scope features](#out-of-scope-features).
2. Take a look at the issues - maybe your idea is already being discussed.
3. Fork the repository and create a pull request (see [Pull Requests](#pull-requests)).
4. In the end I will decide whether your pull request is accepted - therefore please discuss your ideas first. If I don't integrate your code, you can use the whole source code for your own project: it's licensed under the [MIT license]({{ site.github.repository_url }}/blob/master/LICENSE).

By contributing, you agree that your contributions are licensed under the MIT license.

## Development Environment
Details are described in [BUILD.md]({{ site.github.repository_url }}/blob/master/BUILD.md). In short:
* Visual Studio 2022 (or Build Tools) with the .NET Framework 4.8 Developer Pack
* Build with `build.cmd` or Visual Studio (configuration *Release* creates the PLGX file)
* API keys: `KeeAnywhere\ApiKeys.cs` is not under version control and is created from `ApiKeys.Template.cs` with dummy values on the first build. For testing a provider, register your own development app and put its keys into your local `ApiKeys.cs`.
* Test the PLGX with `test-plgx.ps1`: it builds the plugin and starts a separate test copy of KeePass with it.

## Technical Constraints
KeeAnywhere is distributed as PLGX file, which is compiled by KeePass on the user's machine. This leads to some constraints:
* **C# 5 only:** KeePass compiles the plugin with the C# 5 compiler of the .NET Framework. Don't use newer language features like `?.`, `??=`, `$"..."` string interpolation, `nameof`, expression-bodied members or pattern matching.
* **No XML comments in `KeeAnywhere.csproj`:** the PLGX build tool fails on them.
* NuGet packages are referenced via `packages.config` and must come from the official NuGet.org site.

## Pull Requests
* One topic per pull request, based on the current `master` branch.
* Describe what you changed and why, and reference the related issue (e.g. `Fixes #123`).
* Test your changes in KeePass (`test-plgx.ps1`) with the affected storage providers.
* Please **don't** change version numbers, `CHANGELOG.md` or release files (nuspec, version manifest) - this is done when preparing a release.
* Please **don't** commit binaries (e.g. tools like `nuget.exe`), API keys or unrelated formatting changes (whitespace, line endings, encoding/BOM).

## New Cloud Storage Provider
I don't implement new storage providers myself anymore - but they are welcome as pull requests and become [community-driven providers](#community-driven-providers). A new provider has to fit the [criteria](#out-of-scope-features) (e.g. public cloud only, a clear benefit compared to WebDAV/FTP/S3).

Please use the official SDK as first choice. All dependencies should be NuGet packages from the official NuGet.org site.

Keep in mind that simplicity for the user is the highest goal. So avoid complicated dialogs, the need to write documentation etc.

If an app registration is required (like for OAuth2 authentication), please keep in mind that I need the possibility to get an app key by myself. Providers for which I have to pay for traffic, a developer account or something like this are not acceptable (this project is developed in my spare time).

**Never** commit production keys/secrets. Use your own development keys in your local `ApiKeys.cs` (not under version control) - the production keys are kept secret by me.

## Community-driven Providers
KeeAnywhere distinguishes between two kinds of storage providers (see the list on the [homepage](/)):
* **Maintained providers** are maintained and tested by me.
* **Community-driven providers** are contributed and tested by the community. I make sure they still compile and include them in releases, but I don't test them myself - whether they work depends on feedback and contributions from their users.

If a community-driven provider stops working and nobody fixes it, it may be removed in a future version.

# Documentation
This website is generated with Jekyll from the [`docs` folder]({{ site.github.repository_url }}/tree/master/docs) of the repository. Corrections and improvements are welcome as pull request, too.
