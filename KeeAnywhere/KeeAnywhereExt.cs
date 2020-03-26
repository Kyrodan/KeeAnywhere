﻿using System;
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

            _host = pluginHost;

            // WebBrowser Feature Control 
            if (useInternalBrowser())
            {
                SetBrowserFeatureControl();
            }

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

        private void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // FeatureControl settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (string.Compare(fileName, "devenv.exe", true) == 0 || string.Compare(fileName, "XDesProc.exe", true) == 0)
                return;

            BrowserHelper.SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, BrowserHelper.GetBrowserEmulationMode()); // Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.
            //BrowserHelper.SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1);
            //SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0);
            //SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1);
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

            var httpasm = Assembly.Load("System.Net.Http.Primitives");
            var httpver = new Version(1, 5, 0, 0);

            var jsonasm = Assembly.Load("Newtonsoft.Json");
            //var jsonver = new Version(9, 0, 0, 0);

            var odasm = Assembly.Load("Microsoft.Graph.Core");

            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                var requestedAssembly = new AssemblyName(a.Name);

                if (requestedAssembly.Name == "System.Net.Http.Primitives" && requestedAssembly.Version == httpver)
                    return httpasm;

                if (requestedAssembly.Name == "Newtonsoft.Json" && requestedAssembly.Version < jsonasm.GetName().Version)
                    return jsonasm;

                if (requestedAssembly.Name == "Microsoft.Graph.Core" && requestedAssembly.Version < odasm.GetName().Version)
                    return odasm;

                return null;
            };
        }

        public static bool useInternalBrowser()
        {
            return !NativeLib.IsUnix();
        }
    }
}
