using System.Diagnostics;

namespace Vst.Presets.Bitwig
{
    [DebuggerDisplay("{MetaTag} {DataType} {Value}")]
    class MetaItem
    {
        public readonly uint MetaTag;
        public readonly ChunkType DataType;
        public object Value { get; set; }

        public MetaItem(uint metaTag, ChunkType chunkType, object value)
        {
            MetaTag = metaTag;
            DataType = chunkType;
            Value = value;
        }
    }
}



