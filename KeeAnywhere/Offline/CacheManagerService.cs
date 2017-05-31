using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePass.App.Configuration;
using KeePass.Forms;
using KeePass.Plugins;
using KeePassLib.Delegates;
using KeePassLib.Utility;

namespace KeeAnywhere.Offline
{
    public class CacheManagerService : ICacheSupervisor
    {
        private readonly ConfigurationService _configService;
        private readonly IPluginHost _host;

        public CacheManagerService(ConfigurationService configService, IPluginHost host)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            if (host == null) throw new ArgumentNullException("host");

            _configService = configService;
            _host = host;
        }

        public void RegisterEvents()
        {
            var kp = _host.MainWindow;
            //kp.FileOpened += OnFileOpened;
            //kp.FileClosed += OnFileClosed;
            kp.FileSavingPre += OnFileSaving;
            //kp.FileSaving += OnFileSaving;
            kp.FileSaved += OnFileSaved;
        }

        public void UnRegisterEvents()
        {
            var kp = _host.MainWindow;
            //kp.FileOpened -= OnFileOpened;
            //kp.FileClosed -= OnFileClosed;
            kp.FileSavingPre -= OnFileSaving;
            //kp.FileSaving -= OnFileSaving;
            kp.FileSaved -= OnFileSaved;
        }

        private void OnFileSaved(object sender, FileSavedEventArgs fileSavedEventArgs)
        {
            this.IsSaving = false;
        }

        private void OnFileSaving(object sender, FileSavingEventArgs fileSavingEventArgs)
        {
            this.IsSaving = true;
        }

        private void OnFileClosed(object sender, FileClosedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnFileOpened(object sender, FileOpenedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public IStorageProvider WrapInCacheProvider(IStorageProvider provider, Uri uri)
        {
            var cachedProvider = new CacheProvider(provider, uri, this);

            return cachedProvider;
        }

        public string CacheFolder
        {
            get
            {
                var path = _configService.PluginConfiguration.OfflineCacheFolder;

                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("Offline Cache Folder not configured.");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public bool IsSaving { get; protected set; }

        public void CouldNotSaveToRemote(Uri requestedUri, Exception ex)
        {
            Task.Run(() => MessageService.ShowWarning(
                "KeeAnywhere Offline Cache:\r\nCould not save to remote (but saved in offline cache):",
                requestedUri.ToString(),
                ex));
        }

        public void CouldNotOpenFromRemote(Uri requestedUri, Exception ex)
        {
            Task.Run(() => MessageService.ShowWarning(
                "KeeAnywhere Offline Cache:\r\nCould not open from remote, using offline cached version:",
                requestedUri.ToString(),
                ex));
        }

        public void OpenWithConflict(Uri requestedUri)
        {
            Task.Run(() => MessageService.ShowWarning(
                "KeeAnywhere Offline Cache:\r\nConflict opening Datbase.",
                "Both offline cached and online remote database are changed.\r\nMerging both datbases on next save is required.",
                requestedUri.ToString()));
        }

        public void UpdatedRemoteOnLoad(Uri requestedUri)
        {
            Task.Run(() => MessageService.ShowWarning(
                "KeeAnywhere Offline Cache:\r\nRemote Database was updated with last offline cached changes during open.",
                requestedUri.ToString()));
        }
    }
}
