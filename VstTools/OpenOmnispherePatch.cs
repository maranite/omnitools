using System;
using System.IO;
using System.Management.Automation;
using Vst.Presets.Omnisphere;
using Vst.Presets.Utilities;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Opens an Omnisphere patch file.</para>
    /// <para type="description">Loads a patch from disk and returns an instance of OmnispherePatch</para>
    /// </summary>
    /// <example>
    ///   <para>Open a patch file and assign it to a variable:</para>
    ///   <code>$patch = Open-OmnispherePatch -Source "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\User\patch2.prt_omn"</code>
    /// </example>
    [Cmdlet(VerbsCommon.Open, "OmnispherePatch")]
    [OutputType(typeof(OmnispherePatch))]
    public class OpenOmnispherePatch : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The source to open. May be a path (string), FileInfo, XML content or byte[] </para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public object Source { get; set; }

        /// <summary>Processes the record</summary>
        protected override void ProcessRecord()
        {
            Directory.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);

            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            var patch = OmnispherePatch.Load.TryFrom(Source);
            if (patch is null)
                throw new ArgumentNullException($"{nameof(Source)} must be of type string, FileInfo, byte[] or XML.");

            WriteObject(patch);
        }
    }
}
