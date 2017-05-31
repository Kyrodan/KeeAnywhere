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
    // Alternative to System.IO.Path class *without* checking for invalid chars. Required for Bug #23
    public static class CloudPath
    {
        public static char DirectorySeparatorChar = '/';
        
        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            CheckInvalidPathChars(path);

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

        private static void CheckInvalidPathChars(string path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                int c = path[i];

                // Note: This list is duplicated in static char[] InvalidPathChars 
                if (c < 32)
                    throw new ArgumentException("Invalid chars in path.");
            }
        }

        public static bool HasExtension(String path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path);
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
                CheckInvalidPathChars(path);

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
                CheckInvalidPathChars(path);

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

        public static String Combine(String path1, String path2)
        {
            if (path1 == null || path2 == null)
                throw new ArgumentNullException((path1 == null) ? "path1" : "path2");
            Contract.EndContractBlock();
            CheckInvalidPathChars(path1);
            CheckInvalidPathChars(path2);

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
