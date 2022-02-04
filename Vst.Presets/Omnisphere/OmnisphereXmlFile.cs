using System.Text;
using System.Xml;
using System.IO;
using Vst.Presets.Utilities;
using Vst.Presets.VST;

namespace Vst.Presets.Omnisphere
{
    public abstract class OmnisphereXmlFile : IOmnispherePatchOrMulti, ISave
    {
        protected OmnisphereXmlFile(byte[] xmlData, string metaXPath)
        {
            Xml = new XmlDocument();
            Xml.LoadXml(Encoding.ASCII.GetString(xmlData));
            var metaNode = Xml.SelectSingleNode(metaXPath);

            if (metaNode == null)
                throw new PresetException("Invalid XML passed for multi/patch");

            var entryDesc = metaNode.SelectSingleNode("ENTRYDESCR") ?? metaNode;
            if (entryDesc.Attributes.GetNamedItem("name") == null)
                throw new PresetException("Invalid omnisphere file");

            MetaData = new OmniMetaData(entryDesc);
        }

        protected OmnisphereXmlFile(XmlDocument document, string metaXPath)
        {
            this.Xml = document;

            var metaNode = document.SelectSingleNode(metaXPath);

            if (metaNode == null)
                throw new PresetException("Invalid XML passed for multi/patch");

            var entryDesc = metaNode.SelectSingleNode("ENTRYDESCR") ?? metaNode;
            if (entryDesc.Attributes.GetNamedItem("name") == null)
                throw new PresetException("Invalid omnisphere file");

            MetaData = new OmniMetaData(entryDesc);
        }

        public virtual string DefaultExtension => ".prt_omn";
        public XmlDocument Xml { get; protected set; }
        public OmniMetaData MetaData { get; protected set; }

        public void Save(Stream stream) => Xml.Save(stream);
        public override string ToString() => Xml.OuterXml;
    }
}
