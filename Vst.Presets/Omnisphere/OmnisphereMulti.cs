using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System;
using Vst.Presets.Bitwig;
using Vst.Presets.Utilities;
using Vst.Presets.VST;

namespace Vst.Presets.Omnisphere
{
    /// <summary>
    /// For multis, each preset is stored as a SynthSubEngine tag under SynthMaster.
    /// The multi attributes are stored at the root node level (SynthMaster).
    /// For each preset, the ATTRIB_VALUE_DATA library and name attributes all get moved into 
    /// SynthMaster/SynthSubEngine/SynthEngine/SYNTHENG 
    /// 
    /// SynthMaster                             1   - contains attribute data @name, @library, @ATTRIB_VALUE_DATA
    ///     SynthSubEngine                      1
    ///         SynthMasterEngineParamBlock     1     
    ///         InputMapper                     1
    ///         SynthEngine                     8
    ///             ARP                         1
    ///             SYNTHENG                    1   - contains attribute data for preset... @name, @library, @ATTRIB_VALUE_DATA
    ///                 MOD_MATRIX              n
    ///                 VOX                     2-3
    ///                 VOICE                   1
    ///                     OSC                 1
    ///                     FILTER              1
    ///                     WAVESHAPER          1
    /// </summary>
    public class OmnisphereMulti : OmnisphereXmlFile
    {
        public static readonly Loader<OmnisphereMulti> Load = (Stream stream) =>
        {
            var xml = new XmlDocument();
            xml.Load(stream);
            var root = xml.DocumentElement;
            if (root != null && root.Name == "SynthMaster")
                return new OmnisphereMulti(xml);
            return null;
        };

        public static OmnisphereMulti Default => 
            Load.From(Properties.Resources.EmptyOmnisphereMulti);

        public OmnisphereMulti() : base(Properties.Resources.EmptyOmnisphereMulti, "/SynthMaster") 
        {
            var root = Xml.DocumentElement;
            if (root == null || root.Name != "SynthMaster")
                throw new InvalidProgramException("Resource File for Omnisphere Multi is corrupt");
        }

        public OmnisphereMulti(OmnispherePatch patch)  : this()
        {
            SetPatch(patch, 0, true);
        }

        public OmnisphereMulti(XmlDocument document) : base(document, "/SynthMaster")
        {
            var node = Xml.SelectSingleNode("/SynthMaster");
            if (node == null)
                throw new System.Exception("Invalid xml for Omnisphere multi");

            var entryDesc = node.SelectSingleNode("ENTRYDESCR");
            MetaData = new OmniMetaData(entryDesc ?? node);
        }

        public override string DefaultExtension => ".mlt_omn";

        public void SetPatch(OmnispherePatch patch, int slot = 0, bool inheritAttributes = true)
        {
            if (inheritAttributes && patch.MetaData != null)
            {
                MetaData.Name = patch.MetaData.Name;
                MetaData.Library = patch.MetaData.Library;
                MetaData.RawAttributes = patch.MetaData.RawAttributes;
            }

            //var synthEngineNode = patch.Xml.SelectSingleNode("/AmberPart/SynthEngine");
            var synthEngineNode = patch.Xml.SelectSingleNode("/*[substring(name(), string-length(name()) - 3) = 'Part']/SynthEngine");
            if (synthEngineNode == null)
                throw new PresetException("Cannot find SynthEngine node in patch passed");

            synthEngineNode = Xml.ImportNode(synthEngineNode, true);

            var synthEngNode = synthEngineNode.SelectSingleNode("SYNTHENG");
            var entryDesc = synthEngNode.SelectSingleNode("ENTRYDESCR");
            if (entryDesc != null)
            {
                while (entryDesc.Attributes.Count > 0)
                {
                    XmlAttribute attribute = entryDesc.Attributes[0];
                    entryDesc.Attributes.Remove(attribute);
                    synthEngNode.Attributes.Append(attribute);
                }
                synthEngNode.RemoveChild(entryDesc);
            }

            var synthSubEngineNode = Xml.SelectSingleNode("/SynthMaster/SynthSubEngine");
            var engineNodes = synthSubEngineNode.SelectNodes("SynthEngine");

            if (engineNodes == null && engineNodes.Count <= slot)
                throw new PresetException("Cannot find nth node in SynthSubEngine");

            synthSubEngineNode.ReplaceChild(synthEngineNode, engineNodes[slot]);
        }

        public BitwigPreset ToBitWig(BitwigPreset template, string Category = "Omnisphere")
        {
            if (template == null)
                template = BitwigPresets.OmnisphereMulti;

            var preset = template.Clone();

            //preset.PresetFileName = MetaData.Name + ".fxb";
            preset.PresetName = MetaData.Name;
            preset.Author = MetaData.Authors.FirstOrDefault() ?? "";
            preset.Comment = MetaData.Comment;
            preset.Tags = MetaData.Genres.Concat(MetaData.Moods).Distinct();
            preset.VstPreset = preset.VstPreset.Clone(MetaData.Name, Encoding.ASCII.GetBytes(Xml.OuterXml));
            preset.Category = Category;
            return preset;
        }

        //public static implicit operator FxPreset (OmnisphereMulti multi)
        //{
        //    byte[] data = Encoding.UTF8.GetBytes(multi.Xml.OuterXml);
        //    return FxPreset.Clone(multi.MetaData.Name, data);
        //}

        //public FxPreset AsVstPreset
        //{
        //    get
        //    {
        //        byte[] data = Encoding.UTF8.GetBytes(Xml.OuterXml);
        //        return FxPreset.Clone(MetaData.Name, data);
        //    }
        //}
    }
}
