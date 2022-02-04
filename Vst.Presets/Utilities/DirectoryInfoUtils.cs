using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Vst.Presets.Utilities
{
    public static class DirectoryInfoUtils
    {
        public struct RelativeFileInfo
        {
            public RelativeFileInfo(DirectoryInfo baseFolder, FileInfo source)
            {
                Source = source;
                BaseFolder = baseFolder;
            }

            public readonly DirectoryInfo BaseFolder;
            public readonly FileInfo Source;
            public string RelativePath => Source.FullName.Replace(BaseFolder.FullName, "");

            public FileInfo Rebase(DirectoryInfo newBase, string newExtension = null)
                => Rebase(newBase.FullName, newExtension);

            public FileInfo Rebase(string newBase, string newExtension = null) =>
                new FileInfo(Path.Combine(newBase,
                    newExtension == null ? RelativePath
                                         : Path.ChangeExtension(RelativePath, newExtension)));
        }

        public static IEnumerable<RelativeFileInfo> EnumerateFromBase(
            this DirectoryInfo source,
            string searchPattern = "*.*",
            SearchOption options = SearchOption.AllDirectories)
            => from src in source.EnumerateFiles(searchPattern, options)
               select new RelativeFileInfo(source, src);

        public struct FileInfoTuple
        {
            public readonly FileInfo Source;
            public readonly FileInfo Target;
            public FileInfoTuple(FileInfo source, FileInfo target)
            {
                Source = source;
                Target = target;
            }
        }

        public static IEnumerable<FileInfoTuple> EnumerateAndRebase(
            this DirectoryInfo source,
            DirectoryInfo target,
            string searchPattern = "*.*",
            string newExtension = null,
            SearchOption options = SearchOption.AllDirectories)
        {
            var fromPath = source.ToWellFormed();
            var toPath = target.ToWellFormed();

            return from src in source.EnumerateFiles(searchPattern, options)
                   let newPath = src.FullName.Replace(fromPath, toPath)
                   let targetPath = (newExtension == null) ? newPath : Path.ChangeExtension(newPath, newExtension)
                   select new FileInfoTuple(src, new FileInfo(targetPath));
        }


        public static string ToWellFormed(this DirectoryInfo source)
        {
            var path = source.FullName;
            if (path[path.Length - 1] == Path.PathSeparator)
                return path;
            var sb = new StringBuilder(source.FullName);
            sb.Append(Path.PathSeparator);
            return sb.ToString();
        }

        public static DirectoryInfo EnsureWellFormed(this DirectoryInfo source)
        {
            var path = source.FullName;
            if (path[path.Length - 1] == Path.PathSeparator)
                return source;
            var sb = new StringBuilder(source.FullName);
            sb.Append(Path.PathSeparator);
            return new DirectoryInfo(source.ToWellFormed());
        }





        public static string MakeRelative(this DirectoryInfo currentBase, FileInfo file)
        {
            return file.FullName.Replace(currentBase.FullName, "");
        }

        public static FileInfo Rebase(this DirectoryInfo @base, FileInfo file, DirectoryInfo newBase)
        {
            return new FileInfo(Path.Combine(
                     newBase.FullName,
                     file.FullName.Replace(@base.FullName, "")));
        }

        public static DirectoryInfo Rebase(this DirectoryInfo @base, DirectoryInfo subFolder, DirectoryInfo newBase)
        {
            if (!subFolder.FullName.Contains(@base.FullName) && @base.FullName.EndsWith("\\"))
                @base = new DirectoryInfo(@base.FullName.Substring(0, @base.FullName.Length - 1));

            return new DirectoryInfo(Path.Combine(
                     newBase.FullName,
                     subFolder.FullName.Replace(@base.FullName, "")));
            //C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\Distortion
            //C:\ProgramData\Spectrasonics\STEAM\Omnisphere\Soundsources\Factory\Core Library\
        }
    }
}
