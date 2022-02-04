using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Xml;
using Vst.Presets.Bitwig;
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
    [Cmdlet(VerbsData.Convert, "OmnisphereToBitwig")]
    public class ConvertOmnisphereToBitwig : PSCmdlet
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
        public DirectoryInfo SaveTo { get; set; }

        /// <summary>
        /// <para type="description">Optionally specifies the Bitwig preset to use as a template for patch presets.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public object PresetTemplate { get; set; }

        /// <summary>
        /// <para type="description">Prevents fatory patches and multis (contained in .DB files) from being converted </para>
        /// </summary>
        [Parameter]
        public SwitchParameter NoFactory { get; set; }

        /// <summary>
        /// <para type="description">Prevents 3rd Party patches and multis from being converted </para>
        /// </summary>
        [Parameter]
        public SwitchParameter NoThirdParty { get; set; }

        /// <summary>
        /// <para type="description">Prevents patches (.prt_omn) from being converted </para>
        /// </summary>
        [Parameter]
        public SwitchParameter NoPatches { get; set; }

        /// <summary>
        /// <para type="description">Prevents multis (.mlt_omn) from being converted </para>
        /// </summary>
        [Parameter]
        public SwitchParameter NoMultis { get; set; }

        /// <summary>
        /// <para type="description">Prevents sound sources from being converted to bitwig multisamples</para>
        /// </summary>
        [Parameter]
        public SwitchParameter NoSoundSources { get; set; }

        /// <summary>Does fuckall</summary>
        protected override void ProcessRecord()
        {
            if (Source == null)
                throw new ArgumentNullException($"{nameof(Source)} must be specified.");

            if (SaveTo == null)
                throw new ArgumentNullException($"{nameof(SaveTo)} must be specified.");

            if (!NoSoundSources.IsPresent && !NoFactory.IsPresent)
                SaveFactorySoundSources(Source, SaveTo);

            //if (!NoSoundSources.IsPresent && !NoThirdParty.IsPresent)
            //    SaveThirdPartySoundSources(Source, Target);

            if (!NoMultis.IsPresent || !NoPatches.IsPresent)
            {
                var bwTemplate = BitwigPreset.Load.TryFrom(PresetTemplate ?? Properties.Resources.Omnisphere_bwPreset);
                if (bwTemplate == null)
                    throw new ArgumentException($"{nameof(PresetTemplate)} must be of type BitwigPreset, string, FileInfo or byte[].");

                if (!NoFactory.IsPresent)
                    SaveFactoryPatches(bwTemplate);

                if (!NoThirdParty.IsPresent)
                    SaveAllPatches(Source, bwTemplate);
            }
        }

        private void SaveFactoryPatches(BitwigPreset bwTemplate)
        {
            foreach (var dbFile in Source.EnumerateFiles("*.db", SearchOption.AllDirectories))
            {
                var p = Path.GetFileNameWithoutExtension(dbFile.Name);
                if ("234567890".Contains(p.Last()))
                    continue;

                using (var arch = OmnisphereArchiveFile.Open(dbFile))
                    foreach (var file in arch.Files)
                    {
                        try
                        {
                            OmnisphereMulti multi;
                            if (!NoPatches.IsPresent && Path.GetExtension(file.RelativePath).StartsWith(".prt_"))
                            {
                                var patch = OmnispherePatch.Load.From(file.Contents);
                                multi = OmnisphereMulti.Default;
                                multi.SetPatch(patch, 0, true);
                            }
                            else if (!NoMultis.IsPresent && Path.GetExtension(file.RelativePath).StartsWith(".mlt_"))
                                multi = OmnisphereMulti.Load.From(file.Contents);
                            else
                                continue;

                            var bwPreset = multi.ToBitWig(bwTemplate);
                            bwPreset.Category = file.RelativePath.Split('\\', '/').First().Trim();

                            var newPath = new FileInfo(
                                    Path.Combine(
                                        SaveTo.FullName,
                                        dbFile.Directory.Name.Trim(),
                                        Path.GetFileNameWithoutExtension(dbFile.Name).Trim(),
                                        file.RelativePath.Split('\\', '/').First().Trim(), //file.Folder.Trim(),
                                        Path.GetFileNameWithoutExtension(file.RelativePath).Trim() + ".bwpreset"
                                        ));

                            if (!newPath.Directory.Exists)
                                newPath.Directory.Create();

                            bwPreset.Save(newPath);
                        }
                        catch (Exception ex)
                        {
                            var error = new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, file);
                            WriteError(error);
                        }
                    }
            }
        }

        private void SaveAllPatches(DirectoryInfo Base, BitwigPreset bwTemplate)
        {
            foreach (var sourceFile in Base.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    OmnisphereMulti multi;
                    if (!NoPatches.IsPresent && sourceFile.Extension.StartsWith(".prt"))
                    {
                        var patch = OmnispherePatch.Load.From(sourceFile);
                        multi = OmnisphereMulti.Default;
                        multi.SetPatch(patch, 0, true);
                    }
                    else if (!NoMultis.IsPresent && sourceFile.Extension.StartsWith(".mlt"))
                    {
                        multi = OmnisphereMulti.Load.From(sourceFile);
                    }
                    else
                        continue;

                    var bwPreset = multi.ToBitWig(bwTemplate);
                    bwPreset.Category = sourceFile.Directory.Name;

                    var newPath = new FileInfo(
                        Path.ChangeExtension(
                        Path.Combine(
                                SaveTo.FullName,
                                sourceFile.FullName.Replace(Base.FullName, ""))
                                , ".bwpreset"));

                    if (!newPath.Directory.Exists)
                        newPath.Directory.Create();

                    bwPreset.Save(newPath);
                }
                catch (Exception ex)
                {
                    var error = new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, sourceFile);
                    WriteError(error);
                }
            }
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
                    ConvertFactorySoundSource2(arch.Files, newPath);
            }
        }

        private class OmniZoneInfo
        {
            public string MinPitch;
            public string MaxPitch;
            public string MinVelocity;
            public string MaxVelocity;
            public string InstrumentName;
            public XmlNode XmlNode;
            public XmlNode LayerXml;
            public string BaseNote;
            public string SamplePath;
            public OmnisphereFile SampleFile;
            internal string RoundRobin;

            public string LocalName => Path.GetFileName(SamplePath);
        }

        private void ConvertFactorySoundSource2(OmnisphereFile[] folder, DirectoryInfo targetFolder)
        {
            var zmaps = folder.Where(_ => Path.GetExtension(_.RelativePath) == ".zmap");
            //Parallel.ForEach(zmaps, sourceFile => ConvertFolder(folder, targetFolder, sourceFile));
            foreach (var sourceFile in zmaps)
                ConvertFolder(folder, targetFolder, sourceFile);
        }

        private void ConvertFolder(OmnisphereFile[] folder, DirectoryInfo targetFolder, OmnisphereFile sourceFile)
        {
            try
            {
                ConvertFactorySoundSource3(folder, targetFolder, sourceFile);
            }
            catch (Exception ex)
            {
                WriteWarning($"{sourceFile}: {ex.Message}");
            }
        }

        private void ConvertFactorySoundSource3(OmnisphereFile[] folder, DirectoryInfo targetFolder, OmnisphereFile sourceFile)
        {
            var xml = sourceFile.OpenXml();

            var attributeNode = xml.SelectSingleNode("//InstrumentMultisample/@ATTRIB_VALUE_DATA");
            if (attributeNode == null)
                return;

            var sourceMetaData = attributeNode.Value.Split(';')
                .Select(_ => _.Split('='))
                .Where(_ => _.Length == 2 && _[0] != null && _[1] != null)
                .ToLookup(_ => _[0], _ => _[1]);

            var zones = (from XmlNode srcNode in xml.SelectNodes("//InstrumentMultisample/MultisampleZone")
                         select new OmniZoneInfo
                         {
                             MinPitch = srcNode.Attributes["MinPitch"].Value,
                             MaxPitch = srcNode.Attributes["MaxPitch"].Value,
                             MinVelocity = srcNode.Attributes["MinVelocity"].Value,
                             MaxVelocity = srcNode.Attributes["MaxVelocity"].Value,
                             InstrumentName = srcNode.SelectSingleNode("SoundGroupWithNames/@SampledInstrumentName").Value,
                             XmlNode = srcNode
                         }).ToArray();

            var parentPath = Path.GetDirectoryName(sourceFile.RelativePath).Replace('\\', '/');
            if (!string.IsNullOrEmpty(parentPath))
                parentPath = parentPath + "/";

            foreach (var zone in zones)
            {
                var layerPath = $"{parentPath}{zone.InstrumentName}/Pitch {zone.MinPitch}-{zone.MaxPitch}";
                var layerFile = folder.FirstOrDefault(f => f.RelativePath.StartsWith(layerPath) && f.RelativePath.EndsWith("Layer.xml"));

                if (layerFile == null)
                {
                    if (zone.MaxPitch == "127" && zone.MinPitch == "0")
                        return;   //probably just a fucked up Spectrasonics inconsistency.

                    throw new InvalidDataException();
                }
                zone.LayerXml = layerFile.OpenXml();

                var sampleWaveFormNode = zone.LayerXml.SelectSingleNode("LayerHitStack/HitVelocity/SampleWaveform");
                zone.RoundRobin = sampleWaveFormNode.Attributes["RoundRobinSequenceNum"].Value;
                zone.BaseNote = sampleWaveFormNode.Attributes["BaseNote"].Value;
                zone.SamplePath = sampleWaveFormNode.Attributes["AudioFilePath"].Value;
                var samplePath = parentPath + zone.InstrumentName + "/" + sampleWaveFormNode.Attributes["AudioFilePath"].Value;
                zone.SampleFile = folder.FirstOrDefault(f => f.RelativePath == samplePath);
                if (zone.SampleFile == null)
                    throw new InvalidDataException();
            }


            var document = new XmlDocument();
            var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.InsertBefore(xmlDeclaration, document.DocumentElement);
            var multisample = document.AddSimpleNode("multisample", null, "name", Path.GetFileNameWithoutExtension(sourceFile.RelativePath));
            multisample.AddSimpleNode("generator", "Bitwig Studio");
           // multisample.AddSimpleNode("category", "Omnisphere"); 
            multisample.AddSimpleNode("category", targetFolder.Name);   ////TODO: Parse zmap.index for the category?   Category is always the name of the DB folder
            //multisample.AddSimpleNode("creator", sourceMetaData["Author"]?.FirstOrDefault() ?? "");
            multisample.AddSimpleNode("description", sourceMetaData["Description"]?.FirstOrDefault() ?? "");

            var keywords = multisample.AddSimpleNode("keywords");

            foreach (var keyword in sourceMetaData.AnyOf("Mood", "Timbre", "Type"))
                if (keyword.Length > 3)
                    keywords.AddSimpleNode("keyword", keyword);

            foreach (var keyword in sourceMetaData["Keywords"].SelectMany(_ => _.Split(',', '/', '\\')))
                if (keyword.Length > 3)
                    keywords.AddSimpleNode("keyword", keyword);

            var layer = multisample.AddSimpleNode("layer", null, "name", "Default");

            foreach (var info in zones)
            {
                using (var x = new WaveFileReader(new MemoryStream(info.SampleFile.Contents)))
                {
                    //var level = ParseOmniFloat(info.Level) - .5;

                    var sample = layer.AddSimpleNode("sample", null,
                         "file", info.LocalName,
                         //"gain", level.ToString("0.000", formatProvider.NumberFormat),
                         "sample-start", "0.000",  //samples
                         "sample-stop", x.SampleCount.ToString() 
                         );

                    var key = sample.AddSimpleNode("key", string.Empty,
                         "root", info.BaseNote,
                         "track", "true",
                         "tune", "0.0"
                    );

                    if (info.MaxPitch != "127")
                        key.AddAttribute("high", info.MaxPitch);

                    if (info.MinPitch != "0")
                        key.AddAttribute("low", info.MinPitch);

                    var velocity = sample.AddSimpleNode("velocity", string.Empty);

                    if (info.MaxVelocity != "127")
                        velocity.AddAttribute("high", info.MaxVelocity);

                    if (info.MinVelocity != "0")
                        velocity.AddAttribute("low", info.MinVelocity);

                    var chunks = RiffChunkData.From(info.SampleFile.Contents);
                    var smpl = chunks.FirstOrDefault(c => c.ChunkID == "smpl");
                    int loopStart = 0, loopEnd = 0;
                    var loopMode = "off";
                    if (smpl != null)
                    {
                        smpl.Get(28, out int loopCount);
                        if (loopCount > 0)
                        {
                            loopMode = "sustain";
                            smpl.Get(44, out loopStart);
                            smpl.Get(48, out loopEnd);
                        }

                        var loop = sample.AddSimpleNode("loop", string.Empty);
                        loop.AddAttribute("mode", loopMode);
                        loop.AddAttribute("start", loopStart.ToString());
                        loop.AddAttribute("stop", loopEnd.ToString());
                    }
                    layer.AppendChild(sample);
                }
            }

            if (!targetFolder.Exists)
                targetFolder.Create();

            var targetFile = Path.Combine(targetFolder.FullName, Path.GetFileNameWithoutExtension(sourceFile.RelativePath) + ".multisample");
            using (var zipToOpen = new FileStream(targetFile, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create, false))
            {
                var existingSamples = new HashSet<string>();
                foreach (var info in zones)
                    if (!existingSamples.Contains(info.LocalName))
                    {
                        existingSamples.Add(info.LocalName);
                        using (var stream = archive.CreateEntry(info.LocalName, CompressionLevel.NoCompression).Open())
                            stream.Write(info.SampleFile.Contents, 0, info.SampleFile.Contents.Length);
                    }

                var multisampleEntry = archive.CreateEntry("multisample.xml", CompressionLevel.NoCompression);
                using (var stream = multisampleEntry.Open())
                    document.Save(stream);
            }
        }


        private static float ParseOmniFloat(string hex)
        {
            var asInt32 = Convert.ToInt32(hex, 16);
            return BitConverter.ToSingle(BitConverter.GetBytes(asInt32), 0);
        }

    }
}
