using System.Collections.Generic;
using KeeAnywhere.StorageProviders.AmazonDrive;
using KeeAnywhere.StorageProviders.AmazonS3;
using KeeAnywhere.StorageProviders.Box;
using KeeAnywhere.StorageProviders.Dropbox;
using KeeAnywhere.StorageProviders.GoogleDrive;
using KeeAnywhere.StorageProviders.HiDrive;
using KeeAnywhere.StorageProviders.HubiC;
using KeeAnywhere.StorageProviders.OneDrive;

namespace KeeAnywhere.StorageProviders
{
    public static class StorageRegistry
    {
        public static IEnumerable<StorageDescriptor> Descriptors = new[]
        {
            // Preserve sort order: ascending!
            new StorageDescriptor(StorageType.AmazonDrive, "Amazon Drive", "adrive", account => new AmazonDriveStorageProvider(account), () => new AmazonDriveStorageConfigurator(), PluginResources.AmazonDrive_16x16),
            new StorageDescriptor(StorageType.AmazonS3, "Amazon S3", "s3", account => new AmazonS3StorageProvider(account), () => new AmazonS3StorageConfigurator(), PluginResources.AmazonS3_16x16),
            new StorageDescriptor(StorageType.Box, "Box", "box", account => new BoxStorageProvider(account), () => new BoxStorageConfigurator(), PluginResources.Box_16x16),
            new StorageDescriptor(StorageType.Dropbox, "Dropbox", "dropbox", account => new DropboxStorageProvider(account), () => new DropboxStorageConfigurator(false), PluginResources.Dropbox_16x16),
            new StorageDescriptor(StorageType.DropboxRestricted, "Dropbox-Restricted", "dropbox-r", account => new DropboxStorageProvider(account), () => new DropboxStorageConfigurator(true), PluginResources.Dropbox_16x16),
            new StorageDescriptor(StorageType.GoogleDrive, "Google Drive", "gdrive", account => new GoogleDriveStorageProvider(account), () => new GoogleDriveStorageConfigurator(), PluginResources.GoogleDrive_16x16),
            new StorageDescriptor(StorageType.HiDrive, "HiDrive", "hidrive", account => new HiDriveStorageProvider(account), () => new HiDriveStorageConfigurator(), PluginResources.HiDrive_16x16),
            new StorageDescriptor(StorageType.HubiC, "hubiC", "hubic", account => new HubiCStorageProvider(account), () => new HubiCStorageConfigurator(), PluginResources.HubiC_16x16),
            new StorageDescriptor(StorageType.OneDrive, "OneDrive", "onedrive", account => new OneDriveStorageProvider(account), () => new OneDriveStorageConfigurator(), PluginResources.OneDrive_16x16),
        };
    }
}