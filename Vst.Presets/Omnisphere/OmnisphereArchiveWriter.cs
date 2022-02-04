using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Vst.Presets.Omnisphere
{
    public class OmnisphereArchiveWriter : IDisposable
    {
        private string tempPath;
        private FileStream tempFile;
        private XmlDocument thisDocument;
        private string TargetPath;

        public OmnisphereArchiveWriter(string targetPath)
        {
            TargetPath = targetPath;
            thisDocument = new XmlDocument();
            thisDocument.AppendChild(thisDocument.CreateElement("FileSystem"));
            tempPath = Path.GetTempFileName();
            tempFile = File.Open(tempPath, FileMode.Create);
        }

        public void AddFile(string relativePath, byte[] contents)
        {
            if (disposedValue)
                throw new ObjectDisposedException("Cannot add files after object has been disposed");

            if (relativePath == null || contents == null)
                throw new ArgumentNullException("FilePath and Contents must be specified");

            if (Path.IsPathRooted(relativePath))
                throw new ArgumentNullException("Absolute file paths are not supported. Provide a FilePath that is relative to the root of the library.");

            XmlElement parentNode = thisDocument.DocumentElement;
            var pathParts = relativePath.Split('\\');
            for (int i = 0; i < pathParts.Length; i++)
            {
                if (string.IsNullOrEmpty(pathParts[i])) continue;
                var dirNode = parentNode.SelectSingleNode($"DIR[@name=\"{pathParts[i]}\"]") as XmlElement;
                if (dirNode == null)
                {
                    dirNode = thisDocument.CreateElement(i < (pathParts.Length - 1) ? "DIR" : "FILE");
                    var nameAttr = thisDocument.CreateAttribute("name");
                    nameAttr.Value = pathParts[i];
                    dirNode.Attributes.Append(nameAttr);
                    parentNode.AppendChild(dirNode);
                }
                parentNode = dirNode;
            }

            var offs = thisDocument.CreateAttribute("offset");
            offs.Value = tempFile.Length.ToString();
            parentNode.Attributes.Append(offs);

            var size = thisDocument.CreateAttribute("size");
            size.Value = contents.Length.ToString();
            parentNode.Attributes.Append(size);

            var name = thisDocument.CreateAttribute("name");
            name.Value = Path.GetFileName(relativePath);
            parentNode.Attributes.Append(name);

            tempFile.Write(contents, 0, contents.Length);
        }

        public void Save()
        {
            if (disposedValue)
                throw new ObjectDisposedException(GetType().Name);

            var info = new FileInfo(TargetPath);
            if (!info.Directory.Exists)
                info.Directory.Create();

            if (info.Exists)
                info.Delete();

            using (var fileStream = info.OpenWrite())
            {
                var settings = new XmlWriterSettings
                {
                    NewLineChars = "", //Encoding.ASCII.GetString(new byte[] { 0x0A }),
                    NewLineHandling = NewLineHandling.Replace,
                    NewLineOnAttributes = false,
                    IndentChars = "",
                    Indent = false,
                    CloseOutput = false,
                    Encoding = Encoding.ASCII,
                    OmitXmlDeclaration = true 
                };

                using (var writer = XmlWriter.Create(fileStream, settings))
                    thisDocument.WriteTo(writer);

                fileStream.WriteByte(0x0a);

                tempFile.Seek(0, SeekOrigin.Begin);
                tempFile.CopyTo(fileStream);
            }
            Dispose();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    thisDocument = null;
                    if (tempFile != null)
                    {
                        tempFile.Dispose();
                        File.Delete(tempFile.Name);
                        tempFile = null;
                    }
                    File.Delete(tempPath);
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
