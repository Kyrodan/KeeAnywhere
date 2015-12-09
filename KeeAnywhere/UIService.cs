using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    public class UIService
    {
        private readonly ConfigurationService _configService;
        private readonly StorageService _storageService;

        public UIService(ConfigurationService configService, StorageService storageService)
        {
            _configService = configService;
            _storageService = storageService;
        }

        public async Task<AccountConfiguration> CreateOrUpdateAccount()
        {
            var newAccount = await _storageService.CreateAccount(StorageType.OneDrive);

            var existingAccount = _configService.Accounts.SingleOrDefault(_ => _.Id == newAccount.Id);
            if (existingAccount == null) // New Account
            {
                _configService.Accounts.Add(newAccount);
                return newAccount;
            }

            MessageService.ShowInfo("This account already exists.\r\nUpdating account data only.");

            existingAccount.Name = newAccount.Name;
            existingAccount.RefreshToken = newAccount.RefreshToken;

            return existingAccount;
        }
    }
}