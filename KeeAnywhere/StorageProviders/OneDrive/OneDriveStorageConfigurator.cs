using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeePass.UI;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var api = new OneDriveConsumerApi(OneDriveHelper.OneDriveClientId, OneDriveHelper.OneDriveClientSecret);

            var oneDriveAuthenticateForm = new OneDriveAuthenticationForm(api);
            var result = UIUtil.ShowDialogAndDestroy(oneDriveAuthenticateForm);

            if (result != DialogResult.OK)
            {
                return null;
            }

            var drive = await api.GetDrive();

            var account = new AccountConfiguration
            {
                Type = StorageType.OneDrive,
                Name = drive.Owner.User.DisplayName,
                Id = drive.Id,
                Secret = api.AccessToken.RefreshToken
            };


            return account;
        }
    }
}