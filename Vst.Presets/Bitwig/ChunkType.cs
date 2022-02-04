namespace Vst.Presets.Bitwig
{
    enum ChunkType : byte
    {
        Byte = 0x01,
        Int16 = 0x02,
        Int32 = 0x03,
        Byte2 = 0x05,
        Double = 0x07,
        String = 0x08,
        Int32_2 = 0x09,
        Int32_3 = 0x12,
        Bytes = 0x0d,
        Strings = 0x19,
        Flag_1 = 0x16,
        Flag_2 = 0x0A,
        Unknown5 = 0x15,      // new to spec version 1.2.89
        Unknown1 = 222,       // new to spec version 1.2.89
        Unknown2 = 73,        // new to spec version 1.2.89
        Unknown3 = 199,       // new to spec version 1.2.89
        Unknown4 = 211,       // new to spec version 1.2.89
        None = 0x1A
    };
}



