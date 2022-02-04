using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Vst.Presets.Utilities
{
    public static class XmlDocumentExtensions
    {
        public static XmlNode AddSimpleNode(this XmlNode parent, string name, string text = null, params string[] atributes)
        {
            var document = (parent is XmlDocument ? (XmlDocument)parent : parent.OwnerDocument);
            var node = document.CreateElement(string.Empty, name, string.Empty);
            if (text != null)
            {
                var txt = document.CreateTextNode(text);
                txt.Value = text;
                node.AppendChild(txt);
            }
            parent.AppendChild(node);

            if (atributes != null)
                for (int i = 1; i < atributes.Length; i += 2)
                {
                    if (string.IsNullOrEmpty(atributes[i - 1])) break;
                    var attr = document.CreateAttribute(string.Empty, atributes[i - 1], string.Empty);
                    attr.Value = atributes[i];
                    node.Attributes.Append(attr);
                }

            return node;
        }

        public static XmlNode AddAttribute(this XmlNode parent, string name, string value)
        {
            var document = parent.OwnerDocument;
            var attr = document.CreateAttribute(string.Empty, name, string.Empty);
            attr.Value = value;
            parent.Attributes.Append(attr);
            return parent;
        }
    }
}
