using ACD = Azi.Amazon.CloudDrive;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azi.Amazon.CloudDrive.JsonObjects;

namespace KeeAnywhere.StorageProviders.AmazonDrive
{
    public static class AmazonDriveHelper
    {
        /*
        The consumer key and the secret key included here are dummy keys.
        To get your own keys you need to:
        * Register a Security Profile: https://developer.amazon.com/iba-sp/overview.html
        * Enable "Login with Amazon": https://developer.amazon.com/lwa/sp/overview.html
        * Whitelist your security Profile for Amazon Drive: https://developer.amazon.com/cd/sp/overview.html

        This is done to prevent bots from scraping the keys from the source code posted on the web.

        Every now and then an accidental checkin of keys may occur, but these are all dummy applications
        created specifically for development that are deleted frequently and limited to the developer,
        never the real production keys.
       */

        //TODO: Change API keys!!!
        internal const string AmazonDriveClientId = "dummy";
        internal const string AmazonDriveClientSecret = "dummy";

        public static ACD.AmazonDrive GetApi()
        {
            var api = new ACD.AmazonDrive(AmazonDriveClientId, AmazonDriveClientSecret)
            {
                Proxy = ProxyTools.GetProxy()
            };

            return api;
        }

        public static async Task<AmazonNode> GetNodeByPath(this Azi.Amazon.CloudDrive.IAmazonDrive api, string path)
        {
            var parts = path.Split('/');
            AmazonNode node = null;
            string parent = null;

            foreach (var part in parts)
            {
                node = await api.Nodes.GetChild(parent, part);

                if (node == null) return null;
                parent = node.id;
            }

            return node;
        }


    }
}
