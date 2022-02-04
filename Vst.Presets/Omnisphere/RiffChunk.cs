using System;
using System.Collections.Generic;
using System.Text;

namespace Vst.Presets.Omnisphere
{
    public class RiffChunkData
    {
        private byte[] riffData;

        public string ChunkID { get; private set; }
        public int Length { get; private set; }
        public int ChunkOffset { get; private set; }
        public int DataOffset { get; private set; }

        public byte this[int i]
        {
            get => riffData[DataOffset + i];
            set => riffData[DataOffset + i] = value;
        }

        public static List<RiffChunkData> From(byte[] riffData)
        {
            Func<int, string> getString = (int offset) => Encoding.UTF8.GetString(riffData, offset, 4);

            var riff = getString(0);
            var wave = getString(8);
            if (riff != "RIFF" || wave != "WAVE")
                throw new FormatException("Not a WAVE file - no RIFF header");

            var result = new List<RiffChunkData>();

            for (var offset = 12; offset < riffData.Length;)
            {
                var id = getString(offset);
                var length = BitConverter.ToInt32(riffData, offset + 4);
                result.Add(new RiffChunkData
                {
                    ChunkID = id,
                    ChunkOffset = offset,
                    DataOffset = offset + 8,
                    Length = length,
                    riffData = riffData
                });
                offset += length + 8;
            }
            return result;
        }

        public void Get(int offset, out int value) => value = BitConverter.ToInt32(riffData, DataOffset + offset);
        public void Get(int offset, out short value) => value = BitConverter.ToInt16(riffData, DataOffset + offset);

        public void Set(int offset, int value)
        {
            var sourceArray = BitConverter.GetBytes(value);
            Array.Copy(sourceArray, 0, riffData, DataOffset + offset, sourceArray.Length);
        }

        public void Set(int offset, short value)
        {
            var sourceArray = BitConverter.GetBytes(value);
            Array.Copy(sourceArray, 0, riffData, DataOffset + offset, sourceArray.Length);
        }
    }

}
