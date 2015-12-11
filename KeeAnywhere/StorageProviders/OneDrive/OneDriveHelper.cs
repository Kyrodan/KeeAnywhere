using System.Threading.Tasks;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveHelper
    {
        /*
         The consumer key and the secret key included here are dummy keys.
         You should go to https://dev.onedrive.com/ to create your own application
         and get your own keys.

         This is done to prevent bots from scraping the keys from the source code posted on the web.

         Every now and then an accidental checkin of keys may occur, but these are all dummy applications
         created specifically for development that are deleted frequently and limited to the developer,
         never the real production keys.
        */

        //TODO: Change API keys!!!

        internal const string OneDriveClientId = "dummy";
        internal const string OneDriveClientSecret = "dummy";

        //private readonly Dictionary<string, OneDriveApi> _cache = new Dictionary<string, OneDriveApi>(); 

        public static async Task<OneDriveConsumerApi> GetApi(string refreshToken)
        {
            var api = new OneDriveConsumerApi(OneDriveClientId, OneDriveClientSecret);
            await api.AuthenticateUsingRefreshToken(refreshToken);

            return api;
        }

        //public async Task<OneDriveApi> GetApi(AccountConfiguration account)
        //{

        //    if (account == null) throw new ArgumentNullException("account");

        //    var refreshToken = account.RefreshToken;

        //    if (string.IsNullOrEmpty(refreshToken)) return null;

        //    if (_cache.ContainsKey(refreshToken))
        //        return _cache[refreshToken];

        //    var api = await OneDriveApi.GetOneDriveApiFromRefreshToken(OneDriveClientId, OneDriveClientSecret, refreshToken);

        //    if (api != null)
        //        _cache.Add(refreshToken, api);

        //    return api;
        //}

        //public async Task<OneDriveApi> TryGetApi(AccountConfiguration account)
        //{

        //    try
        //    {
        //        if (account.RefreshToken != null)
        //        {
        //            var api = await GetApi(account);
        //            return api;
        //        }
        //    }
        //    catch (WebException)
        //    {
        //    }

        //    while (true)
        //    {
        //        try
        //        {
        //            var api = GetApi();
        //            var oneDriveAuthenticateForm = new OneDriveAuthenticationForm(api);
        //            var result = UIUtil.ShowDialogAndDestroy(oneDriveAuthenticateForm);

        //            if (result != DialogResult.OK)
        //            {
        //                return null;
        //            }

        //            var drive = await api.GetDrive();
        //            var id = drive.Id;
        //            //var name = drive.Owner.User.DisplayName;
        //            var refreshToken = api.AccessToken.RefreshToken;

        //            if (account.Id == id)
        //            {
        //                account.RefreshToken = refreshToken;
        //                return api;
        //            }
        //            else
        //            {
        //                result =
        //                    MessageService.Ask(
        //                        "The authenticated account does not belog to the requested account.\r\nWould you like to retry?",
        //                        "KeeAnywhere Error", MessageBoxButtons.RetryCancel);

        //                if (result == DialogResult.Cancel)
        //                    return null;
        //            }
        //        }
        //        catch (WebException)
        //        {
        //            var result =
        //                MessageService.Ask(
        //                    "Authentication error.\r\nWould you like to retry?",
        //                    "KeeAnywhere Error", MessageBoxButtons.RetryCancel);

        //            if (result == DialogResult.Cancel)
        //                return null;
        //        }
        //    }
        //}
    }
}