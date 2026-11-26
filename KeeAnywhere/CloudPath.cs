using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere
{
    // Alternative to System.IO.Path class *without* checking for invalid chars. Required for Bug #23, #322, #325, #346:
    // cloud storage providers allow chars in names that are invalid in local paths (e.g. tab or newline in Google Drive)
    public static class CloudPath
    {
        public static char DirectorySeparatorChar = '/';
        
        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            int length = path.Length;
            for (int i = length; --i >= 0;)
            {
                char ch = path[i];
                if (ch == '.')
                {
                    if (i != length - 1)
                        return path.Substring(i, length - i);
                    else
                        return string.Empty;
                }
                if (ch == DirectorySeparatorChar)
                    break;
            }
            return string.Empty;
        }

        public static bool HasExtension(String path)
        {
            if (path != null)
            {
                for (int i = path.Length; --i >= 0;)
                {
                    char ch = path[i];
                    if (ch == '.')
                    {
                        if (i != path.Length - 1)
                            return true;
                        else
                            return false;
                    }
                    if (ch == DirectorySeparatorChar) break;
                }
            }
            return false;
        }

        public static String ChangeExtension(String path, String extension)
        {
            if (path != null)
            {
                String s = path;
                for (int i = path.Length; --i >= 0;)
                {
                    char ch = path[i];
                    if (ch == '.')
                    {
                        s = path.Substring(0, i);
                        break;
                    }
                    if (ch == DirectorySeparatorChar) break;
                }
                if (extension != null && path.Length != 0)
                {
                    if (extension.Length == 0 || extension[0] != '.')
                    {
                        s = s + ".";
                    }
                    s = s + extension;
                }
                return s;
            }
            return null;
        }

        public static String GetDirectoryName(String path)
        {
            if (path != null)
            {
                int i = path.Length;
                while (i > 0 && path[--i] != DirectorySeparatorChar) ;
                return path.Substring(0, i);
            }
            return null;
        }

        public static String GetFileName(String path)
        {
            if (path != null)
            {
                int length = path.Length;
                for (int i = length; --i >= 0;)
                {
                    char ch = path[i];
                    if (ch == DirectorySeparatorChar)
                        return path.Substring(i + 1, length - i - 1);

                }
            }
            return path;
        }

        public static String GetFileNameWithoutExtension(String path)
        {
            path = GetFileName(path);
            if (path != null)
            {
                int i = path.LastIndexOf('.');
                if (i >= 0)
                    return path.Substring(0, i);
            }
            return path;
        }

        public static String Combine(String path1, String path2)
        {
            if (path1 == null || path2 == null)
                throw new ArgumentNullException((path1 == null) ? "path1" : "path2");
            Contract.EndContractBlock();
            if (path2.Length == 0)
                return path1;

            if (path1.Length == 0)
                return path2;

            char ch = path1[path1.Length - 1];

            if (ch != DirectorySeparatorChar)
                return path1 + DirectorySeparatorChar + path2;

            return path1 + path2;
        }

        public static string MaskInvalidFileNameChars(string path)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(path.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        }
    }
}
