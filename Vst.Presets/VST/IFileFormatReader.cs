using System.IO;

namespace Vst.Presets.VST
{
    public interface IFileFormatReader<T>
    {
        T From(Stream stream);
    }
}