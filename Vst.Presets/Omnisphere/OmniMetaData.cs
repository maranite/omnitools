using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Vst.Presets.Omnisphere
{
    public class OmniMetaData
    {
        XmlNode entryDescriptor;

        public OmniMetaData(XmlNode entryDescriptor)
        {
            this.entryDescriptor = entryDescriptor;
        }

        IEnumerable<KeyValuePair<string, string>> AllAttributes
        {
            get => from part in RawAttributes.Split(';')
                   let x = part.Split('=')
                   where x.Length > 1
                   select new KeyValuePair<string, string>(x[0], x.Length > 1 ? x[1] : null);
        }


        public IEnumerable<string> GetAttributesFor(string key)
        {
            return from part in RawAttributes.Split(';')
                   let x = part.Split('=')
                   where x.Length > 1 && x[0] == key
                   select x[1];
        }

        void SetAttributesFor(string key, IEnumerable<string> value)
        {
            var node = entryDescriptor.SelectSingleNode("ATTRIB_VALUE_DATA");
            node.Value = string.Join(";",
                             (value ?? Enumerable.Empty<string>())
                             .Where(_ => _ != null)
                             .Select(_ => string.Format("{0}={1}", key, _))
                             .Concat(from part in AllAttributes
                                     where part.Key != key
                                     select part.Value));
        }


        public string Name
        {
            get => entryDescriptor.Attributes.GetNamedItem("name")?.Value ?? "";
            set => entryDescriptor.Attributes.GetNamedItem("name").Value = value;
        }

        public string Library
        {
            get => entryDescriptor.Attributes.GetNamedItem("library")?.Value ?? "";
            set => entryDescriptor.Attributes.GetNamedItem("library").Value = value;
        }

        public string RawAttributes
        {
            get => entryDescriptor.Attributes.GetNamedItem("ATTRIB_VALUE_DATA")?.Value ?? "";
            set => entryDescriptor.Attributes.GetNamedItem("ATTRIB_VALUE_DATA").Value = value;
        }

        public string Comment
        {
            get => GetAttributesFor("Description").FirstOrDefault() ?? "";
            set => SetAttributesFor("Description", new[] { value ?? "" });
        }


        public string Keywords
        {
            get => GetAttributesFor("Keywords").FirstOrDefault() ?? "";
            set => SetAttributesFor("Keywords", new[] { value ?? "" });
        }

        public string Featured
        {
            get => GetAttributesFor("Featured").FirstOrDefault() ?? "";
            set => SetAttributesFor("Featured", new[] { value ?? "" });
        }

        public string Rating
        {
            get => GetAttributesFor("Rtng").FirstOrDefault() ?? "";
            set => SetAttributesFor("Rtng", new[] { value ?? "" });
        }

        public string Url
        {
            get => GetAttributesFor("URL").FirstOrDefault() ?? "";
            set => SetAttributesFor("URL", new[] { value ?? "" });
        }

        public string Complexity
        {
            get => GetAttributesFor("Complex").FirstOrDefault() ?? "";
            set => SetAttributesFor("Complex", new[] { value ?? "" });
        }
        public IEnumerable<string> Moods
        {
            get => GetAttributesFor("Mood") ?? Enumerable.Empty<string>();
            set => SetAttributesFor("Mood", value ?? Enumerable.Empty<string>());
        }
        public IEnumerable<string> Genres
        {
            get => GetAttributesFor("Genre") ?? Enumerable.Empty<string>();
            set => SetAttributesFor("Genre", value ?? Enumerable.Empty<string>());
        }
        public IEnumerable<string> Types
        {
            get => GetAttributesFor("Type") ?? Enumerable.Empty<string>();
            set => SetAttributesFor("Type", value ?? Enumerable.Empty<string>());
        }
        public IEnumerable<string> Authors
        {
            get => GetAttributesFor("Author") ?? Enumerable.Empty<string>();
            set => SetAttributesFor("Author", value ?? Enumerable.Empty<string>());
        }
    }
}
