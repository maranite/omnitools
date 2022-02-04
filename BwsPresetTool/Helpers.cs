using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BwsPresetTool.Bitwig;
using BwsPresetTool.VST;

namespace BwsPresetTool
{
    static class StreamExtensions
    {
        public static MemoryStream ReadToMemory(this Stream stream, int bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            stream.Read(buffer, 0, bytesToRead);
            return new MemoryStream(buffer);
        }

        public static byte[] ReadToEnd(this Stream stream, bool dispose = false)
        {
            if (stream.CanSeek)
            {
                var size = stream.Length - stream.Position;
                var buffer = new byte[size];
                while (size > 0)
                    size -= stream.Read(buffer, 0, (int)size);

                if (dispose)
                    stream.Dispose();

                return buffer;
            }
            else
            {
                var stride = 102400;
                var buffer = new byte[stride];
                var offset = 0;
                while (true)
                {
                    offset += stream.Read(buffer, offset, stride);
                    if (offset < stride)
                    {
                        Array.Resize(ref buffer, offset);
                        return buffer;
                    }
                    Array.Resize(ref buffer, buffer.Length * 2);
                }
            }
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
                        break;

                    default:
                        throw new PresetException("Unknown Tag encountered");
                }
            }
        }

        public static int ReadAsciiHex(this BinaryReader reader, int size)
        {
            var str = reader.ReadString(size);
            return Convert.ToInt32(str, 16);
        }

        public static int ReadAsciiInt(this BinaryReader reader, int size)
        {
            var str = reader.ReadString(size);
            return Int32.Parse(str);
        }

        public static string ReadBitwigString(this BinaryReader reader)
        {
            var length = reader.ReadInt32BE(); // & 0xffff;
            return length == 0 ? string.Empty : reader.ReadString(length);
        }

        public static string ReadString(this BinaryReader reader, int size)
        {
            return new string(reader.ReadChars(size));
        }


        // Note this MODIFIES THE GIVEN ARRAY then returns a reference to the modified array.
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt16(reader.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static Int16 ReadInt16BE(this BinaryReader reader)
        {
            return BitConverter.ToInt16(reader.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToUInt32(reader.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader reader)
        {
            return BitConverter.ToInt32(reader.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
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
