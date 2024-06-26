﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using KeeAnywhere.Configuration;
using File = Google.Apis.Drive.v3.Data.File;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private DriveService _api;

        public GoogleDriveStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            _account = account;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();

            var file = await api.GetFileByPath(path, true);
            if (file == null)
                return null;

            var stream = new MemoryStream();
            var progress = await api.Files.Get(file.Id).DownloadAsync(stream);

            if (progress.Status != DownloadStatus.Completed || progress.Exception != null)
                return null;

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }


        public async Task Save(Stream stream, string path)
        {
            var api = await GetApi();

            IUploadProgress progress;

            var file = await api.GetFileByPath(path, true);
            if (file != null)
            {
                progress = await api.Files.Update(null, file.Id, stream, "application/octet-stream").UploadAsync();
            }
            else // not found: creating new
            {

                var folderName = CloudPath.GetDirectoryName(path);
                var fileName = CloudPath.GetFileName(path);

                file = new File()
                {
                    Name = fileName
                };

                if (!string.IsNullOrEmpty(folderName))
                {
                    var folder = await api.GetFileByPath(folderName, true);
                    if (folder == null)
                        throw new InvalidOperationException(string.Format("Folder does not exist: {0}", folderName));

                    file.Parents = new[] {folder.Id};
                }

                progress = await api.Files.Create(file, stream, "application/octet-stream").UploadAsync();
            }

            if (progress.Status != UploadStatus.Completed || progress.Exception != null)
                throw new InvalidOperationException("Save to Google Drive failed.");
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await GetApi();

            var sourceFile = await api.GetFileByPath(sourcePath, true);
            if (sourceFile == null)
                throw new FileNotFoundException("Google Drive: File not found.", sourcePath);

            var destFilename = CloudPath.GetFileName(destPath);
            var destFolder = CloudPath.GetDirectoryName(destPath);
            var destFile = new File
            {
                Name = destFilename,
                Parents = new[] { "root" }
            };

            if (!string.IsNullOrEmpty(destFolder))
            {
                var parentFolder = await api.GetFileByPath(destFolder, true);
                if (parentFolder == null)
                    throw new FileNotFoundException("Google Drive: File not found.", destFolder);

                destFile.Parents = new[] { parentFolder.Id };
            }

            var result = await api.Files.Copy(destFile, sourceFile.Id).ExecuteAsync();
        }

        public async Task Delete(string path)
        {
            var api = await GetApi();

            var file = await api.GetFileByPath(path, false);
            if (file == null)
                throw new FileNotFoundException("Goolge Drive: File not found.", path);

            var result = await api.Files.Delete(file.Id).ExecuteAsync();
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            return new StorageProviderItem()
                {
                    Id = "root",
                    Name = "root",
                    Type = StorageProviderItemType.Folder,
                };
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var api = await GetApi();
            var query = api.Files.List();
            query.Q = string.Format("'{0}' in parents and trashed = false", parent.Id);
            query.Fields = "nextPageToken, files(id, name, mimeType, shortcutDetails, modifiedTime, parents)"; //The shortcutDetails field isn't returned in queries by default. Unless we request it, it's always null. The downside is, now we have to spell out every field we *do* want. Forgetting something we need will mean it's always set to null in the returned query, File object, etc. and things will break. This already happened once when I forgot I needed to explicitly request nextPageToken.

            var items = await query.ExecuteAsync();
            var newItems = items.Files.Select(async _ => 
			{
				var result = await MakeStorageProviderItem(_, api);
				result.ParentReferenceId = parent.Id;
				return result;
			});


            while (items.NextPageToken != null)
            {
                query.PageToken = items.NextPageToken;

                items = await query.ExecuteAsync();
                newItems = newItems.Concat(items.Files.Select(async _ => 
				{
					var result = await MakeStorageProviderItem(_, api);
					result.ParentReferenceId = parent.Id; return result;
				}));
            }

            return await Task.WhenAll(newItems.ToArray());
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return await GetChildrenByParentItem(await GetRootItem());

            var api = await GetApi();
            var item = await api.GetFileByPath(path, true);
            if (item == null)
                throw new FileNotFoundException("Google Drive: File not found.", path);

            return await GetChildrenByParentItem(new StorageProviderItem {Id = item.Id});
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { '/', '\\'};
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected async Task<DriveService> GetApi()
        {
            if (_api == null)
                _api = await GoogleDriveHelper.GetClient(_account);

            return _api;
        }

        protected async Task<StorageProviderItem> MakeStorageProviderItem(File _, DriveService api)
        {
            var isShortcut = false;
            if (_.MimeType == "application/vnd.google-apps.shortcut")
            {
                isShortcut = true;
                if (_.ShortcutDetails==null)
                {
                    var fileQuery = api.Files.Get(_.Id);
                    fileQuery.Fields = "shortcutDetails";

                    _ = await fileQuery.ExecuteAsync();
                }
            }
            var result = new StorageProviderItem()
            {
                Id =
                    isShortcut
                        ? _.ShortcutDetails.TargetId
                        : _.Id,
                Name = _.Name,
                Type =
                    isShortcut
                        ? _.ShortcutDetails.TargetMimeType == "application/vnd.google-apps.folder"
                            ? StorageProviderItemType.Folder
                            : StorageProviderItemType.File
                        : _.MimeType == "application/vnd.google-apps.folder"
                            ? StorageProviderItemType.Folder
                            : StorageProviderItemType.File,
                LastModifiedDateTime = _.ModifiedTime,
                ParentReferenceId = _.Parents.FirstOrDefault(),
            };

            return result;
        }
    }
}
