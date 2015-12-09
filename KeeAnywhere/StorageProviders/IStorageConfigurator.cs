using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageConfigurator
    {
        Task<AccountConfiguration> CreateAccount();
    }
}