using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Vst.Presets.Utilities;

namespace Vst.Presets.VST
{
    public class FxPreset : IEquatable<FxPreset>
    {
        #region Magic Constants
        const uint CCnkMagic = 0x43636E4B;
        const uint FPChMagic = 0x46504368;
        const uint FBChMagic = 0x46424368;
        const uint FxCkMagic = 0x4678436B;
        const uint FxBkMagic = 0x4678426B;
        #endregion

        public string Name;
        public uint PluginID;
        public uint PluginVersion;
        public uint FormatVersion;
        public readonly byte[] Chunk;
        public readonly uint NumParamsOrProgs;

        public string PluginCode
        {
            get => Encoding.ASCII.GetString(BitConverter.GetBytes(PluginID).Reverse().ToArray());
            set => PluginID = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(value).Reverse().ToArray(), 0);
        }

        public FxPreset(string name, byte[] chunk, uint version, uint pluginID, uint pluginVer, uint numParams)
        {
            Name = name;
            FormatVersion = version;
            Chunk = chunk;
            this.PluginID = pluginID;
            PluginVersion = pluginVer;
            NumParamsOrProgs = numParams;
        }

        public FxPreset Clone(string name, byte[] chunk) => new FxPreset(name, chunk, FormatVersion, PluginID, PluginVersion, NumParamsOrProgs);

        public static Loader<FxPreset> Load = (Stream source) =>
        {
            using (var reader = new BinaryReader(source))
            {
                Func<Predicate<UInt32>, string, uint> ReadAssert = (isOK, errMsg) =>
                {
                    var val = reader.ReadUInt32BE();
                    if (!isOK(val)) throw new PresetException(errMsg);
                    return val;
                };

                var magic = ReadAssert(m => m == CCnkMagic, "Source data is not FXP / FXB");
                var size = reader.ReadUInt32BE();           // 0 for z3t2
                var fxMagic = reader.ReadUInt32BE();
                var version = ReadAssert(m => m == 1 || m == 2, "Unsupported or unknown FXP/FXB version");
                var pluginID = reader.ReadUInt32BE();
                var pluginVer = reader.ReadUInt32BE();
                byte[] chunk = null;
                var name = "";
                uint numParams = 0;

                switch (fxMagic)
                {
                    case FPChMagic:
                        numParams = reader.ReadUInt32BE();
                        var nameBytes = reader.ReadBytes(28);
                        name = Encoding.ASCII.GetString(nameBytes.TakeWhile(n => n >= 0x20).ToArray());
                        var bytes = (int)reader.ReadUInt32BE();
                        chunk = reader.ReadBytes(bytes);
                        break;

                    case FBChMagic:
                        var numPrograms = reader.ReadUInt32BE();
                        if (numPrograms == CCnkMagic)        // Array of FX Presets
                        {
                            throw new PresetException("Unable to load FXB/FXP data - unsupported sub-format");
                        }
                        else
                        {
                            reader.ReadBytes(128);
                            chunk = reader.ReadBytes((int)reader.ReadUInt32BE());
                        }
                        break;

                    default: throw new PresetException("Unable to load FXB/FXP data - possibly unsupported format");
                }
                Debug.Assert(reader.PeekChar() == -1);
                return new FxPreset(name, chunk, version, pluginID, pluginVer, numParams);
            }
        };

        public Saver ToFXB => AsFXB;
        public Saver ToFXP => AsFXP;
        public Saver ToPCHK => AsPCHK;

        void AsFXB(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteBE(CCnkMagic);                                          // long chunkMagic;		
                writer.WriteBE((uint)(Chunk.Length + 152));                         // long byteSize;		
                writer.WriteBE(FBChMagic);                                          // long fxMagic;		
                writer.WriteBE(FormatVersion);                                      // long version;
                writer.WriteBE(PluginID);                                           // long fxID;			
                writer.WriteBE(PluginVersion);                                      // long fxVersion;
                writer.WriteBE(NumParamsOrProgs);                                   // long numPrograms;
                for (int i = 0; i < 128; i++) writer.Write((byte)0);                // char future[128];
                writer.WriteBE(Chunk.Length);                                       // long chunkSize;
                writer.Write(Chunk);                                                // byte* data;	
            }
        }

        void AsFXP(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteBE(CCnkMagic);                                          // long chunkMagic;   
                writer.WriteBE((uint)(Chunk.Length + 60));                          // long byteSize;     
                writer.WriteBE(FPChMagic);                                          // long fxMagic;      
                writer.WriteBE(FormatVersion);                                      // long version;
                writer.WriteBE(PluginID);                                           // long fxID;         
                writer.WriteBE(PluginVersion);                                      // long fxVersion;
                writer.WriteBE(NumParamsOrProgs);                                   // long numParams;
                var pname = Encoding.ASCII.GetBytes(Name.Trim()).Take(28).ToArray();// char name[28];
                writer.Write(pname);
                for (int i = pname.Length; i < 28; i++)
                    writer.Write((byte)0);
                writer.WriteBE(Chunk.Length);                                       //    long chunkSize;
                writer.Write(Chunk);
            }
        }

        void AsPCHK(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((Int32)1);
                writer.Write(Chunk);
                if (Chunk[Chunk.Length - 1] != 0)
                    writer.Write((byte)0);
            }
        }

        public bool Equals(FxPreset other)
        {
            return other != null
                && PluginID == other.PluginID
                && PluginVersion == other.PluginVersion
                && FormatVersion == other.FormatVersion
                && NumParamsOrProgs == other.NumParamsOrProgs
                && Chunk.SequenceEqual(other.Chunk);
        }

    }
}
