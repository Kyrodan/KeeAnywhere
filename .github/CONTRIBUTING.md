# Contributing to KeeAnywhere

Everyone is invited to contribute - every help is welcome!
The complete contribution guide is on the website: **https://keeanywhere.de/contribute**

## Be respectful
KeeAnywhere is developed in my spare time. Please keep issues, pull requests and discussions friendly and constructive - no insults, harassment or discrimination. Instead of "+1" comments, please use the 👍 reaction. There is no guaranteed support or response time. Comments that don't follow these rules may be hidden or deleted, and discussions may be locked. Details: [Be Respectful](https://keeanywhere.de/contribute#be-respectful).

## Reporting bugs
- Check the [FAQ](https://keeanywhere.de/use/faq), [Known Issues](https://keeanywhere.de/use/known_issues) and the [existing issues](https://github.com/Kyrodan/KeeAnywhere/issues) first, and make sure you use the [latest version](https://github.com/Kyrodan/KeeAnywhere/releases/latest).
- Use the bug report template and include KeeAnywhere, KeePass and Windows version, the storage provider and the complete error message.
- **Security vulnerabilities must not be reported as public issues** - see [SECURITY.md](SECURITY.md).

## Feature requests
Please take a look at the [out of scope features](https://keeanywhere.de/contribute#out-of-scope-features) first, and discuss bigger ideas in an issue before starting to code.

## Pull requests
- Development setup: see [BUILD.md](../BUILD.md) (Visual Studio 2022, .NET Framework 4.8 Developer Pack, `build.cmd`, `test-plgx.ps1`).
- KeePass compiles the plugin (PLGX) with **C# 5**: no `?.`, `$"..."`, `nameof` or other newer language features. No XML comments in `KeeAnywhere.csproj`.
- One topic per pull request, based on the current `master`, with a description and a reference to the related issue.
- Please don't change version numbers, `CHANGELOG.md` or release files, and don't commit binaries, API keys (`ApiKeys.cs` is not under version control) or unrelated formatting changes.

By contributing, you agree that your contributions are licensed under the [MIT license](../LICENSE).
