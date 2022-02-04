using System.IO;
using System.Management.Automation;
using Vst.Presets.Omnisphere;

namespace VstTools
{
    /// <summary>
    ///    <para type="synopsis">Returns the STEAM directory.</para>
    ///    <para type="description">Spectrasonic's Steam engine requires all plugins (Trilian, Keyscape, Omnisphere) to place their files </para>
    ///    <para type="description">into a subfolder of C:\ProgramData\Spectrasonics\Steam. This folder can be moved, with a symbolic link</para>
    ///    <para type="description">referencing the actual location of the Steam folder. This Cmdlet locates the actual physical folder where presets are stored.</para>
    /// </summary>
    /// <example>
    ///   <para>Change current directory to the steam folder:</para>
    ///   <code>Get-SteamFolder | cd</code>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "SteamFolder")]
    [OutputType(typeof(DirectoryInfo))]
    public class GetSteamFolder : PSCmdlet
    {
        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            WriteObject(OmniUtilities.GetSteamFolder());
        }
    }
}
