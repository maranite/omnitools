using System;
using System.IO;
using Vst.Presets.Utilities;

namespace Vst.Presets.NKS
{
    public static class NksfWriter
    {
        public static void ReadNksf(this BinaryReader reader)
        {
            reader.ReadRiffChunk("RIFF", out string niks);
            if ("NIKS" != niks)
                throw new NksfException("Stream is not a valid NKSF file");

            reader.ReadRiffChunk("NISI", out byte[] nisi);
            reader.ReadRiffChunk("NICA", out byte[] nica);
            reader.ReadRiffChunk("PLID", out PLID plid);
            reader.ReadRiffChunk("PCHK", out byte[] pchk);
        }

        public static void WriteNksf(this BinaryWriter writer, PLID plid, byte[] pchk, byte[] nisi, byte[] nica)
        {
            writer.WriteRiffChunk("RIFF", "NIKS");
            writer.WriteRiffChunk("NISI", nisi);
            writer.WriteRiffChunk("NICA", nica);
            writer.WriteRiffChunk("PLID", plid);
            writer.WriteRiffChunk("PCHK", pchk);

            // constructor: (magic, nica)->
            //@plid = _serializeMagic magic
            //@nica = _chunk nica if nica

            //_serializeMagic = (magic)->
            //buffer = new Buffer 4
            //buffer.write magic, 0, 4, 'ascii'
            //_serialize "VST.magic": buffer.readUInt32BE 0

            //_serialize = (obj)->
            //ver = new Buffer 4
            //ver.writeUInt32LE $.chunkVer
            //Buffer.concat[ver, msgpack.encode obj]

            //riff = riffBuilder 'NIKS'
            //riff.pushChunk 'NISI', _chunk nisi
            //riff.pushChunk 'NICA',  if nica then(_chunk nica) else @nica
            //riff.pushChunk 'PLID', @plid
            //riff.pushChunk 'PCHK', _pchk pchk
        }
    }

  
}
