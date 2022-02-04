using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Vst.Presets.VST;

namespace Vst.Presets.Utilities
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


        /// <summary>
        /// Finds an array of bytes in a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="seek"></param>
        /// <param name="rewindToStart"></param>
        /// <returns></returns>
        public static long PositionOf(this Stream stream, byte[] seek, bool rewindToStart = true)
        {
            long matches = 0;
            while(stream.Position < stream.Length)
            {
                if (stream.ReadByte() == seek[matches])
                {
                    matches++;
                    if (matches > seek.LongLength)
                    {
                        if(!rewindToStart)
                        return stream.Position;

                        if (stream.CanSeek)
                        {
                            stream.Seek(-seek.LongLength, SeekOrigin.Current);
                            return stream.Position;
                        }
                        return stream.Position - seek.LongLength;
                    }
                }
                else matches = 0;
            }
            return -1;
        }
    }
}
