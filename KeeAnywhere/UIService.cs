using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.Forms;
using KeeAnywhere.StorageProviders;
using KeeAnywhere.StorageProviders.OneDrive;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    public class UIService
    {
        private readonly ConfigurationService _configService;
        private readonly OneDriveService _oneDriveService;

        public UIService(ConfigurationService configService, OneDriveService oneDriveService)
        {
            _configService = configService;
            _oneDriveService = oneDriveService;
        }

        public async Task<AccountConfiguration> CreateOrUpdateAccount()
        {
            var api = _oneDriveService.GetApi();
            var oneDriveAuthenticateForm = new OneDriveAuthenticationForm(api);
            var result = UIUtil.ShowDialogAndDestroy(oneDriveAuthenticateForm);

            if (result != DialogResult.OK)
            {
                return null;
            }

            var drive = await api.GetDrive();
            var id = drive.Id;
            var name = drive.Owner.User.DisplayName;
            var refreshToken = api.AccessToken.RefreshToken;

            var account = _configService.Accounts.SingleOrDefault(_ => _.Id == id);
            if (account == null) // New Account
            {
                account = new AccountConfiguration
                {
                    Type = StorageProviderType.OneDrive,
                    Name = name,
                    Id = id,
                    RefreshToken = refreshToken,
                };

                _configService.Accounts.Add(account);
            }
            else // Account exists: updating data
            {
                MessageService.ShowInfo("This Account already exists.\r\nUpdating account data only.");

                account.Name = name;
                account.RefreshToken = refreshToken;
            }

            return account;
        }
    }
}
