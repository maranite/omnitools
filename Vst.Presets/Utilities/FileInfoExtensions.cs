using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    public static class FileInfoExtensions
    {
        public static bool GetNextInSequence(this FileInfo fileinfo, out FileInfo next)
        {
            next = null;
            var name = new StringBuilder(fileinfo.Name);
            var i = name.Length - 1;
            if (!Char.IsDigit(name[i]))
                return false;

            name[i]++;
            while (!Char.IsDigit(name[i]))
            {
                name[i--] = '0';
                if (i < 0 || !Char.IsDigit(name[i]))
                    return false;
                name[i]++;
            }
            next = new FileInfo(name.ToString());
            return next.Exists;
        }

        /// <summary>
        /// Returns the fileinfo for moving (rebasing) the file from a parent directory to a new directory.
        /// </summary>
        /// <param name="fileinfo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static FileInfo Rebase(this FileInfo fileinfo, DirectoryInfo from, DirectoryInfo to, string extension = null)
        {
            var path = fileinfo.FullName.Replace(from.ToWellFormed(), to.ToWellFormed());
            if (extension != null)
                path = Path.ChangeExtension(path, extension);
            return new FileInfo(path);
        }

        /// <summary>
        /// Ensures that the parent folder/s for a path exist
        /// </summary>
        /// <param name="path"></param>
        public static void EnsureDirectoryExists(this FileInfo path)
        {
            if (!path.Directory.Exists)
                path.Directory.Create();
        }

    }
}
