using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Xml;
using Vst.Presets.Omnisphere;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Compacts a folder of files into a single .DB or .Omnisphere file</para>
    /// <para type="description">Omnisphere uses a simple TAR-like format which consists of an XML header which describes the folder structure and files, followed by chunks of binary data.</para>
    /// <para type="description">This Cmdlet creates this file from a folder or series of input files.</para>
    /// </summary>
    /// <example>
    ///   <para>To create a DB file from all files in "My Patches":</para>
    ///   <code>$patch_folder = "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\My Patches\"</code>
    ///   <code>$target = "C:\ProgramData\STEAM\Spectrasonics\Omnisphere\Settings Library\Patches\Factory\YourLibrary.db"</code>
    ///   <code>Export-OmnisphereFileSystem -Target $target -Folder $patch_folder</code>
    /// </example>
    [Cmdlet(VerbsData.Export, "OmnisphereFileSystem", DefaultParameterSetName = "Folder")]
    [OutputType(typeof(FileInfo))]
    public class ExportOmnisphereFileSystem : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The target file to be created.</para>
        /// </summary>
        [Parameter(Position = 0, Mandatory = true)]
        public FileInfo Target { get; set; }

        /// <summary>
        /// <para type="description">The source folder from which the filesystem will be created.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = true)]
        public DirectoryInfo Source { get; set; }

        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            var firstN = Source.FullName.Length;

            using (var writer = new OmnisphereArchiveWriter(Target.FullName))
            {
                foreach (var path in Directory.EnumerateFiles(Source.FullName, "*.*", SearchOption.AllDirectories))
                    writer.AddFile(path.Remove(0, firstN), File.ReadAllBytes(path));

                writer.Save();
            }
        }
    }
}
