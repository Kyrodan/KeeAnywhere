using System;
using System.IO;
using System.Threading.Tasks;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageProvider : IStorageProvider
    {
        private readonly string _token;
        private readonly OneDriveApi _api;

        public OneDriveStorageProvider(string token)
        {
            if (token == null) throw new ArgumentNullException("token");
            _token = token;

            var api =
                Task.Run(async () => await
                    OneDriveApi.GetOneDriveApiFromRefreshToken(OneDriveService.OneDriveClientId,
                        OneDriveService.OneDriveClientSecret, token));

            _api = api.Result;
        }

        public bool Delete(string path)
        {
            var isOk = Task.Run(async () =>
            {
                var item = await _api.GetItem(path);
                if (item == null) return true;
                return await _api.Delete(path);
            });

            return isOk.Result;
        }

        public Stream Load(string path)
        {
            var tempFilename = Path.GetTempFileName();
            var isOk = Task.Run(async () =>
            {
                // Workaround due to Bug #2 in OneAdriveApi.DownloadItemAndSave(string path, string filename)
                var item = await _api.GetItem(path);
                if (item == null) return false;

                return await _api.DownloadItemAndSaveAs(item, tempFilename);
            });

            if (!isOk.Result)
            {
                File.Delete(tempFilename);
                throw new FileNotFoundException("OneDrive: File not found", path);
            }

            var content = File.ReadAllBytes(tempFilename);
            File.Delete(tempFilename);
            var stream = new MemoryStream(content, false);

            return stream;
        }


        public bool Save(MemoryStream stream, string path)
        {
            var tempFilename = Path.GetTempFileName();
            var bytes = stream.ToArray();
            File.WriteAllBytes(tempFilename, bytes);

            var uploadedItem = Task.Run(async () =>
            {
                var directory = Path.GetDirectoryName(path);
                var filename = Path.GetFileName(path);

                return await _api.UploadFileAs(tempFilename, filename, directory);
            });

            uploadedItem.Wait();

            File.Delete(tempFilename);

            return uploadedItem.Result != null;
        }

        public bool Move(string pathFrom, string pathTo)
        {
            throw new NotImplementedException("OneDrive: Move-Operation not implemented");
        }
    }
}