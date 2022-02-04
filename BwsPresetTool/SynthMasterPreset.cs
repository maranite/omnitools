using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BwsPresetTool.Bitwig;
using System.Linq;

namespace BwsPresetTool
{
    [DebuggerDisplay("{PresetName} by {Author} Bank: {BankName}")]
    public class SynthMasterPreset //: IMetaData
    {
        public byte[] Data;
        public readonly string FileName;

        private static readonly byte[] PresetNameKey = new byte[] { 0x63, 0x5F, 0x50, 0x4E, 0x41, 0x4D, 0x00 };
        public string PresetName { get => GetString(PresetNameKey); set => SetString(PresetNameKey, value); }

        private static readonly byte[] AuthorKey = new byte[] { 0x63, 0x5F, 0x50, 0x41, 0x55, 0x54, 0x00 };
        public string Author { get => GetString(AuthorKey); set => SetString(AuthorKey, value); }

        private static readonly byte[] DescriptionKey = new byte[] { 0x63, 0x5F, 0x50, 0x44, 0x45, 0x53, 0x00 };
        public string Description { get => GetString(DescriptionKey); set => SetString(DescriptionKey, value); }

        private static readonly byte[] InstrumentTypeKey = new byte[] { 0x63, 0x5F, 0x50, 0x43, 0x54, 0x31, 0x00 };
        public IEnumerable<string> InstrumentTypes
        {
            get => GetString(InstrumentTypeKey).Split('%').Where(c => c.Length > 0);
            set => SetString(InstrumentTypeKey, string.Join("%", value) + '%');
        }

        private static readonly byte[] StylesKey = new byte[] { 0x63, 0x5F, 0x50, 0x43, 0x54, 0x32, 0x00 };
        public IEnumerable<string> Styles
        {
            get => GetString(StylesKey).Split('%').Where(c => c.Length > 0);
            set => SetString(StylesKey, string.Join("%", value) + '%');
        }

        private static readonly byte[] AttributesKey = new byte[] { 0x63, 0x5F, 0x50, 0x41, 0x54, 0x54, 0x00 };
        public IEnumerable<string> Attributes
        {
            get => GetString(AttributesKey).Split('%').Where(c => c.Length > 0);
            set => SetString(AttributesKey, string.Join("%", value) + '%');
        }

        private static readonly byte[] BankNameKey = new byte[] { 0x63, 0x5F, 0x50, 0x42, 0x4E, 0x4B, 0x00 };

        public string BankName { get => GetString(BankNameKey); set => SetString(BankNameKey, value); }

        private string GetString(byte[] key)
        {
            var offset = Data.IndexOf(key);
            if (offset >= 0)
            {
                offset += key.Length;
                var length = Data[offset + 1];
                return Encoding.ASCII.GetString(Data, offset + 3, length);
            }
            return "";
        }

        private void SetString(byte[] key, string value)
        {
            if (value == null) value = string.Empty;

            var offset = Data.IndexOf(key);
            if (offset >= 0)
            {
                offset += key.Length;
                var length = Data[offset + 1];
                Data[offset + 1] = (byte)value.Length;
                Data.Replace(offset + 3, length, Encoding.ASCII.GetBytes(value));
            }
        }

        public SynthMasterPreset(byte[] data, string fileName)
        {
            this.Data = data;
            this.FileName = fileName;
        }
    }
}
