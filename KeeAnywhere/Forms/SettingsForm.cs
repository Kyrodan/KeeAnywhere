using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere.Forms
{
    public partial class SettingsForm : Form
    {
        private ConfigurationService m_configService;
        private UIService m_uiService;

        public SettingsForm()
        {
            InitializeComponent();

            m_tcSettings.TabPages.Remove(m_tabGeneral);
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

            if (m_rbStorageLocation_LocalUserSecureStore.Checked)
                m_configService.PluginConfiguration.AccountStorageLocation = AccountStorageLocation.LocalUserSecureStore;
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
                PluginResources.KeeAnywhere_48x48, "KeeAnywhere Settings",
                "Here you can manage KeeAnywhere's settings.");

            InitGeneralTab();
            InitAccountsTab();
            InitAboutTab();
        }

        private void InitAboutTab()
        {
            var version = "(unknown)";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (versionAttribute != null)
                    version = versionAttribute.InformationalVersion;

                //var assemblyName = assembly.GetName().Name;
                //var gitVersionInformationType = assembly.GetType(assemblyName + ".GitVersionInformation");
                //var versionField = gitVersionInformationType.GetField("SemVer");
                //var version = versionField.GetValue(null);
                
            }
            catch
            {
                // ignored
            }
            finally
            {
                m_lblAboutVersion.Text = string.Format("Version {0}", version);
            }
        }

        private void InitAccountsTab()
        {
            switch (m_configService.PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.LocalUserSecureStore:
                    m_rbStorageLocation_LocalUserSecureStore.Checked = true;
                    break;
                case AccountStorageLocation.KeePassConfig:
                    m_rbStorageLocation_Disk.Checked = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var descriptor in StorageRegistry.Descriptors)
            {
                var item = m_mnuAdd.Items.Add(descriptor.FriendlyName, descriptor.SmallImage);
                item.Tag = descriptor;
                item.Click += OnAccountAdd;

                m_imlProviderIcons.Images.Add(descriptor.Type.ToString(), descriptor.SmallImage);
            }

            m_lvAccounts.Columns.Add("Name");
            m_lvAccounts.Columns.Add("Type");
#if DEBUG
            m_lvAccounts.Columns.Add("ID");
            m_lvAccounts.Columns.Add("Secret");
#endif

            UIUtil.ResizeColumns(m_lvAccounts, new int[] {
				3, 2, 2, 2 }, true);

			UpdateAccountList();
        }

        private void UpdateAccountList()
        {
            var s = UIUtil.GetScrollInfo(m_lvAccounts, true);

            m_lvAccounts.BeginUpdate();
            m_lvAccounts.Items.Clear();

            var groups =
                StorageRegistry.Descriptors.Select(_ => new ListViewGroup(_.Type.ToString(), _.FriendlyName)).ToArray();
            m_lvAccounts.Groups.AddRange(groups);

            foreach (var account in m_configService.Accounts.OrderBy(_ => _.Type).ThenBy(_ => _.Name))
            {
                if (StorageRegistry.Descriptors.All(_ => _.Type != account.Type)) 
                    continue;
                
                var lvi = new ListViewItem(account.Name);
                var lviNew = m_lvAccounts.Items.Add(lvi);

                lviNew.Tag = account;
                lviNew.ImageKey = account.Type.ToString();
                lviNew.SubItems.Add(account.Type.ToString());
                lviNew.SubItems.Add(account.Id);
                lviNew.SubItems.Add(account.Secret);
                lviNew.Group = m_lvAccounts.Groups[account.Type.ToString()];
            }

            UIUtil.Scroll(m_lvAccounts, s, true);

            m_lvAccounts.EndUpdate();
        }

        private void InitGeneralTab()
        {
            m_cbOfflineCache.Checked = m_configService.PluginConfiguration.IsOfflineCacheEnabled;
        }

        private async void OnAccountAdd(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null) return;

            await m_uiService.CreateOrUpdateAccount(((StorageDescriptor)item.Tag).Type);
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

        private void OnWhatsNew(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/blob/master/CHANGELOG.md");
        }

        private void OnReportBug(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/issues");
        }

        private void OnContactAuthor(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan");
        }

        private void OnDonate(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/blob/master/DONATE.md");
        }

        private void OnHomepage(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere");
        }

        private void OnDocumentation(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/wiki");
        }

        private void OnHelpMeChooseAccountStorage(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/wiki/Getting-Started#which-account-storage-location-should-i-choose");
        }

        private void OnLicense(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/blob/master/LICENSE");
        }

        private void OnAfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null) return;

            var name = e.Label;

            // Has the name been removed?
            if (name == string.Empty)
            {
                MessageService.ShowWarning("An account name must be given.", "Discarding change.");
                e.CancelEdit = true;
                return;
            }

            // Does the input contains leading or trailing spaces?
            if (char.IsWhiteSpace(name, 0) || char.IsWhiteSpace(name, name.Length - 1))
            {
                MessageService.ShowWarning("The name must not contain leading or trailing spaces.", "Discarding change.");
                e.CancelEdit = true;
                return;
            }

            // Does the input contains disallowed chars?
            var disallowedChars = "!*';:&=+$,/?#[]";
            if (name.IndexOfAny(disallowedChars.ToCharArray()) != -1)
            {
                MessageService.ShowWarning("The name must not contain disallowed chars:", disallowedChars);
                e.CancelEdit = true;
                return;
            }

            var item = m_lvAccounts.Items[e.Item];
            var account = item.Tag as AccountConfiguration;
            if (account == null) return;

            // The user typed but the name has not been changed?
            if (name.Equals(account.Name, StringComparison.InvariantCultureIgnoreCase)) return;

            // Does another account of the same type exists with the same name?
            var nameExists =
                m_configService.Accounts.Any(
                    _ => _ != account && _.Type == account.Type && _.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (nameExists) // Change accepted
            {
                MessageService.ShowWarning("An account with this name already exists.", "Discarding change.");
                e.CancelEdit = true;
                return;
            }

            account.Name = name;
        }

        private async void OnAccountCheck(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            m_tcSettings.Enabled = false;
            m_pnlFormButtons.Enabled = false;

            foreach (ListViewItem item in m_lvAccounts.SelectedItems)
            {
                var account = item.Tag as AccountConfiguration;
                if (account == null) continue;

                await m_uiService.CheckOrUpdateAccount(account);
            }

            m_tcSettings.Enabled = true;
            m_pnlFormButtons.Enabled = true;
            this.UseWaitCursor = false;
        }


    }
}
