using System;
using System.IO;
using System.Linq;
using Vst.Presets.VST;
using Vst.Presets.Utilities;
using System.Runtime.InteropServices;

namespace Vst.Presets.Bitwig
{
    static class BinaryReaderExtensions
    {
        public static void ReadBitwig(this BinaryReader reader, out byte chunk, out object value)
        {
            value = null;
            chunk = 0;
            var nextByte = reader.PeekChar();
            if (nextByte > 0)
            {
                chunk = reader.ReadByte();
                switch ((ChunkType)chunk)
                {
                    case ChunkType.String:
                        value = reader.ReadBitwigString();
                        break;

                    case ChunkType.Strings:
                        var items = reader.ReadInt32BE();
                        value = Enumerable.Range(0, items).Select(c => reader.ReadBitwigString()).ToList();
                        break;

                    case ChunkType.Int32:
                    case ChunkType.Int32_2:
                    case ChunkType.Int32_3:
                        value = reader.ReadInt32BE();
                        break;

                    case ChunkType.Int16:
                        value = reader.ReadInt16BE();
                        break;

                    case ChunkType.Double:
                        value = reader.ReadDouble();
                        break;

                    case ChunkType.Byte:
                    case ChunkType.Byte2:
                        value = reader.ReadByte();
                        break;

                    case ChunkType.Bytes:
                        var items2 = reader.ReadInt32BE();
                        value = reader.ReadBytes(items2);
                        break;

                    case ChunkType.Flag_1:
                    case ChunkType.Flag_2:
                    case ChunkType.Unknown5:
                    case ChunkType.Unknown1:    // Need to investogate these??
                    case ChunkType.Unknown2:    // Need to investogate these??
                    case ChunkType.Unknown3:    // Need to investogate these??
                    case ChunkType.Unknown4:    // Need to investogate these??
                        break;

                    default:
                        var x = chunk; 
                        throw new PresetException("Unknown Tag encountered");
                }
            }
        }

        public static string ReadBitwigString(this BinaryReader reader)
        {
            var length = reader.ReadInt32BE(); // & 0xffff;
            return length == 0 ? string.Empty : reader.ReadString(length);
        }

        public static byte[] ReadBytesRequired(this BinaryReader reader, int byteCount)
        {
            var result = reader.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }

    }
}
