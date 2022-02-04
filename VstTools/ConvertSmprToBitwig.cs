using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using Vst.Presets.Bitwig;
using Vst.Presets.Synthmaster;
using Vst.Presets.Utilities;

namespace VstTools
{
    //<para type="link" uri="https://github.com/red-gate/XmlDoc2CmdletDoc/">The XmlDoc2CmdletDoc website.</para>
    /// <summary>
    ///    <para type="synopsis">Batch converts Synthmaster patches into Bitwig presets.</para>
    ///    <para type="description">Recurses all sub-directories of a source folder for SMPR patches </para>
    ///    <para type="description">and converts each patch into a Bitwig Preset in a corresponding subfolder </para>
    ///    <para type="description">of the given Bitwig output folder. </para>
    ///    <para type="description">Optionally, you may specify an existing Bitwig preset (of an Synthmaster patch) </para>
    ///    <para type="description">to be used as a template for the presets that are created. </para>
    /// </summary>
    /// <example>
    ///   <para>To convert all 3rd party patches:</para>
    ///   <code>Convert-SmprToBitwig -Source "C:\Users\[username]\Documents\Synthmaster\Patches"
    ///                              -Target "C:\Users\[username]\Documents\Bitwig\Library"</code>
    /// </example>
    [Cmdlet(VerbsData.Convert, "SmprToBitwig")]
    public class ConvertSmprToBitwig : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The source folder to scan for Omnisphere patches</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0,
                   HelpMessage = "The folder Omnipshere folder to scan for patches. ")]
        public DirectoryInfo Source { get; set; }

        /// <summary>
        /// <para type="description">The target folder into which to save Bitwig presets. Usually this would be your Bitwig library folder.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public DirectoryInfo Target { get; set; }

        /// <summary>
        /// <para type="description">Optionally specifies the Bitwig preset to use as a template for patch presets.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public object PresetTemplate { get; set; }

        /// <summary>
        /// <para type="description">Save presets under category folders instead of using the original folder structure</para>
        /// </summary>
        [Parameter]
        public SwitchParameter ByCategory { get; set; }

        /// <summary>Runs the cmdlet</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            if (Target == null)
                throw new ArgumentNullException($"{nameof(Target)} must be specified.");

            var bwPreset = BitwigPreset.Load.TryFrom(PresetTemplate ?? Properties.Resources.Synthmaster_Template);
            if (bwPreset == null)
                throw new ArgumentException($"{nameof(PresetTemplate)} must be of type BitwigPreset, string, FileInfo or byte[].");

            SaveAllPatches(Source);
        }

        private void SaveAllPatches(DirectoryInfo Base)
        {
            var byCategory = ByCategory.IsPresent;
            try
            {
                var files = Base.EnumerateFiles("*.smpr", SearchOption.AllDirectories).ToArray();
                Parallel.ForEach(files,// new ParallelOptions() { MaxDegreeOfParallelism = 1 },
                sourceFile =>
                {
                    retry:
                    try
                    {
                        var bwPreset = PresetTemplate as BitwigPreset ?? BitwigPresets.Synthmaster;
                        var smpr = new SynthMasterPreset(File.ReadAllBytes(sourceFile.FullName), sourceFile.FullName);
                        bwPreset = smpr.ConvertTo(bwPreset);
                        string outPath;
                        if (byCategory)
                        {
                            outPath = Path.Combine(Target.FullName, bwPreset.Category, bwPreset.PresetName);
                        }
                        else
                        {
                            outPath = sourceFile.FullName.Replace(Base.FullName, "");
                            outPath = outPath.Replace(Path.GetFileName(outPath), bwPreset.PresetName);
                            outPath = Path.Combine(Target.FullName, outPath);
                        }
                        outPath = Path.ChangeExtension(outPath, ".bwpreset");
                        bwPreset.Save(outPath);
                    }
                    catch (System.IO.IOException)
                    {
                        Thread.Sleep(1000);
                        goto retry;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
            catch (Exception ex)
            {
                var error = new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, "");
                WriteError(error);
            }
        }
    }
}
