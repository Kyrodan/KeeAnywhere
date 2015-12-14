using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    static class GoogleDriveHelper
    {
        /*
          The consumer key and the secret key included here are dummy keys.
          You should go to https://console.developers.google.com/ to create your own application
          and get your own keys.

          This is done to prevent bots from scraping the keys from the source code posted on the web.

          Every now and then an accidental checkin of keys may occur, but these are all dummy applications
          created specifically for development that are deleted frequently and limited to the developer,
          never the real production keys.
        */

        //TODO: Change API keys!!!

        internal const string GoogleDriveClientId = "dummy";
        internal const string GoogleDriveClientSecret = "dummy";

        internal const string AuthUrl = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={0}&redirect_uri={1}&state={2}&scope={3}";
    }
}
