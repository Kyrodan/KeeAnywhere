using KeePass.App.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.Configuration
{
    public static class ConfigurationInfo
    {
        private static string _settingsDirectory;
        private static bool _isPortable;

        static ConfigurationInfo()
        {
            _isPortable = !KeePass.Program.Config.Meta.PreferUserConfiguration;

            if (_isPortable)
            {
                var asm = Assembly.GetEntryAssembly();
                var filename = asm.Location;
                _settingsDirectory = Path.GetDirectoryName(filename);
            }
            else
            {
                _settingsDirectory = AppConfigSerializer.AppDataDirectory;
            }
        }

        public static string SettingsDirectory
        {
            get
            {
                return _settingsDirectory;
            }
        }

        public static bool IsPortable
        {
            get
            {
                return _isPortable;
            }
        }
    }
}
