---
title: Known noteworthy issues
---
There are some known issues:
* Linux/Mono and Wine are currently not supported: KeePass cannot compile the plugin there. See details in [#18](https://github.com/Kyrodan/KeeAnywhere/issues/18).
* `Google Drive (restricted)` only shows files created with KeeAnywhere, and Google may block the full access mode (`Google Drive`). See the [FAQ](faq) how to handle this.
* When opening a database automatically (KeeAutoExec, triggers, command line) KeePass may show the `Open URL` dialog or ask for server credentials. See [Advanced Topics](advanced_topics#open-automatically) how to avoid this.
* KpScript is not supported, because it does not support plugins. See details in [#66](https://github.com/Kyrodan/KeeAnywhere/issues/66).
* After upgrading from versions older than 2.0.0 some accounts have to be deleted and re-registered.