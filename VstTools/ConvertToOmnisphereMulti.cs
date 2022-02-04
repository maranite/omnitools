using System;
using System.IO;
using System.Management.Automation;
using Vst.Presets.Omnisphere;
using Vst.Presets.Utilities;

namespace VstTools
{
    /// <summary>
    ///    <para type="synopsis">Converts a single Omnisphere Patch into a Multi.</para>
    ///    <para type="description">Converts a single omnisphere patch into a multi, optionally </para>
    ///    <para type="description">placing the patch into a specific slot (1-8) of the multi.</para>
    ///    <para type="description">By providing specifying a multi as a starting point, </para>
    ///    <para type="description">you can set up to all patch 8 slots before saving.</para>
    /// </summary>
    /// <example>
    ///   <para>To convert a patch into an equivalent multi:</para>
    ///   <code>Convert-OmnisphereMulti -Source "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch1.prt_omn" -SaveAs "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\multis\User\my multi.mlt_omn"</code>
    /// </example>
    /// <example>
    ///   <para>To create a multi consisting of two patches occupying slots 1 and 2:</para>
    ///   <code>$patch1 = Open-OmnispherePatch "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch1.prt_omn"</code>
    ///   <code>$patch2 = Open-OmnispherePatch "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch2.prt_omn"</code>
    ///   <code>$multi = Convert-OmnisphereMulti -Source $patch1 -Slot 0</code>
    ///   <code>Convert-OmnisphereMulti -Source $patch2 -Slot 1 -Multi $multi -SaveAs "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\multis\User\my multi.mlt_omn"</code>
    /// </example>
    [Cmdlet(VerbsData.ConvertTo, "OmnisphereMulti")]
    public class ConvertToOmnisphereMulti : PSCmdlet
    {
        /// <summary>
        /// <para type="description">An OmnispherePatch object or full path to a patch (.prt_omn) file</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public object Source { get; set; }

        /// <summary>
        /// <para type="description">An existing multi to use instead of the default (empty) template</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public OmnisphereMulti Multi { get; set; }

        /// <summary>
        /// <para type="description">The patch-slot within the multi into which the patch should be loaded</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public int Slot { get; set; } 

        /// <summary>
        /// <para type="description">The path to which to save the resulting multi.</para>
        /// </summary>
        [Parameter(ValueFromPipeline = false, ValueFromPipelineByPropertyName = true)]
        public FileInfo SaveAs { get; set; }

        /// <summary>Processes the cmdlet record</summary>
        protected override void ProcessRecord()
        {
            Directory.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);

            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            if (Source is PSObject)
                Source = ((PSObject)Source).BaseObject;

            if (Slot < 0 || Slot > 7)
                throw new ArgumentException($"{nameof(Slot)} must be a number between 0 and 7.");

            var patch  = OmnispherePatch.Load.TryFrom(Source);
            if (patch is null)
                throw new ArgumentNullException($"{nameof(Source)} must be of type string, FileInfo or OmnispherePatch.");

            var multi = OmnisphereMulti.Load.TryFrom(Multi ?? OmnisphereMulti.Default);
            if (multi == null)
                throw new ArgumentException($"{nameof(Multi)} must be of type OmnisphereMulti, string, FileInfo, byte[], XmlDocument.");

            multi.SetPatch(patch, Slot);
            WriteObject(multi);

            if (SaveAs != null)
                multi.Save(SaveAs.ToString());
        }
    }
}
