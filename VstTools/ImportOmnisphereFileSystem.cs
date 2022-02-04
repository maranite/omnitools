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
using Vst.Presets.Utilities;
using Vst.Presets.Omnisphere;

namespace VstTools
{
    /// <summary>
    /// <para type="synopsis">Imports the contents of .DB or .Omnisphere file</para>
    /// <para type="description">Recreates the folder structure and files represented by the source file. 
    /// Omnisphere uses a simple TAR-like format which consists of an XML header which describes the folder 
    /// structure and files, followed by chunks of binary data representing the contents of the original files.</para>
    /// </summary>
    [Cmdlet(VerbsData.Import, "OmnisphereFileSystem")]
    [OutputType(typeof(OmnisphereArchiveFile), typeof(FileInfo))]
    public class ImportOmnisphereFileSystem : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The source file from which contents will be extracted.</para>
        /// </summary>
        [Parameter(Position = 0,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true,
                    ValueFromPipeline = false)]
        public FileInfo Source { get; set; }

        /// <summary>
        /// <para type="description">The existing folder which acts as the root folder into which files will be written.</para>
        /// </summary>
        [Parameter(Position = 1, Mandatory = false)]
        public DirectoryInfo SaveTo { get; set; }

        /// <summary>Processes the Record</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException("Input cannot be null");

            if (SaveTo != null && !SaveTo.Exists)
                    SaveTo.Create();

            if (!Path.IsPathRooted(Source.FullName))
                Source = new FileInfo(Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Source.Name));

            if (SaveTo != null)
                OmnisphereArchiveFile.ExtractTo(Source, SaveTo);
            else
            {
                using (var arch = OmnisphereArchiveFile.Open(Source))
                    foreach (var file in arch.Files)
                        WriteObject(file);
            }
        }
    }
}
