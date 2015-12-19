using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace KeeAnywhere
{
    /*
     * Original Copyright info:
     * 
     * UninstallInformations
     * 
     * Copyright (C) 2006-2010 Julien Roncaglia
     *
     * This library is free software; you can redistribute it and/or
     * modify it under the terms of the GNU Lesser General Public
     * License as published by the Free Software Foundation; either
     * version 2.1 of the License, or (at your option) any later version.
     *
     * This library is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
     * Lesser General Public License for more details.
     *
     * You should have received a copy of the GNU Lesser General Public
     * License along with this library; if not, write to the Free Software
     * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
    */

    public static class IconHelper
    {
        #region Custom exceptions class

        public class IconNotFoundException : Exception
        {
            public IconNotFoundException(string fileName, int index, Exception innerException)
                : base(string.Format("Icon with Id = {0} wasn't found in file {1}", index, fileName), innerException)
            {
            }
        }

        public class UnableToExtractIconsException : Exception
        {
            public UnableToExtractIconsException(string fileName, int firstIconIndex, int iconCount)
                : base(string.Format("Tryed to extract {2} icons starting from the one with id {1} from the \"{0}\" file but failed", fileName, firstIconIndex, iconCount))
            {
            }
        }

        #endregion

        #region DllImports

        /// <summary>
        /// Contains information about a file object. 
        /// </summary>
        struct SHFILEINFO
        {
            /// <summary>
            /// Handle to the icon that represents the file. You are responsible for
            /// destroying this handle with DestroyIcon when you no longer need it. 
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// Index of the icon image within the system image list.
            /// </summary>
            public IntPtr iIcon;

            /// <summary>
            /// Array of values that indicates the attributes of the file object.
            /// For information about these values, see the IShellFolder::GetAttributesOf
            /// method.
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            /// String that contains the name of the file as it appears in the Microsoft
            /// Windows Shell, or the path and file name of the file that contains the
            /// icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>
            /// String that describes the type of file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [Flags]
        enum FileInfoFlags : int
        {
            /// <summary>
            /// Retrieve the handle to the icon that represents the file and the index 
            /// of the icon within the system image list. The handle is copied to the 
            /// hIcon member of the structure specified by psfi, and the index is copied 
            /// to the iIcon member.
            /// </summary>
            SHGFI_ICON = 0x000000100,
            /// <summary>
            /// Indicates that the function should not attempt to access the file 
            /// specified by pszPath. Rather, it should act as if the file specified by 
            /// pszPath exists with the file attributes passed in dwFileAttributes.
            /// </summary>
            SHGFI_USEFILEATTRIBUTES = 0x000000010
        }

        /// <summary>
        ///     Creates an array of handles to large or small icons extracted from
        ///     the specified executable file, dynamic-link library (DLL), or icon
        ///     file. 
        /// </summary>
        /// <param name="lpszFile">
        ///     Name of an executable file, DLL, or icon file from which icons will
        ///     be extracted.
        /// </param>
        /// <param name="nIconIndex">
        ///     <para>
        ///         Specifies the zero-based index of the first icon to extract. For
        ///         example, if this value is zero, the function extracts the first
        ///         icon in the specified file.
        ///     </para>
        ///     <para>
        ///         If this value is �1 and <paramref name="phiconLarge"/> and
        ///         <paramref name="phiconSmall"/> are both NULL, the function returns
        ///         the total number of icons in the specified file. If the file is an
        ///         executable file or DLL, the return value is the number of
        ///         RT_GROUP_ICON resources. If the file is an .ico file, the return
        ///         value is 1. 
        ///     </para>
        ///     <para>
        ///         Windows 95/98/Me, Windows NT 4.0 and later: If this value is a 
        ///         negative number and either <paramref name="phiconLarge"/> or 
        ///         <paramref name="phiconSmall"/> is not NULL, the function begins by
        ///         extracting the icon whose resource identifier is equal to the
        ///         absolute value of <paramref name="nIconIndex"/>. For example, use -3
        ///         to extract the icon whose resource identifier is 3. 
        ///     </para>
        /// </param>
        /// <param name="phIconLarge">
        ///     An array of icon handles that receives handles to the large icons
        ///     extracted from the file. If this parameter is NULL, no large icons
        ///     are extracted from the file.
        /// </param>
        /// <param name="phIconSmall">
        ///     An array of icon handles that receives handles to the small icons
        ///     extracted from the file. If this parameter is NULL, no small icons
        ///     are extracted from the file. 
        /// </param>
        /// <param name="nIcons">
        ///     Specifies the number of icons to extract from the file. 
        /// </param>
        /// <returns>
        ///     If the <paramref name="nIconIndex"/> parameter is -1, the
        ///     <paramref name="phIconLarge"/> parameter is NULL, and the
        ///     <paramref name="phiconSmall"/> parameter is NULL, then the return
        ///     value is the number of icons contained in the specified file.
        ///     Otherwise, the return value is the number of icons successfully
        ///     extracted from the file. 
        /// </returns>
        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static int ExtractIconEx(
            [MarshalAs(UnmanagedType.LPTStr)] 
            string lpszFile,
            int nIconIndex,
            IntPtr[] phIconLarge,
            IntPtr[] phIconSmall,
            int nIcons);

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            FileInfoFlags uFlags);

        #endregion

        /// <summary>
        /// Two constants extracted from the FileInfoFlags, the only that are
        /// meaningfull for the user of this class.
        /// </summary>
        public enum SystemIconSize : int
        {
            Large = 0x000000000,
            Small = 0x000000001
        }

        /// <summary>
        /// Get the number of icons in the specified file.
        /// </summary>
        /// <param name="fileName">Full path of the file to look for.</param>
        /// <returns></returns>
        static int GetIconsCountInFile(string fileName)
        {
            return ExtractIconEx(fileName, -1, null, null, 0);
        }

        #region ExtractIcon-like functions

        public static void ExtractEx(string fileName, List<System.Drawing.Icon> largeIcons,
            List<System.Drawing.Icon> smallIcons, int firstIconIndex, int iconCount)
        {
            /*
             * Memory allocations
             */

            IntPtr[] smallIconsPtrs = null;
            IntPtr[] largeIconsPtrs = null;

            if (smallIcons != null)
            {
                smallIconsPtrs = new IntPtr[iconCount];
            }
            if (largeIcons != null)
            {
                largeIconsPtrs = new IntPtr[iconCount];
            }

            /*
             * Call to native Win32 API
             */

            int apiResult = ExtractIconEx(fileName, firstIconIndex, largeIconsPtrs, smallIconsPtrs, iconCount);
            if (apiResult != iconCount)
            {
                throw new UnableToExtractIconsException(fileName, firstIconIndex, iconCount);
            }

            /*
             * Fill lists
             */

            if (smallIcons != null)
            {
                smallIcons.Clear();
                foreach (IntPtr actualIconPtr in smallIconsPtrs)
                {
                    smallIcons.Add(System.Drawing.Icon.FromHandle(actualIconPtr));
                }
            }
            if (largeIcons != null)
            {
                largeIcons.Clear();
                foreach (IntPtr actualIconPtr in largeIconsPtrs)
                {
                    largeIcons.Add(System.Drawing.Icon.FromHandle(actualIconPtr));
                }
            }
        }

        public static List<System.Drawing.Icon> ExtractEx(string fileName, SystemIconSize size,
            int firstIconIndex, int iconCount)
        {
            List<Icon> iconList = new List<Icon>();

            switch (size)
            {
                case SystemIconSize.Large:
                    ExtractEx(fileName, iconList, null, firstIconIndex, iconCount);
                    break;

                case SystemIconSize.Small:
                    ExtractEx(fileName, null, iconList, firstIconIndex, iconCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("size");
            }

            return iconList;
        }

        public static void Extract(string fileName, List<System.Drawing.Icon> largeIcons, List<System.Drawing.Icon> smallIcons)
        {
            int iconCount = GetIconsCountInFile(fileName);
            ExtractEx(fileName, largeIcons, smallIcons, 0, iconCount);
        }

        public static List<System.Drawing.Icon> Extract(string fileName, SystemIconSize size)
        {
            int iconCount = GetIconsCountInFile(fileName);
            return ExtractEx(fileName, size, 0, iconCount);
        }

        public static System.Drawing.Icon ExtractOne(string fileName, int index, SystemIconSize size)
        {
            try
            {
                List<System.Drawing.Icon> iconList = ExtractEx(fileName, size, index, 1);
                return iconList[0];
            }
            catch (UnableToExtractIconsException e)
            {
                throw new IconNotFoundException(fileName, index, e);
            }
        }

        public static void ExtractOne(string fileName, int index,
            out System.Drawing.Icon largeIcon, out System.Drawing.Icon smallIcon)
        {
            var smallIconList = new List<System.Drawing.Icon>();
            var largeIconList = new List<System.Drawing.Icon>();
            try
            {
                ExtractEx(fileName, largeIconList, smallIconList, index, 1);
                largeIcon = largeIconList[0];
                smallIcon = smallIconList[0];
            }
            catch (UnableToExtractIconsException e)
            {
                throw new IconNotFoundException(fileName, index, e);
            }
        }

        #endregion

        static string GetExtensionIconStringFromKeyUsingDefaultIcon(RegistryKey key)
        {
            Debug.Assert(key != null);

            using (var defaultIconKey = key.OpenSubKey("DefaultIcon", false))
            {
                if (defaultIconKey == null) return null;

                var value = defaultIconKey.GetValue(null);
                if (value == null) return null;

                return value.ToString();
            }
        }

        static string GetExtensionIconStringFromKeyFromClsid(RegistryKey classesRootKey, string clsid)
        {
            Debug.Assert(classesRootKey != null);
            Debug.Assert(clsid != null);

            using (var clsidKey = classesRootKey.OpenSubKey("CLSID", false))
            {
                if (clsidKey == null) return null;

                using (var applicationClsidKey = clsidKey.OpenSubKey(clsid, false))
                {
                    if (applicationClsidKey == null) return null;

                    return GetExtensionIconStringFromKeyUsingDefaultIcon(applicationClsidKey);
                }
            }
        }

        static string GetExtensionIconStringFromKeyUsingClsid(RegistryKey key)
        {
            Debug.Assert(key != null);

            string applicationClsid;
            using (var clsidKey = key.OpenSubKey("CLSID", false))
            {
                if (clsidKey == null) return null;

                var value = clsidKey.GetValue(null);
                if (value == null) return null;

                applicationClsid = value.ToString();
            }

            using (var classesRootKey = Registry.ClassesRoot)
            {
                var fromNormalClsid = GetExtensionIconStringFromKeyFromClsid(classesRootKey, applicationClsid);
                if (fromNormalClsid != null) return fromNormalClsid;

                using (var wow6432ClassesRootKey = classesRootKey.OpenSubKey("Wow6432Node"))
                {
                    if (wow6432ClassesRootKey == null) return null;

                    return GetExtensionIconStringFromKeyFromClsid(wow6432ClassesRootKey, applicationClsid);
                }
            }
        }

        static string GetExtensionIconStringFromRegistry(string extension)
        {
            Debug.Assert(extension != null);
            Debug.Assert(extension.Length > 1);
            Debug.Assert(extension.StartsWith("."));

            using (var classesRootKey = Registry.ClassesRoot)
            using (var extensionKey = classesRootKey.OpenSubKey(extension, false))
            {
                if (extensionKey == null) return null;

                var fileTypeObject = extensionKey.GetValue(null);
                if (fileTypeObject == null) return null;

                using (var applicationKey = classesRootKey.OpenSubKey(fileTypeObject.ToString(), false))
                {
                    var result = GetExtensionIconStringFromKeyUsingClsid(applicationKey);
                    if (result == null)
                    {
                        result = GetExtensionIconStringFromKeyUsingDefaultIcon(applicationKey);
                    }

                    return result;
                }
            }
        }

        public static Icon IconFromExtensionUsingRegistry(string extension, SystemIconSize size)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            if (extension.Length == 0 || extension == ".") throw new ArgumentException("Empty extension", "extension");

            if (extension[0] != '.') extension = '.' + extension;

            var iconLocation = GetExtensionIconStringFromRegistry(extension);
            if (iconLocation == null) return null;

            return ExtractFromRegistryString(iconLocation, size);
        }

        public static Icon IconFromExtension(string extension, SystemIconSize size)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            if (extension.Length == 0 || extension == ".") throw new ArgumentException("Empty extension", "extension");

            if (extension[0] != '.') extension = '.' + extension;

            var fileInfo = new SHFILEINFO();
            SHGetFileInfo(extension, 0, out fileInfo, Marshal.SizeOf(fileInfo),
                FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size);

            return Icon.FromHandle(fileInfo.hIcon);
        }

        public static Icon IconFromResource(string resourceName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);

            return new Icon(stream);
        }

        /// <summary>
        /// Parse strings in registry who contains the name of the icon and
        /// the index of the icon an return both parts.
        /// </summary>
        /// <param name="regString">The full string in the form "path,index" as found in registry.</param>
        /// <param name="fileName">The "path" part of the string.</param>
        /// <param name="index">The "index" part of the string.</param>
        public static void ExtractInformationsFromRegistryString(
            string regString, out string fileName, out int index)
        {
            if (regString == null) throw new ArgumentNullException("regString");
            if (regString.Length == 0) throw new ArgumentException("Empty regString", "regString");

            index = 0;
            string[] strArr = regString.Replace("\"", "").Split(',');
            fileName = strArr[0].Trim();
            if (strArr.Length > 1)
            {
                int.TryParse(strArr[1].Trim(), out index);
            }
        }

        public static Icon ExtractFromRegistryString(string regString, SystemIconSize size)
        {
            string fileName;
            int index;
            ExtractInformationsFromRegistryString(regString, out fileName, out index);
            try
            {
                return ExtractOne(fileName, index, size);
            }
            catch (IconNotFoundException)
            {
                return null;
            }
        }
    }

}
