using System;
using System.IO;
using System.Management.Automation;
using Vst.Presets.Omnisphere;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Enumerates the Plugins found under the STEAM directory.</para>
    /// <para type="description">This helper function enumerates each plugin (Omnisphere, Keyscape, Trilian) found under the Steam folder.</para>
    /// </summary>
    /// <example>
    ///   <para>Change current directory to the steam folder:</para>
    ///   <code>Get-PluginFolders | for-each { Write-Host _ }</code>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "PluginFolders")]
    [OutputType(typeof(DirectoryInfo))]
    public class GetPluginFolders : PSCmdlet
    {
        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            var result = OmniUtilities.GetSteamFolder();
            foreach (var folder in result.EnumerateDirectories())
                WriteObject(folder);
        }
    }
}
