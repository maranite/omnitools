using System;
using System.IO;
using System.Linq;
using Vst.Presets.Bitwig;
using Vst.Presets.VST;
using System.Runtime.InteropServices;
using System.Text;

namespace Vst.Presets.Utilities
{

    [Serializable]
    public class RiffException : Exception
    {
        public RiffException() { }
        public RiffException(string message) : base(message) { }
        public RiffException(string message, Exception inner) : base(message, inner) { }
        protected RiffException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    static class BinaryReaderExtensions
    {
        public static void ReadRiffChunk(this BinaryReader reader, string chunkId, out byte[] data)
        {
            reader.ReadRiffChunk(out string id, out data);
            if (!string.Equals(id, chunkId))
                throw new RiffException($"Unexpected section {id} encountered where {chunkId} expected.");
        }

        public static void ReadRiffChunk(this BinaryReader reader, out string chunkId, out byte[] data)
        {
            try
            {
                chunkId = reader.ReadString(4);
                var length = reader.ReadInt32();
                data = reader.ReadBytes(length);
                if ((length & 1) == 1 && (reader.BaseStream.Position < reader.BaseStream.Length))
                    reader.ReadByte();
            }
            catch (Exception inner)
            {
                throw new RiffException($"Invalid RIFF chunk encountered", inner);
            }
        }

        public static void ReadRiffChunk<T>(this BinaryReader reader, out string chunkId, out T obj) where T : struct
        {
            reader.ReadRiffChunk(out chunkId, out byte[] data);
            BitUtils.ReadStruct(data, out obj);
        }

        public static void ReadRiffChunk<T>(this BinaryReader reader, string chunkId, out T obj) where T : struct
        {
            reader.ReadRiffChunk(out string id, out byte[] data);
            if (!string.Equals(id, chunkId))
                throw new RiffException($"Unexpected section {id} encountered where {chunkId} expected.");
            BitUtils.ReadStruct(data, out obj);
        }

        public static void ReadRiffChunk(this BinaryReader reader, out string chunkId, out string str)
        {
            reader.ReadRiffChunk(out chunkId, out byte[] data);
            str = Encoding.ASCII.GetString(data);
        }

        public static void ReadRiffChunk(this BinaryReader reader, string chunkId, out string str)
        {
            reader.ReadRiffChunk(out string id, out byte[] data);
            if (!string.Equals(id, chunkId))
                throw new RiffException($"Unexpected section {id} encountered where {chunkId} expected.");
            str = Encoding.ASCII.GetString(data);
        }

        public static void ReadStruct<T>(this BinaryReader reader, out T result) where T : struct
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            BitUtils.ReadStruct(bytes, out result);
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


        public static string ReadString(this BinaryReader reader, int size)
        {
            return new string(reader.ReadChars(size));
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

        public static Int32 ReadInt24(this BinaryReader reader)
        {
            var bytes = new byte[4];
            reader.Read(bytes, 0, 3);
            if ((bytes[2] & 0x80) == 0x80)
                bytes[3] = 0xFF;

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
