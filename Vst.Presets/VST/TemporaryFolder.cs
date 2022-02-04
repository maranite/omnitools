using System;
using System.IO;

namespace Vst.Presets.VST
{
    public class TemporaryFolder : IDisposable
    {
        public TemporaryFolder()
        {
            var basePath = Path.GetTempFileName();
            File.Delete(basePath);
            if (Path.HasExtension(basePath))
                basePath = Path.ChangeExtension(basePath, null);
            Directory.CreateDirectory(basePath);
            DirectoryInfo = new DirectoryInfo(basePath);
            FullPath = basePath;
        }

        ~TemporaryFolder()
        {
            Dispose(false);
        }

        public readonly DirectoryInfo DirectoryInfo;
        public readonly string FullPath;
        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (DirectoryInfo.Exists)
                  Directory.Delete(DirectoryInfo.FullName,true);
                disposedValue = true;
            }
        }

        public static implicit operator DirectoryInfo(TemporaryFolder value)
        {
            return value.DirectoryInfo;
        }

        public override string ToString()
        {
            return this.FullPath;
        }
    }
}
