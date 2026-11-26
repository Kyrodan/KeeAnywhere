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
                    TryRotateLocal(path);
                }
            }

            if (_configService.PluginConfiguration.IsBackupToRemoteEnabled)
            {
                if (await TryBackupRemote(path, now))
                {
                    await TryRotateRemote(path);
                }
            }

            stream.Position = 0;
            await this.BaseProvider.Save(stream, path);

        }

        // Removing old backups must never prevent saving the database
        private async Task TryRotateRemote(string path)
        {
            try
            {
                var folder = CloudPath.GetDirectoryName(path);
                var items = await this.BaseProvider.GetChildrenByParentPath(folder);

                // Remote backups are named after the unmasked cloud filename (see TryBackupRemote)
                var baseFilename = CloudPath.GetFileNameWithoutExtension(path);
                var extension = GetBackupExtension(CloudPath.GetFileName(path));
                var regex = new Regex("^" + Regex.Escape(baseFilename) + @"_Backup_\d{4}-\d{2}-\d{2}-\d{2}-\d{2}-\d{2}" + Regex.Escape(extension) + "$");

                var files = items.Where(item => item.Name != null && regex.IsMatch(item.Name)).Select(item => item.Name).ToArray();
                Array.Sort(files, StringComparer.Ordinal);

                for (var i = 0; i < files.Length - _configService.PluginConfiguration.BackupCopies; i++)
                {
                    await this.BaseProvider.Delete(CloudPath.Combine(folder, files[i]));
                }
            }
            catch (Exception)
            {
                // Ignore: old backups are removed with the next save
            }
        }

        private async Task<bool> TryBackupRemote(string path, DateTime saveDateTime)
        {
            try
            {
                var backupFilename = GetBackupFilename(CloudPath.GetFileName(path), saveDateTime);
                var backupFolder = CloudPath.GetDirectoryName(path);
                var backupPath = CloudPath.Combine(backupFolder, backupFilename);

                await this.BaseProvider.Copy(path, backupPath);
                return true;
            }
            catch (Exception)
            {
                // Try to copy file to backup: it fails if file is saved for the first time
                return false;
            }
        }

        // Removing old backups must never prevent saving the database
        private void TryRotateLocal(string path)
        {
            try
            {
                var folder = GetBackupFolder();
                var filenameFilter = GetBackupFilenamePattern(GetLocalFilename(path));

                var files = Directory.GetFiles(folder, filenameFilter, SearchOption.TopDirectoryOnly);
                Array.Sort(files, StringComparer.Ordinal);

                for (var i = 0; i < files.Length - _configService.PluginConfiguration.BackupCopies; i++)
                {
                    File.Delete(files[i]);
                }
            }
            catch (Exception)
            {
                // Ignore: old backups are removed with the next save
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
            var baseFilename = CloudPath.GetFileNameWithoutExtension(filename);

            filename = string.Format("{0}_Backup_{1:yyyy-MM-dd-HH-mm-ss}{2}", baseFilename, saveDateTime, GetBackupExtension(filename));

            return filename;
        }

        public string GetBackupFilenamePattern(string filename)
        {
            var baseFilename = CloudPath.GetFileNameWithoutExtension(filename);

            filename = string.Format("{0}_Backup_????-??-??-??-??-??{1}", baseFilename, GetBackupExtension(filename));

            return filename;
        }

        // Backups keep the extension of the saved file: other files saved beside the database (e.g. the .xml file
        // of KeeChallenge, #173) must not be backed up as .kdbx and rotated together with the database backups.
        // Files without extension keep the former .kdbx extension.
        private static string GetBackupExtension(string filename)
        {
            var extension = CloudPath.GetExtension(filename);
            return string.IsNullOrEmpty(extension) ? ".kdbx" : extension;
        }

    }
}
