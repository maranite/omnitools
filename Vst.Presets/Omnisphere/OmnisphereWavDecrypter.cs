using System;
using System.Linq;

namespace Vst.Presets.Omnisphere
{
    internal static class OmnisphereWavDecrypter
    {
        public static void DecryptWavContents(byte[] contents)
        {
            var chunks = RiffChunkData.From(contents); // FindRiffChunks(contents);

            var fmtChunk = chunks.First(c => c.ChunkID == "fmt ");
            var channels = BitConverter.ToInt16(contents, fmtChunk.DataOffset + 2);
            byte[] xorPattern;
            switch (channels & 0xFFFC)
            {
                case 0:
                    return;

                case 0x28:
                    xorPattern = new byte[] { 0xF5, 0xA9, 0xF5, 0xAE };
                    break;

                case 0x14:
                    xorPattern = new byte[] { 0x82, 0x4A, 0x81, 0xDE };
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (xorPattern == null)
                return;

            contents[fmtChunk.DataOffset + 2] &= 3;
            var dataChunk = chunks.First(c => c.ChunkID == "data");

            for (int i = 0; i < dataChunk.Length; i++)
                dataChunk[i] ^= xorPattern[i % xorPattern.Length];
        }
    }

}
