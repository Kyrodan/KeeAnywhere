using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;

namespace KeeAnywhere.Backup
{
    public class BackupProvider : ProxyProvider
    {
        private readonly StorageUri _requestedUri;
        private readonly ConfigurationService _configService;

        public BackupProvider(IStorageProvider baseProvider, StorageUri requestedUri, ConfigurationService configService)
            :base(baseProvider)
        {
            if (requestedUri == null) throw new ArgumentNullException("requestedUri");
            if (configService == null) throw new ArgumentNullException("configService");
            _requestedUri = requestedUri;
            _configService = configService;
        }

        public static IStorageProvider WrapInBackupProvider(IStorageProvider provider, StorageUri requestedUri, ConfigurationService configService)
        {
            var backupProvider = new BackupProvider(provider, requestedUri, configService);

            return backupProvider;
        }

        public override async Task Save(Stream stream, string path)
        {
            var now = DateTime.Now;

            if (_configService.PluginConfiguration.IsBackupToLocalEnabled)
            {
                if (TryBackupLocal(stream, path, now))
                {
                    RotateLocal(path);
                }
            }

            if (_configService.PluginConfiguration.IsBackupToRemoteEnabled)
            {
                if (await TryBackupRemote(path, now))
                {
                    await RotateRemote(path);
                }
            }

            stream.Position = 0;
            await this.BaseProvider.Save(stream, path);

        }

        private async Task RotateRemote(string path)
        {
            var folder = CloudPath.GetDirectoryName(path);
            var items = await this.BaseProvider.GetChildrenByParentPath(folder);

            var pattern = "^" + Regex.Escape(GetBackupFilenamePattern(path))
                                     .Replace("\\*", ".*")
                                     .Replace("\\?", ".") + "$";

            var regex = new Regex(pattern);
            var files = items.Where(item => regex.IsMatch(item.Name)).Select(item => item.Name).ToArray();
            Array.Sort(files);

            for (var i = 0; i < files.Length - _configService.PluginConfiguration.BackupCopies; i++)
            {
                await this.BaseProvider.Delete(CloudPath.Combine(folder, files[i]));
            }
        }

        private async Task<bool> TryBackupRemote(string path, DateTime saveDateTime)
        {
            var backupFilename = GetBackupFilename(CloudPath.GetFileName(path), saveDateTime);
            var backupFolder = CloudPath.GetDirectoryName(path);
            var backupPath = CloudPath.Combine(backupFolder, backupFilename);

            try
            {
                await this.BaseProvider.Copy(path, backupPath);
                return true;
            }
            catch (Exception)
            {
                // Try to copy file to backup: it fails if file is saved for the first time
                return false;
            }
        }

        private void RotateLocal(string path)
        {
            var folder = GetBackupFolder();
            var filenameFilter = GetBackupFilenamePattern(path);

            var files = Directory.GetFiles(folder, filenameFilter, SearchOption.TopDirectoryOnly);
            Array.Sort(files);

            for (var i = 0; i < files.Length - _configService.PluginConfiguration.BackupCopies; i++)
            {
                File.Delete(files[i]);
            }
        }

        private bool TryBackupLocal(Stream stream, string path, DateTime saveDateTime)
        {
            try
            {
                stream.Position = 0;

                var folder = GetBackupFolder();

                var localFilename = GetLocalFilename(path);
                var localPath = Path.Combine(folder, localFilename);

                if (File.Exists(localPath))
                {
                    var backupFilename = GetBackupFilename(GetLocalFilename(path), saveDateTime);
                    var backupPath = Path.Combine(folder, backupFilename);
                    File.Move(localPath, backupPath);
                }

                using (var backupStream = File.OpenWrite(localPath))
                    stream.CopyTo(backupStream);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetBackupFolder()
        {
            var accountFolder = string.Format("{0} - {1}", _requestedUri.Scheme, _requestedUri.GetAccountName());
            var path = Path.Combine(_configService.PluginConfiguration.BackupToLocalFolder, accountFolder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public string GetLocalFilename(string path)
        {
            var filename = CloudPath.MaskInvalidFileNameChars(CloudPath.GetFileName(path));

            return filename;
        }

        public string GetBackupFilename(string filename, DateTime saveDateTime)
        {
            var baseFilename = Path.GetFileNameWithoutExtension(filename);

            filename = string.Format("{0}_Backup_{1:yyyy-MM-dd-HH-mm-ss}.kdbx", baseFilename, saveDateTime);

            return filename;
        }

        public string GetBackupFilenamePattern(string path)
        {
            var filename = GetLocalFilename(path);
            var baseFilename = Path.GetFileNameWithoutExtension(filename);

            filename = string.Format("{0}_Backup_????-??-??-??-??-??.kdbx", baseFilename);

            return filename;
        }

    }
}
