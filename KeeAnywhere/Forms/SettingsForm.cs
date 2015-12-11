using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePass.UI;

namespace KeeAnywhere.Forms
{
    public partial class SettingsForm : Form
    {
        private ConfigurationService m_configService;
        private UIService m_uiService;

        public SettingsForm()
        {
            InitializeComponent();
        }

        public void InitEx(ConfigurationService configService, UIService uiService)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            if (uiService == null) throw new ArgumentNullException("uiService");

            m_configService = configService;
            m_uiService = uiService;
        }

        private void OnBtnCancelClick(object sender, EventArgs e)
        {

        }

        private void OnBtnOkClick(object sender, EventArgs e)
        {
            // General Settings
            m_configService.PluginConfiguration.IsOfflineCacheEnabled = m_cbOfflineCache.Checked;

            if (m_rbStorageLocation_WindowsCredentialManager.Checked)
                m_configService.PluginConfiguration.AccountStorageLocation = AccountStorageLocation.WindowsCredentialManager;
            else if (m_rbStorageLocation_Disk.Checked)
                m_configService.PluginConfiguration.AccountStorageLocation = AccountStorageLocation.KeePassConfig;
            else
                throw new NotImplementedException();


            m_configService.Save();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.OneDrive_48x48, "KeeAnywhere Settings",
                "Here you can manage KeeAnywhere's settings.");

            InitGeneralTab();
            InitAccountsTab();
            InitAboutTab();
        }

        private void InitAboutTab()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName().Name;
                var gitVersionInformationType = assembly.GetType(assemblyName + ".GitVersionInformation");
                var versionField = gitVersionInformationType.GetField("SemVer");
                var version = versionField.GetValue(null);
                m_lblAboutVersion.Text = string.Format("Version {0}", version);
            }
            catch
            {
                m_lblAboutVersion.Text = "Version (unknown)";
            }
        }

        private void InitAccountsTab()
        {
            switch (m_configService.PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.WindowsCredentialManager:
                    m_rbStorageLocation_WindowsCredentialManager.Checked = true;
                    break;
                case AccountStorageLocation.KeePassConfig:
                    m_rbStorageLocation_Disk.Checked = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var descriptor in StorageRegistry.Descriptors)
            {
                var item = m_mnuAdd.Items.Add(descriptor.Type.ToString());
                item.Tag = descriptor.Type;
                item.Click += OnAccountAdd;
            }

            m_lvAccounts.Columns.Add("Type");
            m_lvAccounts.Columns.Add("Name");
            m_lvAccounts.Columns.Add("ID");
            m_lvAccounts.Columns.Add("Refresh Token");

            UIUtil.ResizeColumns(m_lvAccounts, new int[] {
				2, 2, 2, 4 }, true);

			UpdateAccountList();
        }

        private void UpdateAccountList()
        {
            var s = UIUtil.GetScrollInfo(m_lvAccounts, true);

            m_lvAccounts.BeginUpdate();
            m_lvAccounts.Items.Clear();

            foreach (var account in m_configService.Accounts.OrderBy(_ => _.Type).ThenBy(_ => _.Name))
            {
                var lvi = new ListViewItem(account.Type.ToString());
                var lviNew = m_lvAccounts.Items.Add(lvi);

                lviNew.Tag = account;
                lviNew.SubItems.Add(account.Name);
                lviNew.SubItems.Add(account.Id);
                lviNew.SubItems.Add(account.Secret);
            }

            UIUtil.Scroll(m_lvAccounts, s, true);
            m_lvAccounts.EndUpdate();

        }

        private void InitGeneralTab()
        {
            m_cbOfflineCache.Checked = m_configService.PluginConfiguration.IsOfflineCacheEnabled;
        }

        private void OnHelpMeChooseAccountStorage(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //TODO: Change to production URL
            Process.Start("https://localhost/AccountStorageLocation.md");
        }


        private async void OnAccountAdd(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null) return;

            await m_uiService.CreateOrUpdateAccount((StorageType)item.Tag);

            UpdateAccountList();
        }

        private void OnAccountRemove(object sender, EventArgs e)
        {
            foreach (ListViewItem item in m_lvAccounts.SelectedItems)
            {
                var account = item.Tag as AccountConfiguration;
                if (account == null) continue;

                m_configService.Accounts.Remove(account);
            }

            UpdateAccountList();
        }

        private void OnReportBug(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/issues");
        }

        private void OnContactAuthor(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Kyrodan");
        }
    }
}
