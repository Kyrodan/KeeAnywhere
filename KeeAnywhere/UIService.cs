using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.Forms;
using KeeAnywhere.StorageProviders;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    public class UIService
    {
        private readonly ConfigurationService _configService;
        private readonly StorageService _storageService;

        private bool _donationDialogAlreadyShownInThisUpgradedSession;

        public UIService(ConfigurationService configService, StorageService storageService)
        {
            _configService = configService;
            _storageService = storageService;
        }

        public async Task<AccountConfiguration> CreateOrUpdateAccount(StorageType type)
        {
            var newAccount = await _storageService.CreateAccount(type);
            if (newAccount == null) return null;

            var existingAccount = _configService.Accounts.SingleOrDefault(_ => _.Type == newAccount.Type && _.Id == newAccount.Id);
            if (existingAccount == null) // New Account
            {
                // ensure account's name is unique
                var i = 1;
                var name = newAccount.Name;
                while (
                    _configService.Accounts.Any(
                        _ => _.Type == newAccount.Type && _.Name.Equals(newAccount.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    i++;
                    newAccount.Name = string.Format("{0} ({1})", name, i);
                }

                _configService.Accounts.Add(newAccount);
                return newAccount;
            }

            MessageService.ShowInfo("This account already exists.\r\nUpdating account data only.");

            //existingAccount.Name = newAccount.Name;
            existingAccount.Secret = newAccount.Secret;

            return existingAccount;
        }

        public async Task CheckOrUpdateAccount(AccountConfiguration account)
        {
            var provider = _storageService.GetProviderByAccount(account);
            if (provider == null) return;

            try
            {
                var root = await provider.GetRootItem();
                var result = await provider.GetChildrenByParentItem(root);
                if (result != null)
                {
                    MessageService.ShowInfo("Connecting to this account succeeded.");
                    return;
                }

                MessageService.ShowWarning("Connecting to this account failed!", "Root folder could not be read.", "Try to re-authorize in next step.");
            }
            catch (Exception ex)
            {
                MessageService.ShowWarning("Connecting to this account failed!", ex, "Try to re-authorize in next step.");

            }

            var newAccount = await _storageService.CreateAccount(account.Type);
            if (newAccount == null)
            {
                MessageService.ShowWarning("Re-Authorization failed or cancelled by user!");
            }
            else if (newAccount.Id != account.Id)
            {
                MessageService.ShowWarning("Re-Authorization failed!", "The entered credentials do not belong to this account.");
            }
            else
            {
                account.Secret = newAccount.Secret;
                MessageService.ShowInfo("Re-Authorization succeeded!");
            }
        }

        public void ShowDonationDialog()
        {
            var lastShown = _configService.PluginConfiguration.DonationDialogLastShown;
            var isUpgraded = _configService.IsUpgraded && !_donationDialogAlreadyShownInThisUpgradedSession;

            if (isUpgraded)
            {
                _configService.PluginConfiguration.DonationDialogLastShown = DateTime.Today;
                return;
            }

            if (lastShown > DateTime.Today.AddMonths(-1))
                return;

            var dlg = new DonationForm();
            UIUtil.ShowDialogAndDestroy(dlg);

            _configService.PluginConfiguration.DonationDialogLastShown = dlg.IsDontShowMessageAgain
                ? DateTime.MaxValue
                : DateTime.Today;

            _donationDialogAlreadyShownInThisUpgradedSession = true;
        }

        public void ShowChangelog()
        {
            var version = this.GetVersionTag();
            var url = string.Format("https://github.com/Kyrodan/KeeAnywhere/blob/{0}/CHANGELOG.md", version);
            Process.Start(url);
        }

        private string GetVersionTag()
        {
            var tag = "master";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (versionAttribute != null)
                    tag = "v" + versionAttribute.InformationalVersion;

                if (tag.EndsWith("unstable"))
                    tag = "master";
            }
            catch
            {
                // ignored
            }

            return tag;
        }

        public void ShowSettingsDialog()
        {
            this.ShowDonationDialog();
            var form = new SettingsForm();
            form.InitEx(_configService, this);
            UIUtil.ShowDialogAndDestroy(form);
        }
    }
}