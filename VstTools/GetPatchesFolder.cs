using System;
using System.IO;
using System.Management.Automation;
using Vst.Presets.VST;
using Vst.Presets.Omnisphere;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Locates and returns the Patches subfolder for a plugin.</para>
    /// <para type="description">For a given plugin folder, this Cmdlet locates the Patches subfolder.</para>
    /// </summary>
    /// <example>
    ///   <para>List all patches folders:</para>
    ///   <code>Get-PluginFolders | Get-PatchesFolder | Write-Host </code>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PatchesFolder")]
    [OutputType(typeof(DirectoryInfo))]
    public class GetPatchesFolder : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The plugin folder in which to find the patch subfolder.
        /// If not specified, the Steam\Omnisphere folder is used.</para>
        /// </summary>
        [Parameter(Position = 0,
                    Mandatory = false,
                    ValueFromPipelineByPropertyName = true,
                    ValueFromPipeline = true)]
        public DirectoryInfo Folder { get; set; }

        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            var result = (Folder ?? OmniUtilities.GetOmnisphereFolder());
            result = OmniUtilities.GetPatchesFolder(result);
            WriteObject(result);
        }
    }
}
