using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    internal class SubStream : Stream
    {
        private long currentPosition;
        private readonly long length;
        private readonly long offset;
        private readonly Stream stream;
        private readonly bool ownsParent;

        public SubStream(Stream parent, long offset, long length, bool ownsParent = false)
        {
            stream = parent; // Stream.Synchronized(parent);
            this.ownsParent = ownsParent;
            this.offset = offset;
            this.length = length;

            if (this.offset + this.length > stream.Length)
                throw new ArgumentException("Substream extends beyond end of parent stream");
        }

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => false;// stream.CanWrite;

        public override long Length => length;

        public override long Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                if (value <= length)
                    currentPosition = value;
                else
                    throw new ArgumentOutOfRangeException("value", "Attempt to move beyond end of stream");
            }
        }

        public override void Flush() => stream.Flush();
        public override void SetLength(long value)
        {
            throw new NotSupportedException("Attempt to change length of a substream");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current: offset += currentPosition; break;
                case SeekOrigin.End: offset += length; break;
            }

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Attempt to move before start of stream");

            currentPosition = offset;
            return currentPosition;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Attempt to read negative bytes");

            if (currentPosition > length)
                return 0;

            if (count > (length - currentPosition))
                count = (int)(length - currentPosition);

            lock (stream)
            {
                var position = stream.Position;
                stream.Position = this.offset + currentPosition;
                int numRead = stream.Read(buffer, offset, count);
                stream.Position = position;
                currentPosition += numRead;
                return numRead;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
            //if (count < 0)
            //    throw new ArgumentOutOfRangeException("count", "Attempt to write negative bytes");

            //if (currentPosition + count > length)
            //    throw new ArgumentOutOfRangeException("count", "Attempt to write beyond end of substream");

            //lock (stream)
            //{
            //    stream.Position = offsetIntoStream + currentPosition;
            //    stream.Write(buffer, offset, count);
            //}
            //currentPosition += count;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    if (ownsParent)
                        stream.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
