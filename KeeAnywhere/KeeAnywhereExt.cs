using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.Forms;
using KeeAnywhere.StorageProviders;
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Native;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    /// <summary>
    /// Plugin for KeePass to allow synchronization with OneDrive
    /// </summary>
    /// <remarks>KeePass SDK documentation: http://keepass.info/help/v2_dev/plg_index.html</remarks>
    public class KeeAnywhereExt : Plugin
    {
        private IPluginHost _host;

        private ToolStripMenuItem _tsShowSettings;
        private ToolStripMenuItem _tsOpenFromCloudDrive;
        private ToolStripMenuItem _tsSaveToCloudDrive;

        private ConfigurationService _configService;
        private UIService _uiService;
        private StorageService _storageService;


        /// <summary>
        /// Returns the URL where KeePass can check for updates of this plugin
        /// </summary>
        public override string UpdateUrl
        {
            get { return @"https://raw.githubusercontent.com/Kyrodan/KeeAnywhere/master/version_manifest.txt"; }
        }

        /// <summary>
        /// Called when the Plugin is being loaded which happens on startup of KeePass
        /// </summary>
        /// <returns>True if the plugin loaded successfully, false if not</returns>
        public override bool Initialize(IPluginHost pluginHost)
        {
            if (_host != null) Terminate();
            if (pluginHost == null) return false;
            if (NativeLib.IsUnix()) return false;

            _host = pluginHost;

            // Load the configuration
            _configService = new ConfigurationService(pluginHost);
            _configService.Load();

            // Initialize storage providers
            _storageService = new StorageService(_configService);
            _storageService.Register();

            // Initialize UIService
            _uiService = new UIService(_configService, _storageService);


            // Add the menu option for configuration under Tools
            var menu = _host.MainWindow.ToolsMenu.DropDownItems;

            _tsShowSettings = new ToolStripMenuItem("KeeAnywhere Settings...", PluginResources.OneDrive_16x16);
            _tsShowSettings.Click += OnShowSetting;
            menu.Add(_tsShowSettings);

            // Add "Open from Cloud Drive..." to File\Open menu.
            var fileMenu = _host.MainWindow.MainMenu.Items["m_menuFile"] as ToolStripMenuItem;
            if (fileMenu != null)
            {
                var openMenu = fileMenu.DropDownItems["m_menuFileOpen"] as ToolStripMenuItem;
                if (openMenu != null)
                {
                    _tsOpenFromCloudDrive = new ToolStripMenuItem("Open from Cloud Drive...", PluginResources.OneDrive_16x16);
                    _tsOpenFromCloudDrive.Click += OnOpenFromCloudDrive;
                    openMenu.DropDownItems.Add(_tsOpenFromCloudDrive);
                }

                var saveMenu = fileMenu.DropDownItems["m_menuFileSaveAs"] as ToolStripMenuItem;
                if (saveMenu != null)
                {
                    var index = saveMenu.DropDownItems.IndexOfKey("m_menuFileSaveAsSep0");

                    _tsSaveToCloudDrive = new ToolStripMenuItem("Save to Cloud Drive...", PluginResources.OneDrive_16x16);
                    _tsSaveToCloudDrive.Click += OnSaveToCloudDrive;
                    saveMenu.DropDownItems.Insert(index, _tsSaveToCloudDrive);
                }
            }

            // Indicate that the plugin started successfully
            return true;
        }

        private async void OnSaveToCloudDrive(object sender, EventArgs e)
        {
            if (_host.Database == null) return;

            // First usage: register new account
            if (!await HasAccounts()) return;

            var form = new CloudDriveFilePicker();
            form.InitEx(_configService, _storageService, CloudDriveFilePicker.Mode.Save);
            var result = UIUtil.ShowDialogAndDestroy(form);

            if (result != DialogResult.OK)
                return;

            var ci = IOConnectionInfo.FromPath(form.ResultUri);
            ci.CredSaveMode = IOCredSaveMode.SaveCred;
            _host.MainWindow.SaveDatabaseAs(_host.Database, ci, true,null, true);
        }

        private async void OnOpenFromCloudDrive(object sender, EventArgs eventArgs)
        {
            // First usage: register new account
            if (!await HasAccounts()) return;

            var form = new CloudDriveFilePicker();
            form.InitEx(_configService, _storageService, CloudDriveFilePicker.Mode.Open);
            var result = UIUtil.ShowDialogAndDestroy(form);

            if (result != DialogResult.OK)
                return;
            
            var ci = IOConnectionInfo.FromPath(form.ResultUri);
            ci.CredSaveMode = IOCredSaveMode.SaveCred;

            _host.MainWindow.OpenDatabase(ci, null, false);
        }

        private async Task<bool> HasAccounts()
        {
            if (_configService.Accounts.Any()) return true;

            AccountConfiguration account = null;

            while (account == null)
            {
                account = await _uiService.CreateOrUpdateAccount(StorageType.OneDrive);

                if (account != null) continue;
                var result = MessageService.Ask("You did not created a new account.\nAt least one account is required to work with KeeAnywhere.", "KeeAnywhere", MessageBoxButtons.RetryCancel);

                if (result != DialogResult.Retry)
                {
                    return false;
                }
            }

            return true;
        }

        public override void Terminate()
        {
            if (_host == null) return;

            _configService.Save();

            _host.MainWindow.ToolsMenu.DropDownItems.Remove(_tsShowSettings);

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
                }
            }

            _tsShowSettings = null;
            _tsOpenFromCloudDrive = null;
        }

        /// <summary>
        /// Triggered when clicking on the KeeAnywhere menu item under Tools
        /// </summary>
        private void OnShowSetting(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            form.InitEx(_configService, _uiService);
            UIUtil.ShowDialogAndDestroy(form);
        }
    }
}
