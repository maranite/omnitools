using System.Xml;

namespace Vst.Presets.Omnisphere
{
    public interface IOmnispherePatchOrMulti
    {
        string DefaultExtension { get; }
        XmlDocument Xml { get; }
        OmniMetaData MetaData { get; }
        string ToString();
    }
}
