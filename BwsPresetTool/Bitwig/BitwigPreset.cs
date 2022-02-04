using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using BwsPresetTool.VST;

namespace BwsPresetTool.Bitwig
{
    class BitwigPreset : IEquatable<BitwigPreset>
    {
        const UInt32 partTwoHeader = 0x00000561;
        const string metaId = "meta";
        const string magic = "BtWg";

        FxPreset preset;
        public readonly Version Version; //= new Version(1, 2, 79);
        public readonly HeaderItem[] HeaderItems;
        public readonly MetaItem[] MetaItems;
        public readonly MetaItem[] MetaItems2;
        public int HeaderPadding;

        private BitwigPreset(Version version, HeaderItem[] headers, MetaItem[] metas, FxPreset preset, int padding)
        {
            Version = version;
            HeaderItems = headers;
            MetaItems = metas;
            MetaItems2 = MetaItems.Where(m => m.DataType == ChunkType.String).ToArray();
            this.preset = preset;
            HeaderPadding = padding;
        }

        public static BitwigPreset From(string path)
        {
            try
            {

                using (var stream = File.OpenRead(path))
                    return From(stream.ReadToEnd());

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BitwigPreset From(Stream stream)
        {
            return From(stream.ReadToEnd());
        }

        public static BitwigPreset From(byte[] buffer)
        {
            int metaStart, fxStart;
            Version Version;
            var headers = new List<HeaderItem>();
            int padding = 0;
            var metas = new List<MetaItem>();
            FxPreset preset;

            using (var reader = new BinaryReader(new MemoryStream(buffer, 0, 40), Encoding.ASCII))
            {
                if (magic != new string(reader.ReadChars(4)))
                    throw new BadImageFormatException();

                Version = new Version(reader.ReadAsciiInt(4), reader.ReadAsciiInt(4), reader.ReadAsciiInt(4));
                Debug.Assert(Version.Equals(new Version(1, 2, 79)));

                metaStart = reader.ReadAsciiHex(8);
                reader.ReadAsciiHex(8);
                fxStart = reader.ReadAsciiHex(8);
            }

            using (var reader = new BinaryReader(new MemoryStream(buffer, 40, metaStart - 40), Encoding.ASCII))
            {
                var metaNum = reader.ReadInt32BE(); Debug.Assert(metaNum == 4);
                var metaTag = reader.ReadBitwigString(); Debug.Assert(metaTag == metaId);

                while (reader.ReadInt32BE() == 1)
                {
                    var label = reader.ReadBitwigString();
                    reader.ReadBitwig(out byte chunk, out object value);
                    headers.Add(new HeaderItem(label, (ChunkType)chunk, value));
                }

                Debug.Assert(0x20 == reader.PeekChar());

                while (reader.ReadByte() == 0x20)
                    padding++;

                Debug.Assert(reader.PeekChar() < 0);
                Debug.Assert(reader.BaseStream.Position >= reader.BaseStream.Length);
            }

            using (var reader = new BinaryReader(new MemoryStream(buffer, metaStart, fxStart - metaStart), Encoding.ASCII))
            {
                var header = reader.ReadUInt32BE();
                Debug.Assert(header == partTwoHeader);
                while (true)
                {
                    var key = reader.ReadUInt32BE();
                    reader.ReadBitwig(out byte chunk, out object value);
                    metas.Add(new MetaItem(key, (ChunkType)chunk, value));
                    if (reader.PeekChar() < 0 || reader.BaseStream.Position >= reader.BaseStream.Length)
                        break;
                }
            }

            using (var zippedFx = new ZipArchive(new MemoryStream(buffer, fxStart, buffer.Length - fxStart), ZipArchiveMode.Read, false))
            {
                var entry = zippedFx.Entries.FirstOrDefault();
                if (entry == null)
                    throw new PresetException("No zipped FXB found in bwPreset");

                using (var stream = entry.Open())
                    preset = FxPreset.From(stream);
            }

            return new BitwigPreset(Version, headers.ToArray(), metas.ToArray(), preset, padding);
        }

        public byte[] ToArray()
        {
            using (var ms = new MemoryStream())
            {
                Write(ms);
                return ms.ToArray();
            }
        }

        public void Write(Stream stream)
        {
            byte[] headerBytes;
            using (var headerInfo = new MemoryStream())
            using (var writer = new BinaryWriter(headerInfo, Encoding.ASCII, true))
            {
                writer.WriteBE((uint)4);
                writer.WriteBitwig(metaId);
                foreach (var item in HeaderItems)
                {
                    writer.WriteBE((uint)1);
                    writer.WriteBitwig(item.Label);
                    writer.WriteBitwig((byte)item.DataType, item.Value);
                }
                writer.WriteBE((uint)0);
                for (int i = 0; i < HeaderPadding; i++)
                    writer.Write((byte)0x20);

                writer.Write((byte)0x0A);
                headerBytes = headerInfo.ToArray();
            }

            byte[] metaBytes;
            using (var metaInfo = new MemoryStream())
            using (var writer = new BinaryWriter(metaInfo, Encoding.ASCII, true))
            {
                writer.WriteBE(partTwoHeader);
                foreach (var item in MetaItems)
                {
                    writer.WriteBE(item.MetaTag);
                    if (item.DataType > 0)
                        writer.WriteBitwig((byte)item.DataType, item.Value);
                }
                metaBytes = metaInfo.ToArray();
            }

            byte[] fxBytes;
            using (var fxInfo = new MemoryStream())
            {
                using (var zippedFx = new ZipArchive(fxInfo, ZipArchiveMode.Create))
                {
                    var data = preset.ToFXB();
                    var entry = zippedFx.CreateEntry("plugin-states/" + PresetFileName);
                    using (var e = entry.Open())
                        e.Write(data, 0, data.Length);
                }
                fxBytes = fxInfo.ToArray();
            }

            var metaStart = headerBytes.Length + 40;
            var fxbOffset = metaStart + metaBytes.Length;

            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                writer.WriteAscii(magic);
                writer.WriteAscii("{0:0000}{1:0000}{2:0000}", Version.Major, Version.Minor, Version.Build);
                writer.WriteAscii(metaStart.ToString("x8"));
                writer.WriteAscii("00000000");
                writer.WriteAscii(fxbOffset.ToString("x8"));
                writer.Write(headerBytes);
                writer.Write(metaBytes);
                writer.Write(fxBytes);
            }
        }

        public FxPreset VstPreset
        {
            get => preset;
            set
            {
                preset = value;
                var uuid = Guid.NewGuid();
                preset.Name = string.Format("{0:D}.fxb", uuid);
                SetMetaTag(MetaTag.PresetFileName, preset.Name);
            }
        }

        public void SetHeader(string label, object value)
        {
            var comp = StringComparer.InvariantCultureIgnoreCase;
            foreach (var item in HeaderItems)
                if (comp.Compare(item.Label, label) == 0)
                {
                    item.Value = value;
                    return;
                }
        }

        public void SetMetaTag(MetaTag tag, object value)
        {
            foreach (var item in MetaItems)
                if (item.MetaTag == (uint)tag)
                {
                    item.Value = value;
                    return;
                }
        }

        HeaderItem GetHeader(string label)
        {
            var comp = StringComparer.InvariantCultureIgnoreCase;
            foreach (var item in HeaderItems)
                if (comp.Compare(item.Label, label) == 0)
                    return item;
            return null;
        }

        MetaItem GetMetaTag(MetaTag tag)
        {
            foreach (var item in MetaItems)
                if (item.MetaTag == (uint)tag)
                    return item;
            return null;
        }

        public bool Equals(BitwigPreset other)
        {
            return preset.Equals(other.preset)
                //&& Author.Equals(other.Author)
                //&& Comment.Equals(other.Comment)
                && PresetName.Equals(other.PresetName);
        }

        public IEnumerable<string> Tags
        {
            get
            {
                var htags = GetHeader("tags")?.Value as string ?? "";
                var mtags = GetMetaTag(MetaTag.Tags)?.Value as string ?? "";
                return htags.Split(' ').Union(mtags.Split(' ')).Select(c => c.Replace('_', ' ')).Distinct().ToArray();
            }
            set
            {
                var val = string.Join(" ", value.Select(v => v.Replace(' ', '_')) ?? Enumerable.Empty<string>());
                SetHeader("tags", val);
                SetMetaTag(MetaTag.Tags, val);
            }
        }

        public string Author
        {
            get
            {
                var htags = GetHeader("creator")?.Value as string;
                var mtags = GetMetaTag(MetaTag.Author)?.Value as string ?? "";
                return string.IsNullOrWhiteSpace(htags) ? mtags : htags;
            }
            set
            {
                SetHeader("creator", value);
                SetMetaTag(MetaTag.Author, value);
            }
        }

        public string Category
        {
            get
            {
                var htags = GetHeader("preset_category")?.Value as string;
                var mtags = GetMetaTag(MetaTag.Category)?.Value as string ?? "";
                return string.IsNullOrWhiteSpace(htags) ? mtags : htags;
            }
            set
            {
                SetHeader("preset_category", value);
                SetMetaTag(MetaTag.Category, value);
            }
        }

        public string Comment
        {
            get
            {
                var htags = GetHeader("comment")?.Value as string;
                var mtags = GetMetaTag(MetaTag.Comment)?.Value as string ?? "";
                return string.IsNullOrWhiteSpace(htags) ? mtags : htags;
            }
            set
            {
                SetHeader("comment", value);
                SetMetaTag(MetaTag.Comment, value);
            }
        }

        public string PresetName
        {
            get
            {
                return GetMetaTag(MetaTag.PresetName)?.Value as string ?? "";
            }
            set
            {
                SetMetaTag(MetaTag.PresetName, value);
            }
        }

        public string PresetFileName
        {
            get
            {
                return GetMetaTag(MetaTag.PresetFileName)?.Value as string ?? "";
            }
            set
            {
                SetMetaTag(MetaTag.PresetFileName, value);
            }
        }
    }
}



