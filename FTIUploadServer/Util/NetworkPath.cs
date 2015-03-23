using System;
using System.IO;

namespace FTIUploadServer.Util
{
    /// <summary>
    /// Simple helper class to handle forward slashes and simple validation
    /// </summary>
    internal class NetworkPath
    {
        /// <summary>
        /// Simple combine for web paths similar to the standard C# Path combine, with simple path validation
        /// </summary>
        /// <param name="path1">string - root of url</param>
        /// <param name="path2">string - path to append</param>
        /// <returns>string - combined path</returns>
        public static string Combine(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                throw new ArgumentNullException(path1 == null ? "path1" : "path2");
            CheckInvalidPathChars(path1, false);
            CheckInvalidPathChars(path2, false);
            return CleanPath(CombineNoChecks(path1, path2));
        }

        /// <summary>
        /// Utility function to replace backslashes with forward slashes - intended for use on web urls
        /// </summary>
        /// <param name="path">string - path to operation one</param>
        /// <returns>string - path with forward slashes</returns>
        public static string CleanPath(string path)
        {
            return path == null ? null : path.Replace("\\", "/");
        }

        internal static void CheckInvalidPathChars(string path, bool checkAdditional = false)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (HasIllegalCharacters(path, checkAdditional))
                throw new ArgumentException("Argument_InvalidPathChars");
        }

        /// <summary>
        /// Check for invalid characters in file path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="checkAdditional"></param>
        /// <returns></returns>
        internal static bool HasIllegalCharacters(string path, bool checkAdditional)
        {
            foreach (char t in path)
            {
                int num = (int) t;
                switch (num)
                {
                    case 34:
                    case 60:
                    case 62:
                    case 124:
                        return true;
                    default:
                        if (num >= 32 && (!checkAdditional || num != 63 && num != 42))
                            continue;
                        goto case 34;
                }
            }
            return false;
        }

        /// <summary>
        /// Combine path 1 and path two for url paths
        /// </summary>
        /// <param name="path1">string - root of url</param>
        /// <param name="path2">string - path to append</param>
        /// <returns>string - combined path</returns>
        private static string CombineNoChecks(string path1, string path2)
        {
            if (path2.Length == 0)
                return path1;
            if (path1.Length == 0 || IsPathRooted(path2))
                return path2;

            path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            path1 = path1.TrimEnd(Path.AltDirectorySeparatorChar);
            return path1 + '/' + path2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPathRooted(string path)
        {
            if (path != null)
            {
                CheckInvalidPathChars(path, false);
                int length = path.Length;
                if (length >= 1 &&
                    ((int) path[0] == (int) Path.DirectorySeparatorChar ||
                     length >= 2 && (int) path[1] == (int) Path.VolumeSeparatorChar))
                    return true;
            }
            return false;
        }
    }
}