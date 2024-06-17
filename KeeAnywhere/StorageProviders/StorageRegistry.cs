using System.Collections.Generic;
using System.Linq;
using KeeAnywhere.StorageProviders.AmazonS3;
using KeeAnywhere.StorageProviders.Azure;
using KeeAnywhere.StorageProviders.Box;
using KeeAnywhere.StorageProviders.Dropbox;
using KeeAnywhere.StorageProviders.GoogleCloudPlatform;
using KeeAnywhere.StorageProviders.GoogleDrive;
using KeeAnywhere.StorageProviders.HiDrive;
using KeeAnywhere.StorageProviders.OneDrive;
using KeePassLib.Native;

namespace KeeAnywhere.StorageProviders
{
    public static class StorageRegistry
    {
        static StorageRegistry()
        {
            var d = new HashSet<StorageDescriptor>();

            var isUnix = NativeLib.IsUnix();

            d.Add(new StorageDescriptor(StorageType.AmazonS3, "Amazon S3", "s3", account => new AmazonS3StorageProvider(account), () => new AmazonS3StorageConfigurator(), PluginResources.AmazonS3_16x16));
            d.Add(new StorageDescriptor(StorageType.AzureBlob, "Azure Blob Storage", "azureblob", account => new AzureBlobStorageProvider(account), () => new AzureStorageConfigurator(StorageType.AzureBlob), PluginResources.Azure_16x16));
            d.Add(new StorageDescriptor(StorageType.AzureFile, "Azure File Storage", "azurefile", account => new AzureFileStorageProvider(account), () => new AzureStorageConfigurator(StorageType.AzureFile), PluginResources.Azure_16x16));
            if (!isUnix) d.Add(new StorageDescriptor(StorageType.Box, "Box", "box", account => new BoxStorageProvider(account), () => new BoxStorageConfigurator(), PluginResources.Box_16x16));
            d.Add(new StorageDescriptor(StorageType.Dropbox, "Dropbox", "dropbox", account => new DropboxStorageProvider(account), () => new DropboxStorageConfigurator(false), PluginResources.Dropbox_16x16));
            d.Add(new StorageDescriptor(StorageType.DropboxRestricted, "Dropbox (restricted)", "dropbox-r", account => new DropboxStorageProvider(account), () => new DropboxStorageConfigurator(true), PluginResources.Dropbox_16x16));
            d.Add(new StorageDescriptor(StorageType.GoogleCloudStorage, "Google Cloud Storage", "gs", account => new GoogleCloudStorageProvider(account), () => new GoogleCloudStorageConfigurator(), PluginResources.GoogleCloudStorage_16x16));
            d.Add(new StorageDescriptor(StorageType.GoogleDrive, "Google Drive", "gdrive", account => new GoogleDriveStorageProvider(account), () => new GoogleDriveStorageConfigurator(false), PluginResources.GoogleDrive_16x16));
            d.Add(new StorageDescriptor(StorageType.GoogleDriveRestricted, "Google Drive (restricted)", "gdrive-r", account => new GoogleDriveStorageProvider(account), () => new GoogleDriveStorageConfigurator(true), PluginResources.GoogleDrive_16x16));
            d.Add(new StorageDescriptor(StorageType.HiDrive, "HiDrive", "hidrive", account => new HiDriveStorageProvider(account), () => new HiDriveStorageConfigurator(), PluginResources.HiDrive_16x16));
            d.Add(new StorageDescriptor(StorageType.OneDrive, "OneDrive", "onedrive", account => new OneDriveStorageProvider(account), () => new OneDriveStorageConfigurator(), PluginResources.OneDrive_16x16));

            Descriptors = d.ToArray();
        }

        public static IEnumerable<StorageDescriptor> Descriptors;
    }
}