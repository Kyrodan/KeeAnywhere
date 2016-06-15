using System.Windows.Forms;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere.OAuth2
{
    public static class OAuth2Flow
    {
        public static bool TryAuthenticate(IOAuth2Provider provider)
        {
            var dlg = new OAuth2Form();
            dlg.InitEx(provider);
            var result = UIUtil.ShowDialogAndDestroy(dlg);

            if (result == DialogResult.Abort) // Faulted - no user cancellation
            {
                MessageService.ShowFatal("Authentication failed!", dlg.LastException);
            }

            return result == DialogResult.OK;
        }
    }
}