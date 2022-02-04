using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vst.Presets.Bitwig;
using System.Diagnostics;

namespace Vst.Presets.Utilities
{
    static class BinaryWriterExtensions
    {
        public static void WriteRiffChunk(this BinaryWriter writer, string chunkId, byte[] data)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(chunkId));
            Debug.Assert(chunkId.Length == 4);
            writer.Write(chunkId);
            writer.Write((int)data.Length);
            writer.Write(data);
            if ((data.Length & 1) == 1)
                writer.Write((byte)0);
        }

        public static void WriteRiffChunk<T>(this BinaryWriter writer, string chunkId, T obj) where T : struct
        {
            var bytes = BitUtils.StructureToBytes(obj);
            writer.WriteRiffChunk(chunkId, bytes);
        }

        public static void WriteRiffChunk(this BinaryWriter writer, string chunkId, string str)
        {
            writer.WriteRiffChunk(chunkId, Encoding.ASCII.GetBytes(str));
        }

        public static void WriteStruct<T>(this BinaryWriter writer, T value) where T : struct
        {
            var bytes = BitUtils.StructureToBytes(value);
            writer.Write(bytes);
        }

        public static void WriteAscii(this BinaryWriter writer, string text)
        {
            writer.Write(Encoding.ASCII.GetBytes(text));
        }

        public static void WriteAscii(this BinaryWriter writer, string format, params object[] args)
        {
            writer.WriteAscii(string.Format(format, args));
        }

        public static void WriteBE(this BinaryWriter writer, uint value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBE(this BinaryWriter writer, int value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBE(this BinaryWriter writer, ushort value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBE(this BinaryWriter writer, short value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBE(this BinaryWriter writer, ulong value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBE(this BinaryWriter writer, long value)
        {
            writer.Write(BitConverter.GetBytes(value).Reverse());
        }

        public static void WriteBitwig(this BinaryWriter writer, string value)
        {
            if (string.IsNullOrEmpty(value))
                writer.WriteBE((uint)0);
            else
            {
                writer.WriteBE((uint)value.Length);
                writer.WriteAscii(value);
            }
        }

        public static void WriteBitwig(this BinaryWriter writer, IEnumerable<string> svals)
        {
            if (svals == null)
                writer.WriteBE((uint)0);
            else
            {
                writer.WriteBE(svals.Count());
                foreach (var val in svals)
                {
                    writer.WriteBE(val.Length);
                    writer.WriteAscii(val);
                }
            }
        }

        public static void WriteBitwig(this BinaryWriter writer, byte[] bval)
        {
            if (bval == null)
                writer.WriteBE((uint)0);
            else
            {
                writer.WriteBE(bval.Length);
                writer.Write(bval);
            }
        }

        public static void WriteBitwig(this BinaryWriter writer, byte chunkType, object value)
        {
            if (chunkType <= 0)
                return;

            writer.Write(chunkType);

            if (value != null)
            {
                switch ((ChunkType)chunkType)
                {
                    case ChunkType.String:
                        writer.WriteBitwig(value as string);
                        break;

                    case ChunkType.Strings:
                        writer.WriteBitwig(value as IEnumerable<string>);
                        break;

                    case ChunkType.Bytes:
                        writer.WriteBitwig(value as byte[]);
                        break;

                    case ChunkType.Int32:
                    case ChunkType.Int32_2:
                    case ChunkType.Int32_3:
                        writer.WriteBE((int)value);
                        break;

                    case ChunkType.Int16:
                        writer.WriteBE((Int16)value);
                        break;

                    case ChunkType.Double:
                        writer.Write((double)value);
                        break;

                    case ChunkType.Byte:
                    case ChunkType.Byte2:
                        writer.Write((byte)value);
                        break;
                }
            }
        }


    }
}
