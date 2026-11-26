# Security Policy

KeeAnywhere gives KeePass access to your cloud storage, so security reports are taken seriously.

## Supported versions

Only the [latest release](https://github.com/Kyrodan/KeeAnywhere/releases/latest) receives security fixes.

## Reporting a vulnerability

**Please do not report security vulnerabilities as public issues.**

Use GitHub's private vulnerability reporting instead: open the [Security tab](https://github.com/Kyrodan/KeeAnywhere/security) of this repository and click **Report a vulnerability**.

Please include:
- the affected KeeAnywhere version (and KeePass version, if relevant)
- the affected storage provider(s)
- a description of the vulnerability and its impact
- steps to reproduce or a proof of concept

KeeAnywhere is developed in my spare time - I'll respond as soon as possible and keep you informed about the progress. Please give me reasonable time to release a fix before disclosing the vulnerability publicly.

## Out of scope

- **OAuth client ids/secrets contained in the released plugin:** KeeAnywhere is a desktop application, so its OAuth client credentials cannot be kept secret (they are part of every released PLGX file). This is common for OAuth desktop clients and not a vulnerability. Access to your data always requires your own login and consent at the cloud provider.
- Vulnerabilities in KeePass itself - please report them to [KeePass](https://keepass.info/).
- Vulnerabilities in third-party libraries that are already fixed in a newer version - a hint via a regular issue is fine.
