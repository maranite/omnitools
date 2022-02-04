using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System;
using Vst.Presets.Utilities;

namespace Vst.Presets.Omnisphere
{
    /// <summary>
    /// For patches, the attribute data is stored under an ENTRYDESCR tag:
    /// *Part                               1
    ///     SynthEngine                     1
    ///         ARP                         1
    ///         SYNTHENG                    1
    ///             ENTRYDESCR              1 - contains attribute data @name, @library, @ATTRIB_VALUE_DATA
    ///             MOD_MATRIX              n
    ///             VOX                     2-3
    ///             VOICE                   1
    ///                 OSC                 1
    ///                 FILTER              1
    ///                 WAVESHAPER          1
    ///
    ///
    /// </summary>
    public class OmnispherePatch : OmnisphereXmlFile
    {
        public static readonly Loader<OmnispherePatch> Load = (Stream stream) =>
        {
            var xml = new XmlDocument();
            xml.Load(stream);
            var root = xml.DocumentElement;
            if (root != null && root.Name.EndsWith("Part"))
                return new OmnispherePatch(xml);
            return null;
        };

        //private static OmnispherePatch _default;
        //public static OmnispherePatch Default
        //{
        //    get
        //    {
        //        if (_default == null)
        //            Load.From(Properties.Resources.????, out _default);
        //        return _default;
        //    }
        //}

        public OmnispherePatch(XmlDocument document) : base (document, "/*[substring(name(), string-length(name()) - 3) = 'Part']/SynthEngine/SYNTHENG")
        {
        }

        public OmnisphereMulti AsMulti => new OmnisphereMulti(this);
    }
}
