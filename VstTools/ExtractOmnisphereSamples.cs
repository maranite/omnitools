using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Vst.Presets.Omnisphere;
using Vst.Presets.Utilities;

namespace VstTools
{
    //<para type="link" uri="https://github.com/red-gate/XmlDoc2CmdletDoc/">The XmlDoc2CmdletDoc website.</para>

    /// <summary>
    ///    <para type="synopsis">Batch converts Omnisphere patches into Bitwig presets.</para>
    ///    <para type="description">Recurses all sub-directories of a source folder for Omnipshere patches </para>
    ///    <para type="description">and converts each patch into a Bitwig Preset in a corresponding subfolder </para>
    ///    <para type="description">of the given Bitwig output folder. </para>
    ///    <para type="description">Optionally, you may specify an existing Bitwig preset (of an Omnisphere patch) </para>
    ///    <para type="description">to be used as a template for the presets that are created. </para>
    /// </summary>
    /// <example>
    ///   <para>To convert all 3rd party patches:</para>
    ///   <code>$patches = Get-PatchesFolder</code>
    ///   <code>Convert-OmnisphereToBitwig -Source $patches -Target "C:\Users\[username]\Documents\Bitwig\Library" -NoFactory</code>
    /// </example>
    /// <example>
    ///   <para>To convert the factory patches:</para>
    ///   <code>$patches = Get-PatchesFolder</code>
    ///   <code>Convert-OmnisphereToBitwig -Source $patches -Target "C:\Users\[username]\Documents\Bitwig\Library" -NoThirdParty</code>
    /// </example>
    [Cmdlet("Extract", "OmnisphereSamples")]
    public class ExtractOmnisphereSamples : PSCmdlet
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


        ///// <summary>
        ///// <para type="description">Prevents sound sources from being converted to bitwig multisamples</para>
        ///// </summary>
        //[Parameter]
        //public SwitchParameter NoSoundSources { get; set; }


        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            if (Target == null)
                throw new ArgumentNullException($"{nameof(Target)} must be specified.");

            SaveFactorySoundSources(Source, Target);

            //if (!NoSoundSources.IsPresent && !NoThirdParty.IsPresent)
            //    SaveThirdPartySoundSources(Source, Target);
        }

        private void SaveFactorySoundSources(DirectoryInfo @base, DirectoryInfo target)
        {
            foreach (var sourceFile in @base.EnumerateFiles("*.db", SearchOption.AllDirectories))
            {
                var p = Path.GetFileNameWithoutExtension(sourceFile.Name);
                if ("234567890".Contains(p.Last()))
                    continue;

                var newPath = @base.Rebase(sourceFile.Directory, target);

                using (var arch = OmnisphereArchiveFile.Open(sourceFile))
                    ConvertFactorySoundSource(arch.Files, newPath);
            }
        }

   
        private void ConvertFactorySoundSource(OmnisphereFile[] folder, DirectoryInfo targetFolder)
        {
            var xmlFiles = folder.Where(_ => Path.GetExtension(_.RelativePath) == ".xml").ToArray();

            var zmaps = (from zmap in folder.Where(_ => Path.GetExtension(_.RelativePath) == ".zmap")
                         from XmlNode srcNode in zmap.OpenXml().SelectNodes("//InstrumentMultisample/MultisampleZone")
                         select new
                         {
                             MinPitch = srcNode.Attributes["MinPitch"].Value,
                             MaxPitch = srcNode.Attributes["MaxPitch"].Value,
                             MinVelocity = srcNode.Attributes["MinVelocity"].Value,
                             MaxVelocity = srcNode.Attributes["MaxVelocity"].Value,
                             InstrumentName = srcNode.SelectSingleNode("SoundGroupWithNames/@SampledInstrumentName").Value,
                             srcNode,
                             zmap
                         }).ToArray();


            foreach (var wavSrc in folder.Where(_ => Path.GetExtension(_.RelativePath) == ".wav"))
            {
                // find all XML files that reference this sample..
                var relativePath = Path.GetFileName(wavSrc.RelativePath);

                var erwerew = (from xmlFile in xmlFiles
                        let zmap = zmaps.FirstOrDefault(z => xmlFile.RelativePath.Contains(z.InstrumentName))
                        where zmap != null
                        select new { zmap, xmlFile }).ToArray();

                var files = erwerew.ToArray();
            }

            foreach (var sourceFile in folder.Where(_ => Path.GetExtension(_.RelativePath) == ".zmap"))
            {
                var xml = sourceFile.OpenXml();

                foreach (XmlNode srcNode in xml.SelectNodes("//InstrumentMultisample/MultisampleZone"))
                {
                    var MinPitch = srcNode.Attributes["MinPitch"].Value;
                    var MaxPitch = srcNode.Attributes["MaxPitch"].Value;
                    var MinVelocity = srcNode.Attributes["MinVelocity"].Value;
                    var MaxVelocity = srcNode.Attributes["MaxVelocity"].Value;
                    var instrumentName = srcNode.SelectSingleNode("SoundGroupWithNames/@SampledInstrumentName").Value;
                    var applicableFiles = folder.Where(_ => _.RelativePath.StartsWith(instrumentName));

                    var defaultLayerFile = applicableFiles.FirstOrDefault(_ => _.RelativePath.EndsWith("Default Layer.xml")
                                                                            || _.RelativePath.EndsWith("300.xml"));
                    if (defaultLayerFile == null) continue;

                    var defaultLayerDoc = defaultLayerFile.OpenXml();
                    var hitVelocity = defaultLayerDoc.SelectSingleNode("LayerHitStack/HitVelocity");
                    var sampleWaveform = hitVelocity.SelectSingleNode("SampleWaveform");
                    var RootNote = sampleWaveform.Attributes["BaseNote"].Value;
                    var Level = sampleWaveform.Attributes["Level"].Value;
                    var RelativeLevel = ParseOmniFloat(sampleWaveform.Attributes["Level"].Value) - .5;
                    var roundRobin = sampleWaveform.Attributes["RoundRobinSequenceNum"].Value;

                    var audioFilePath = sampleWaveform.Attributes["AudioFilePath"].Value.Replace('/', '\\').ToLowerInvariant();
                    var LocalName = Path.GetFileName(audioFilePath);
                    var audioFile = folder.Single(_ => _.RelativePath.ToLowerInvariant().EndsWith(audioFilePath));
                    var WavReader = new WaveFileReader(audioFile.Open());
                    var TotalTime = WavReader.TotalTime;

                    var wavFilePath = $"{targetFolder}\\{instrumentName}\\RR{roundRobin}\\root {RootNote} pitch {MinPitch}-{MaxPitch} velocity {MinVelocity}-{MaxVelocity}.wav";
                }
            }
        }

        private static float ParseOmniFloat(string hex)
        {
            var asInt32 = Convert.ToInt32(hex, 16);
            return BitConverter.ToSingle(BitConverter.GetBytes(asInt32), 0);
        }
    }
}
