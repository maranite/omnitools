using System;
using System.Management.Automation;
using System.IO;
using Vst.Presets.Bitwig;
using Vst.Presets.Utilities;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Opens a Bitwig preset as a template for creating other presets.</para>
    /// <para type="description">Loads a Bitwig present from disk and returns an instance of BitwigPreset</para>
    /// </summary>
    /// <example>
    ///   <para>Open a patch file and assign it to a variable:</para>
    ///   <code>$preset = Open-BitwigPreset -Source "C:\Users\[You]\Documents\Bitwig\Library\Patches\My Patch.bwpreset"</code>
    /// </example>
    [Cmdlet(VerbsCommon.Open, "BitwigPreset")]
    [OutputType(typeof(BitwigPreset))]
    public class OpenBitwigPreset : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The source to open. May be a path (string), FileInfo or byte[] </para>
        /// </summary>
        [Parameter(Position=0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public object Source { get; set; }

        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            var preset = BitwigPreset.Load.TryFrom(Source);
            if (preset == null)
                throw new ArgumentNullException($"{nameof(Source)} is not a valid Bitwig preset. Supported types include: string (full path), FileInfo, Stream or an instance of BitwigPreset");

            WriteObject(preset);
        }
    }
}
