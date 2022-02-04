using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Vst.Presets.Utilities;

namespace Vst.Presets.Omnisphere
{
    [DebuggerDisplay("{RelativePath}")]
    public class OmnisphereFile
    {
        public OmnisphereFile(string relativePath, Func<Stream> fnOpen)
        {
            openStream = fnOpen;
            var ext = Path.GetExtension(relativePath).ToLowerInvariant();
            IsMulti = ext.StartsWith(".prt_");
            IsPatch = ext.StartsWith(".mlt_");
            IsWav = ext.StartsWith(".wav");
            RelativePath = relativePath;
        }

        private readonly Func<Stream> openStream;
        public readonly string RelativePath;
        public readonly bool IsMulti;
        public readonly bool IsPatch;
        public readonly bool IsWav;

        public Stream Open() => openStream();
        // public byte[] Contents => openStream().ReadToEnd(true);

        private byte[] contents;
        public byte[] Contents
        {
            get
            {
                if (contents == null)
                {
                    contents = openStream().ReadToEnd(true);
                    if (IsWav)
                        try
                        {
                            OmnisphereWavDecrypter.DecryptWavContents(contents);
                        }
                        catch (Exception)
                        {
                        }
                }
                return contents;
            }
        }

        public byte[] OriginalContents => openStream().ReadToEnd(true);

#if DEBUG
        public string StringContents => OpenXml().OuterXml;
#endif
        public XmlDocument OpenXml()
        {
            var contents = Encoding.ASCII.GetChars(Contents);
            var inQuote = false;
            for (var i = 0; i < contents.Length; i++)
            {
                switch (contents[i])
                {
                    case '"': inQuote = !inQuote; break;
                    case '<':
                    case '>': if (inQuote) contents[i] = '_'; break;
                }
            }
            var str = new string(contents);
            var xml = new XmlDocument();
            using (var reader = new XmlTextReader(new StringReader(str)) { DtdProcessing = DtdProcessing.Ignore })
                xml.Load(reader);
            return xml;
        }

        public OmnisphereMulti AsMulti()
        {
            var xml = OpenXml();
            var root = xml.DocumentElement;
            if (root != null)
            {
                if (IsMulti && root.Name == "SynthMaster")
                    return new OmnisphereMulti(xml);

                if (IsPatch && root.Name.EndsWith("Part"))
                    return new OmnisphereMulti(new OmnispherePatch(xml));
            }
            return null;
            //throw new PresetException("No valid Omnisphere Multi XML data found");
        }

        public OmnispherePatch AsPatch()
        {
            var xml = OpenXml();
            var root = xml.DocumentElement;
            if (root != null)
                if (root.Name.EndsWith("Part"))
                    return new OmnispherePatch(xml);
            return null;
            //throw new PresetException("No valid Omnisphere Multi XML data found");
        }
    }

}
