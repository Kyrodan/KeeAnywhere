using System;
using System.Drawing;
using KeePass.Plugins;

namespace KeeAnywhere
{
    public class KpResources
    {
        private IPluginHost _host;

        public KpResources(IPluginHost host)
        {
            _host = host;
        }

        public Bitmap B16x16_KeePass
        {
            get { return (Bitmap) _host.Resources.GetObject("B16x16_KeePass"); }
        }

        public Bitmap B16x16_Folder
        {
            get { return (Bitmap) _host.Resources.GetObject("B16x16_Folder"); }
        }

        public Bitmap B16x16_Binary
        {
            get { return (Bitmap) _host.Resources.GetObject("B16x16_Binary"); }
        }
    }
}