using System;
using System.IO;
using System.Management.Automation;
using Vst.Presets.Bitwig;
using Vst.Presets.Omnisphere;
using Vst.Presets.Utilities;

namespace VstTools
{
    /// <summary>
    ///    <para type="synopsis">Converts a single Omnisphere Patch into a Bitwig Preset.</para>
    ///    <para type="description">Converts a single omnisphere patch into a multi, optionally </para>
    ///    <para type="description">placing the patch into a specific slot (1-8) of the multi.</para>
    ///    <para type="description"></para>
    ///    <para type="description"> </para>
    ///    <para type="description"> </para>
    ///    <para type="description"> </para>
    /// </summary>
    /// <example>
    ///   <para>To convert all 3rd party patches:</para>
    ///   <code>$patches = Get-PatchesFolder</code>
    ///   <code>Convert-OmnisphereToBitwig -Source $patches -Target "C:\Users\[username]\Documents\Bitwig\Library" -NoFactory</code>
    /// </example>
    [Cmdlet(VerbsData.ConvertTo, "BitwigPreset")]
    [OutputType(typeof(BitwigPreset))]
    public class ConvertToBitwigPreset : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The Omnisphere patch or multi to be converted.</para>
        /// <para type="description">Valid values are either a path (string / FileInfo) or an instance of OmnispherePatch / OmnipshereMulti.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public object Source { get; set; }

        /// <summary>
        /// <para type="description">Optional Bitwig preset to use as a template. </para>
        /// <para type="description">Valid values are either a path (FileInfo).</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public FileInfo BitwigTemplate { get; set; }

        /// <summary>
        /// <para type="description">Optional path to specify where to save the preset to</para>
        /// </summary>
        [Parameter(ValueFromPipeline = false, ValueFromPipelineByPropertyName = true)]
        public FileInfo SaveAs { get; set; }

        /// <summary>Processes the record</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            if (Source is PSObject)
                Source = ((PSObject)Source).BaseObject;

            throw new NotImplementedException();

            //OmnisphereFileFactory.Load.TryFrom(Source, out var item);
            //if(item == null)
            //    throw new ArgumentNullException($"{nameof(Source)} must be of type string, FileInfo, OmnipshereMulti or OmnispherePatch.");

            //OmnisphereMulti multi = item as OmnisphereMulti;
            //if (multi is null)
            //{
            //    multi = OmnisphereMulti.Default;
            //    multi.SetPatch((OmnispherePatch)item, 0, true);
            //}

            //BitwigPreset.Load.TryFrom(BitwigTemplate, out var bwTemplate);
            ////if (bwTemplate == null)
            ////    BitwigPreset.Load.From(Properties.Resources.Omnisphere_bwPreset);
            ////    throw new ArgumentException($"{nameof(BitwigTemplate)} must be of type BitwigPreset, string, FileInfo, byte[], XmlDocument.");

            //var result = multi.ToBitWig(bwTemplate);

            //WriteObject(result);

            //if (SaveAs != null)
            //    result.Save(SaveAs.ToString());
        }
    }
}
