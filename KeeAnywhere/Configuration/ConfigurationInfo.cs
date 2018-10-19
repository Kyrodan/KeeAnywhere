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
            var isGlobalConfig = !KeePass.Program.Config.Meta.PreferUserConfiguration;
            var asm = Assembly.GetEntryAssembly();
            var filename = asm.Location;
            var directory = Path.GetDirectoryName(filename);

            _isPortable = isGlobalConfig 
                && !directory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) 
                && !directory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            if (_isPortable)
            {
                _settingsDirectory = directory;
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
