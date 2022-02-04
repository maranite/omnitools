using System;
using System.Management.Automation;
using System.IO;
using Vst.Presets.Omnisphere;
using Vst.Presets.Utilities;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Opens an Omnisphere Multi file.</para>
    /// <para type="description">Loads a multi from disk and returns an instance of OmnisphereMulti</para>
    /// </summary>
    /// <example>
    ///   <para>Open a patch file and assign it to a variable:</para>
    ///   <code>$multi = Open-OmnisphereMulti -Source "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Multis\User\Orchestra.prt_omn"</code>
    /// </example>
    [Cmdlet(VerbsCommon.Open, "OmnisphereMulti")]
    [OutputType(typeof(OmnisphereMulti))]
    public class OpenOmnisphereMulti : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The source to open. May be a path (string), FileInfo, XML content or byte[] </para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public object Source { get; set; }

        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            Directory.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);

            var multi = OmnisphereMulti.Load.TryFrom(Source);
            if (multi is null)
                throw new ArgumentNullException($"{nameof(Source)} must be of type string, FileInfo, byte[] or XML.");

            WriteObject(multi);
        }
    }
}
