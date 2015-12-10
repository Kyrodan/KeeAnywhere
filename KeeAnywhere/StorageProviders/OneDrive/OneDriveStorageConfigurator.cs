using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeePass.UI;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator
    {
        private readonly OneDriveConsumerApi _api;

        public OneDriveStorageConfigurator()
        {
            _api = new OneDriveConsumerApi(OneDriveHelper.OneDriveClientId, OneDriveHelper.OneDriveClientSecret);
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var oneDriveAuthenticateForm = new OneDriveAuthenticationForm(_api);
            var result = UIUtil.ShowDialogAndDestroy(oneDriveAuthenticateForm);

            if (result != DialogResult.OK)
            {
                return null;
            }

            var drive = await _api.GetDrive();

            var account = new AccountConfiguration
            {
                Type = StorageType.OneDrive,
                Name = drive.Owner.User.DisplayName,
                Id = drive.Id,
                RefreshToken = _api.AccessToken.RefreshToken
            };


            return account;
        }
    }
}