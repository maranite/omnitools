using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Presets.Bitwig;
using System.Runtime.InteropServices;

namespace Vst.Presets.Utilities
{
    static class ByteUtils
    {
        public static int IndexOf(this byte[] data, params byte[] seek)
        {
            var end = data.Length - seek.Length;
            for (int i = 0; i <= end; i++)
            {
                int j = 0;
                for (; j < seek.Length && data[i + j] == seek[j]; j++) ;
                if (j == seek.Length)
                    return i;
            }
            return -1;
        }

        public static IEnumerable<int> IndexesOf(this byte[] data, params byte[] seek)
        {
            var end = data.Length - seek.Length;
            for (int i = 0; i <= end; i++)
            {
                int j = 0;
                for (; j < seek.Length && data[i + j] == seek[j]; j++) ;
                if (j == seek.Length)
                {
                    yield return i;
                    break;
                }
            }
        }
        
        public static byte[] Replace(this byte[] src, int index, int count, byte[] repl)
        {
            var dst = new byte[src.Length - count + repl.Length];
            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + count, dst, index + repl.Length, src.Length - (index + count));
            return dst;
        }

        public static T ReadSlice<T>(this byte[] buffer, int start, int count, Func<BinaryReader, T> read)
        {
            using (var reader = new BinaryReader(new MemoryStream(buffer, start, count), Encoding.ASCII))
                return read(reader);
        }
    }
}
