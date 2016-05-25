using System;
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

            var file = await api.GetFileByPath(path);
            if (file == null)
                return null;

            var stream = new MemoryStream();
            var progress = await api.Files.Get(file.Id).DownloadAsync(stream);

            if (progress.Status != DownloadStatus.Completed || progress.Exception != null)
                return null;

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }


        public async Task<bool> Save(Stream stream, string path)
        {
            var api = await GetApi();

            IUploadProgress progress;

            var file = await api.GetFileByPath(path);
            if (file != null)
            {
                progress = await api.Files.Update(null, file.Id, stream, "application/octet-stream").UploadAsync();
            }
            else // not found: creating new
            {

                var folderName = CloudPath.GetDirectoryName(path);
                var fileName = CloudPath.GetFileName(path);

                var folder = await api.GetFileByPath(folderName);
                if (folder == null)
                    throw new InvalidOperationException(string.Format("Folder does not exist: {0}", folderName));

                file = new File()
                {
                    Name = fileName,
                    Parents = new[] {folder.Id},
                };

                progress = await api.Files.Create(file, stream, "application/octet-stream").UploadAsync();
            }

            return progress.Status == UploadStatus.Completed && progress.Exception == null;
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
            var items = await query.ExecuteAsync();

            var newItems = items.Files.Select(_ => new StorageProviderItem()
            {
                Id = _.Id,
                Name = _.Name,
                Type =
                    _.MimeType == "application/vnd.google-apps.folder"
                        ? StorageProviderItemType.Folder
                        : StorageProviderItemType.File,
                LastModifiedDateTime = _.ModifiedTime,
                ParentReferenceId = parent.Id,
            });

            return newItems.ToArray();
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
    }
}
