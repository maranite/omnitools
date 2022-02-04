using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Xml;
using Vst.Presets.Utilities;

namespace Vst.Presets.Omnisphere
{
    public interface IOmnisphereFileSource
    {
        IEnumerable<OmnisphereFile> Files { get; }
        string ContainerPath { get; }
    }



    public sealed class OmnisphereArchiveFile : IDisposable
    {
        public static void ExtractTo(FileInfo sourceFile, DirectoryInfo SaveTo)
        {
            using (var f = Open(sourceFile))
            {
                foreach (var file in f.Files)
                {
                    var ext = Path.GetExtension(file.RelativePath).ToLowerInvariant();
                    var contents = file.Contents;
                    if (ext == ".wav" && Encoding.UTF8.GetString(contents, 0, 4) != "RIFF")
                        continue;  // FLAC encoded files arent supported.

                    var filePath = new FileInfo(Path.Combine(SaveTo.FullName, file.RelativePath));
                    var wavFilePath = filePath.FullName;
                    filePath.EnsureDirectoryExists();
                    File.WriteAllBytes(wavFilePath, file.Contents);
                }

                //TODO: Add logic to insert SMPL and INST tags into wavs from HitBundle xmls 
            }
        }

        public static OmnisphereArchiveFile Open(FileInfo libraryFile)
        {
            if (!libraryFile.Exists)
                throw new FileNotFoundException($"{libraryFile.FullName} does not exist");

            if (libraryFile.Length < 50)
                throw new FileNotFoundException($"{libraryFile.FullName} is not a Spectrasonics library file");

            var extension = libraryFile.Extension.ToLowerInvariant();
            if (extension != ".omnisphere" && extension != ".db")
                throw new FileNotFoundException($"{libraryFile.FullName} is not a Spectrasonics library file");

            var files = new List<OmnisphereFile>();
            var mappedFiles = new List<MemoryMappedFile>();

            try
            {
                do
                {
                    var mappedFile = MemoryMappedFile.CreateFromFile(libraryFile.FullName);
                    mappedFiles.Add(mappedFile);
                    var sb = new StringBuilder();
                    long startOfFileData = 0;
                    XmlDocument xml;
                    using (var stream = mappedFile.CreateViewStream(0, Math.Min(1024 * 1024 * 32, libraryFile.Length), MemoryMappedFileAccess.Read))
                        GetFileTable(stream, out startOfFileData, out xml);

                    var root = xml.SelectSingleNode("//FileSystem");

                    var entries = (from XmlNode node in xml.SelectNodes("//*/FILE")
                                   select new
                                   {
                                       node,
                                       name = node.Attributes["name"].Value,
                                       ext = Path.GetExtension(node.Attributes["name"].Value.Trim()).ToLowerInvariant(),
                                       offset = Convert.ToInt64(node.Attributes["offset"].Value.Trim()),
                                       size = Convert.ToInt32(node.Attributes["size"].Value.Trim()),
                                       path = GetNodePath(node)
                                   }).ToArray();

                    //todo: add logic to look for xml tags and fixup the absolute offset.
                    var xmlEntry = entries.First(e => e.ext == ".xml"
                                                        || e.ext.StartsWith(".prt")
                                                        || e.ext.StartsWith(".mlt"));

                    using (var st2 = mappedFile.CreateViewStream(xmlEntry.offset + startOfFileData, 10, MemoryMappedFileAccess.Read))
                    {
                        using (var sr2 = new StreamReader(st2))
                        {
                            var txt = sr2.ReadToEnd();
                            for (var ix = 0; txt[ix] != '<' && ix < txt.Length; ix++)
                                startOfFileData++;
                        }
                    }

                    files.AddRange(from entry in entries
                                   select new OmnisphereFile
                                   (
                                       entry.path,
                                       () => mappedFile.CreateViewStream(entry.offset + startOfFileData, entry.size, MemoryMappedFileAccess.Read)
                                   ));

                } while (libraryFile.GetNextInSequence(out libraryFile));
            }
            catch
            {
                foreach (var file in mappedFiles)
                    file.Dispose();
                throw;
            }
            return new OmnisphereArchiveFile(mappedFiles.ToArray(), files.ToArray());
        }

        private static void GetFileTable(Stream stream, out long startOfFileData, out XmlDocument xml)
        {
            var sb = new StringBuilder();
            var seek = "</FileSystem>";
            var found = 0;
            var inQuote = false;
            startOfFileData = 0;
            while (found < seek.Length)
            {
                var thisChar = Convert.ToChar(stream.ReadByte());

                if (sb.Length == 0 && thisChar != '<')
                    continue;

                switch (thisChar)
                {
                    case '"': inQuote = !inQuote; break;
                    case '<':
                    case '>': if (inQuote) thisChar = '_'; break;
                }

                sb.Append(thisChar);

                if (thisChar == seek[found])
                    found++;
                else
                    found = 0;

                if (sb.Length > 1024 * 1024 * 32)
                    throw new FileNotFoundException($"File is not a Spectrasonics library file");
            }

            startOfFileData = stream.Position + 1;
            xml = new XmlDocument();
            xml.LoadXml(sb.ToString());
        }

        private static string GetNodePath(XmlNode node)
        {
            var name = node.Attributes["name"].Value.Trim();
            if (node.ParentNode != null && node.ParentNode.LocalName == "DIR")
                return $"{GetNodePath(node.ParentNode)}/{name}";
            return name;
        }

        private OmnisphereArchiveFile(MemoryMappedFile[] containers, OmnisphereFile[] files)
        {
            this.containers = containers;
            this.files = files;
        }

        private OmnisphereFile[] files;
        private MemoryMappedFile[] containers;

        public readonly FileInfo ContainerPath;
        public OmnisphereFile[] Files => files;
        public IEnumerable<OmnisphereMulti> Multis => from file in Files where file.IsMulti select file.AsMulti();
        public IEnumerable<OmnispherePatch> Patches => from file in Files where file.IsPatch select file.AsPatch();
        public IEnumerable<OmnisphereMulti> PatchesAsMultis => from file in Files where file.IsPatch select file.AsMulti();
        public IEnumerable<OmnisphereMulti> AllMultis => from file in Files where file.IsPatch || file.IsMulti select file.AsMulti();

        public void Dispose()
        {
            if (containers != null)
            {
                foreach (var c in containers)
                    c.Dispose();
                containers = null;
            }
            files = null;
        }
    }
}
