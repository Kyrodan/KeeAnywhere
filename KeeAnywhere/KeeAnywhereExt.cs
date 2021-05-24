using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.Forms;
using KeeAnywhere.Offline;
using KeeAnywhere.StorageProviders;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Native;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    /// <summary>
    ///     Plugin for KeePass to allow access to cloud storage providers
    /// </summary>
    /// <remarks>KeePass SDK documentation: http://keepass.info/help/v2_dev/plg_index.html</remarks>
    public class KeeAnywhereExt : Plugin
    {
        private ConfigurationService _configService;
        private IPluginHost _host;
        private StorageService _storageService;
        private ToolStripMenuItem _tsOpenFromCloudDrive;
        private ToolStripMenuItem _tsSaveToCloudDrive;
        private ToolStripMenuItem _tsSaveCopyToCloudDrive;

        private UIService _uiService;
        private CacheManagerService _cacheManagerService;
        private KpResources _kpResources;

        /// <summary>
        /// Static Constructor; implemented to fix:
        /// * https://github.com/Kyrodan/KeeAnywhere/issues/141
        /// * https://github.com/Kyrodan/KeeAnywhere/issues/152
        /// </summary>
        static KeeAnywhereExt()
        {
            // Some binding redirection fixes for Google Drive API
            FixDependencyLoading();

            // Enable new TLS-Versions (see #152)
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        ///     Returns the URL where KeePass can check for updates of this plugin
        /// </summary>
        public override string UpdateUrl
        {
            get { return @"https://raw.githubusercontent.com/Kyrodan/KeeAnywhere/master/version_manifest.txt"; }
        }

        /// <summary>
        ///     Called when the Plugin is being loaded which happens on startup of KeePass
        /// </summary>
        /// <returns>True if the plugin loaded successfully, false if not</returns>
        public override bool Initialize(IPluginHost pluginHost)
        {
            if (_host != null) Terminate();
            if (pluginHost == null) return false;
            //if (NativeLib.IsUnix()) return false;

            _host = pluginHost;

            // Load the configuration
            _configService = new ConfigurationService(pluginHost);
            _configService.Load();

            // Initialize CacheManager
            _cacheManagerService = new CacheManagerService(_configService, _host);
            _cacheManagerService.RegisterEvents();

            // Initialize storage providers
            _storageService = new StorageService(_configService, _cacheManagerService);
            _storageService.RegisterPrefixes();

            // Initialize UIService
            _uiService = new UIService(_configService, _storageService);

            // Initialize KeePass-Resource Service
            _kpResources = new KpResources(_host);

            // Add "Open from Cloud Drive..." to File\Open menu.
            var fileMenu = _host.MainWindow.MainMenu.Items["m_menuFile"] as ToolStripMenuItem;
            if (fileMenu != null)
            {
                var openMenu = fileMenu.DropDownItems["m_menuFileOpen"] as ToolStripMenuItem;
                if (openMenu != null)
                {
                    _tsOpenFromCloudDrive = new ToolStripMenuItem("Open from Cloud Drive...",
                        PluginResources.KeeAnywhere_16x16);
                    _tsOpenFromCloudDrive.Click += OnOpenFromCloudDrive;
                    _tsOpenFromCloudDrive.ShortcutKeys = Keys.Control | Keys.Alt | Keys.O;
                    openMenu.DropDownItems.Add(_tsOpenFromCloudDrive);
                }

                var saveMenu = fileMenu.DropDownItems["m_menuFileSaveAs"] as ToolStripMenuItem;
                if (saveMenu != null)
                {
                    var index = saveMenu.DropDownItems.IndexOfKey("m_menuFileSaveAsSep0");

                    _tsSaveToCloudDrive = new ToolStripMenuItem("Save to Cloud Drive...",
                        PluginResources.KeeAnywhere_16x16);
                    _tsSaveToCloudDrive.Click += OnSaveToCloudDrive;
                    saveMenu.DropDownItems.Insert(index, _tsSaveToCloudDrive);

                    _tsSaveCopyToCloudDrive = new ToolStripMenuItem("Save Copy to Cloud Drive...",
                        PluginResources.KeeAnywhere_16x16);
                    _tsSaveCopyToCloudDrive.Click += OnSaveToCloudDrive;
                    saveMenu.DropDownItems.Add(_tsSaveCopyToCloudDrive);

                }
            }

            if (_configService.IsUpgraded)
            {
                _uiService.ShowChangelog();
            }

            // Indicate that the plugin started successfully
            return true;
        }

        public override ToolStripMenuItem GetMenuItem(PluginMenuType t)
        {
            if (t == PluginMenuType.Main)
            {
                // Add the menu option for configuration under Tools
                var tsShowSettings = new ToolStripMenuItem("KeeAnywhere Settings...", PluginResources.KeeAnywhere_16x16);
                tsShowSettings.Click += OnShowSetting;

                return tsShowSettings;
            }

            return null; // No menu items in other locations
        }

        private void OnSaveToCloudDrive(object sender, EventArgs e)
        {
            if (_host.Database == null) return;

            // First usage: register new account
            if (!HasAccounts()) return;

            _uiService.ShowDonationDialog();

            var form = new CloudDriveFilePicker();
            form.InitEx(_configService, _storageService, _kpResources, CloudDriveFilePicker.Mode.Save);
            var result = UIUtil.ShowDialogAndDestroy(form);

            if (result != DialogResult.OK)
                return;

            var ci = IOConnectionInfo.FromPath(form.ResultUri);
            ci.CredSaveMode = IOCredSaveMode.SaveCred;

            var isCopy = sender == _tsSaveCopyToCloudDrive;
            _host.MainWindow.SaveDatabaseAs(_host.Database, ci, true, null, isCopy);
        }

        private void OnOpenFromCloudDrive(object sender, EventArgs eventArgs)
        {
            // First usage: register new account
            if (!HasAccounts()) return;

            _uiService.ShowDonationDialog();

            var form = new CloudDriveFilePicker();
            form.InitEx(_configService, _storageService, _kpResources, CloudDriveFilePicker.Mode.Open);
            var result = UIUtil.ShowDialogAndDestroy(form);

            if (result != DialogResult.OK)
                return;

            var ci = IOConnectionInfo.FromPath(form.ResultUri);
            ci.CredSaveMode = IOCredSaveMode.SaveCred;

            _host.MainWindow.OpenDatabase(ci, null, false);
        }

        private bool HasAccounts()
        {
            if (_configService.Accounts.Any()) return true;

            var result = MessageService.Ask(
                "At least one account is required to work with KeeAnywhere.\r\nWould you like to open KeeAnywhere Settings to create a new account?",
                "KeeAnywhere", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                _uiService.ShowSettingsDialog();
            }

            return false;

            //AccountConfiguration account = null;

            //while (account == null)
            //{
            //    account = await _uiService.CreateOrUpdateAccount(StorageType.OneDrive);

            //    if (account != null) continue;
            //    var result = MessageService.Ask("You did not created a new account.\nAt least one account is required to work with KeeAnywhere.", "KeeAnywhere", MessageBoxButtons.RetryCancel);

            //    if (result != DialogResult.Retry)
            //    {
            //        return false;
            //    }
            //}

            //return true;
        }

        public override void Terminate()
        {
            if (_host == null) return;

            _configService.Save();
            _cacheManagerService.UnRegisterEvents();

            var fileMenu = _host.MainWindow.MainMenu.Items["m_menuFile"] as ToolStripMenuItem;
            if (fileMenu != null)
            {
                var openMenu = fileMenu.DropDownItems["m_menuFileOpen"] as ToolStripMenuItem;
                if (openMenu != null)
                {
                    openMenu.DropDownItems.Remove(_tsOpenFromCloudDrive);
                }

                var saveAsMenu = fileMenu.DropDownItems["m_menuFileSaveAs"] as ToolStripMenuItem;
                if (saveAsMenu != null)
                {
                    saveAsMenu.DropDownItems.Remove(_tsSaveToCloudDrive);
                    saveAsMenu.DropDownItems.Remove(_tsSaveCopyToCloudDrive);
                }
            }

            _tsOpenFromCloudDrive = null;
        }

        private void OnShowSetting(object sender, EventArgs e)
        {
            _uiService.ShowSettingsDialog();
        }

        private static void FixDependencyLoading()
        {
            // Google.Api relies on System.Net.Http.Primitives version 1.5.0.0
            // In general a binding redirect is added to the App.config file.
            // Due to this is a KeePass plugin which has no App.config, a workaround is implemented.
            //
            // See https://github.com/google/google-api-dotnet-client/issues/554 for details.

            var names = new[] {
                "System.Net.Http.Primitives",
                "Newtonsoft.Json",
                "Microsoft.Graph.Core",
                "Google.Apis",
                "Google.Apis.Auth",
                "Google.Apis.Core"
            //    "System.Text.Encodings.Web",
            //    "System.Runtime.CompilerServices.Unsafe"
            };

            var asms = new Dictionary<string, Assembly>();

            foreach (var name in names)
            {
                asms.Add(name, Assembly.Load(name));
            }

            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                var requestedAssembly = new AssemblyName(a.Name);
                var name = requestedAssembly.Name;

                if (asms.ContainsKey(name)) return asms[name];

                return null;
            };
        }
    }
}