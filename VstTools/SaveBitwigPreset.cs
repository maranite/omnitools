using System;
using System.Management.Automation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using Vst.Presets.Bitwig;
using Vst.Presets.VST;
using Vst.Presets.Utilities;

namespace VstTools
{
#if DEBUG
    /// <summary>
    /// <para type="synopsis">Saves a preset in Bitwig format</para>
    /// <para type="description">Saves a BitwigPreset object to disk, optionally modifying the </para>
    /// <para type="description">metadata tags for the preset.</para>
    /// </summary>
    /// <example>
    ///   <para>To save a preset with a new Category:</para>
    ///   <code>$preset = Open-BitwigPreset -Source "C:\Users\[You]\Documents\Bitwig\Library\Patches\My Patch.bwpreset"</code>
    ///   <code>Save-BitwigPreset -Template $preset</code>
    /// </example>
    [Cmdlet(VerbsData.Save, "BitwigPreset")]
    public class SaveBitwigPreset : PSCmdlet
    {
        /// <summary>
        /// The Bitwig template to use
        /// </summary>
        [Parameter(Mandatory = true)]
        public object Template { get; set; }

        /// <summary>Path to save the preset to</summary>
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true)]
        public object Path { get; set; }

        /// <summary>Name to give to the preset</summary>
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true)]
        public string Name { get; set; }

        /// <summary>Category to give to the preset</summary>
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true)]
        public string Category { get; set; }

        /// <summary>Comment for the preset</summary>
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true)]
        public string Comment { get; set; }
        
        /// <summary>Tags for the preset</summary>
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true)]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>Does fuckall</summary>
        protected override void ProcessRecord()
        {
            if (Path == null)
                throw new ArgumentNullException($"{nameof(Path)} must be specified.");

            if (Template == null)
                throw new ArgumentNullException($"{nameof(Template)} must be a valid Bitwig template.");

            var template = BitwigPreset.Load.TryFrom(Template);

            if (template == null)
                throw new ArgumentNullException($"{nameof(Template)} is not a valid Bitwig template. Supported types include: string (full path), FileInfo, Stream or an instance of BitwigPreset");

            if (Name != null)
                template.PresetName = Name;

            if (Category != null)
                template.Category = Category;

            if (Comment != null)
                template.Comment = Comment;

            if (Tags != null)
                template.Tags = Tags;

            if (Path is string)
                template.Save((string)Path);
            else if (Path is FileInfo)
                template.Save((FileInfo)Path);
            else
                throw new ArgumentNullException($"{nameof(Path)} must be either a string (path) or FileInfo.");
        }
    }
#endif
}
