using System.Diagnostics;

namespace Vst.Presets.Bitwig
{
    [DebuggerDisplay("{Label} {DataType} {Value}")]
    class HeaderItem
    {
        public readonly string Label;
        public readonly ChunkType DataType;
        public object Value { get; set; }

        public HeaderItem(string label, ChunkType chunkType, object value)
        {
            Label = label;
            DataType = chunkType;
            Value = value;
        }
    }
}



