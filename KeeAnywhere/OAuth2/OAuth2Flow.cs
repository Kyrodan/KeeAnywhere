using System.Windows.Forms;
using KeePass.UI;

namespace KeeAnywhere.OAuth2
{
    public static class OAuth2Flow
    {
        public static bool TryAuthenticate(IOAuth2Provider provider)
        {
            var dlg = new OAuth2Form();
            dlg.InitEx(provider);
            var result = UIUtil.ShowDialogAndDestroy(dlg);

            return result == DialogResult.OK;
        }
    }
}