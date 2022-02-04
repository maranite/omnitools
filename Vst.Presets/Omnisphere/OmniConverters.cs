using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Presets.Bitwig;
using Vst.Presets.Utilities;

namespace Vst.Presets.Omnisphere
{
    public static class OmniConverters
    {
        //protected override void ProcessRecord()
        //{
        //    if (Source == null)
        //        throw new ArgumentNullException($"{nameof(Source)} must be specified.");

        //    if (Target == null)
        //        throw new ArgumentNullException($"{nameof(Target)} must be specified.");

        //    if (!NoSoundSources.IsPresent && !NoFactory.IsPresent)
        //        SaveFactorySoundSources(Source, Target);

        //    //if (!NoSoundSources.IsPresent && !NoThirdParty.IsPresent)
        //    //    SaveThirdPartySoundSources(Source, Target);

        //    if (!NoMultis.IsPresent || !NoPatches.IsPresent)
        //    {
        //        BitwigPreset.Load.TryFrom(PresetTemplate ?? Properties.Resources.Omnisphere_bwPreset, out var bwTemplate);
        //        if (bwTemplate == null)
        //            throw new ArgumentException($"{nameof(PresetTemplate)} must be of type BitwigPreset, string, FileInfo or byte[].");

        //        if (!NoFactory.IsPresent)
        //            SaveFactoryPatches(bwTemplate);

        //        if (!NoThirdParty.IsPresent)
        //            SaveAllPatches(Source, bwTemplate);
        //    }
        //}

        private static void SaveFactoryPatches(BitwigPreset bwTemplate, DirectoryInfo Source, DirectoryInfo Target)
        {
            foreach (var dbFile in Source.EnumerateFiles("*.db", SearchOption.AllDirectories))
            {
                var p = Path.GetFileNameWithoutExtension(dbFile.Name);
                if ("234567890".Contains(p.Last()))
                    continue;

                ConvertOmniPatchesToBitwig(dbFile, Target, bwTemplate);
            }
        }

        private static void ConvertOmniPatchesToBitwig(FileInfo archiveFile, DirectoryInfo Target, BitwigPreset bwTemplate = null)
        {
            using (var arch = OmnisphereArchiveFile.Open(archiveFile))
            {
                var newBasePath = Path.Combine(Target.FullName,
                                               archiveFile.Directory.Name.Trim(),
                                               Path.GetFileNameWithoutExtension(archiveFile.Name).Trim());
                var t2 = new DirectoryInfo(newBasePath);
                arch.ConvertPatchesToBitwig(t2, bwTemplate);
            }
        }

        public static void ConvertPatchesToBitwig(this OmnisphereArchiveFile archive, DirectoryInfo Target, BitwigPreset bwTemplate = null)
        {
            if (bwTemplate == null)
                bwTemplate = BitwigPresets.OmnisphereMulti;

            foreach (var file in archive.Files)
            {
                if (!file.IsPatch)
                    continue;

                var category = file.RelativePath.Split('\\', '/').First().Trim();

                var newPath = new FileInfo(Path.Combine(
                            Target.FullName,
                            category,
                            Path.GetFileNameWithoutExtension(file.RelativePath).Trim() + ".bwpreset"
                            ));

                newPath.EnsureDirectoryExists();
                file.AsMulti().ToBitWig(bwTemplate, category).Save(newPath);
            }
        }

        

        //private void SaveAllPatches(DirectoryInfo Base, BitwigPreset bwTemplate)
        //{
        //    foreach (var sourceFile in Base.EnumerateFiles("*.*", SearchOption.AllDirectories))
        //    {
        //        try
        //        {
        //            OmnisphereMulti multi;
        //            if (!NoPatches.IsPresent && sourceFile.Extension.StartsWith(".prt"))
        //            {
        //                OmnispherePatch.Load.From(sourceFile, out var item);
        //                multi = OmnisphereMulti.Default;
        //                multi.SetPatch(item, 0, true);
        //            }
        //            else if (!NoMultis.IsPresent && sourceFile.Extension.StartsWith(".mlt"))
        //            {
        //                OmnisphereMulti.Load.From(sourceFile, out multi);
        //            }
        //            else
        //                continue;

        //            var bwPreset = multi.ToBitWig(bwTemplate);
        //            bwPreset.Category = sourceFile.Directory.Name;

        //            var newPath = new FileInfo(
        //                Path.ChangeExtension(
        //                Path.Combine(
        //                        Target.FullName,
        //                        sourceFile.FullName.Replace(Base.FullName, ""))
        //                        , ".bwpreset"));

        //            if (!newPath.Directory.Exists)
        //                newPath.Directory.Create();

        //            bwPreset.Save(newPath);
        //        }
        //        catch (Exception ex)
        //        {
        //            var error = new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.InvalidData, sourceFile);
        //            WriteError(error);
        //        }
        //    }
        //}

        //private void SaveFactorySoundSources(DirectoryInfo @base, DirectoryInfo target)
        //{
        //    foreach (var sourceFile in @base.EnumerateFiles("*.db", SearchOption.AllDirectories))
        //    {
        //        var p = Path.GetFileNameWithoutExtension(sourceFile.Name);
        //        if ("234567890".Contains(p.Last()))
        //            continue;

        //        var newPath = @base.Rebase(sourceFile.Directory, target);

        //        using (var arch = OmnisphereArchiveFile.Open(sourceFile))
        //            ConvertFactorySoundSource2(arch.Files, newPath);
        //    }
        //}

        //private void ConvertFactorySoundSource2(OmnisphereFile[] folder, DirectoryInfo targetFolder)
        //{
        //    foreach (var sourceFile in folder.Where(_ => Path.GetExtension(_.RelativePath) == ".zmap"))
        //    {
        //        var xml = sourceFile.OpenXml();

        //        var attributeNode = xml.SelectSingleNode("//InstrumentMultisample/@ATTRIB_VALUE_DATA");
        //        if (attributeNode == null)
        //            return;

        //        var sourceMetaData = attributeNode.Value.Split(';')
        //            .Select(_ => _.Split('='))
        //            .Where(_ => _.Length == 2 && _[0] != null && _[1] != null)
        //            .ToLookup(_ => _[0], _ => _[1]);

        //        var document = new XmlDocument();
        //        var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        //        document.InsertBefore(xmlDeclaration, document.DocumentElement);

        //        var multisample = document.AddSimpleNode("multisample", null, "name", Path.GetFileNameWithoutExtension(sourceFile.RelativePath));
        //        multisample.AddSimpleNode("generator", "Bitwig Studio");
        //        multisample.AddSimpleNode("category", "Omnisphere"); // Path.GetFileNameWithoutExtension(sourceFile.Name));   ////TODO: Parse zmap.index for the category?   Category is always the name of the DB folder
        //        multisample.AddSimpleNode("creator", sourceMetaData["Author"]?.FirstOrDefault() ?? "");
        //        multisample.AddSimpleNode("description", sourceMetaData["Description"]?.FirstOrDefault() ?? "");

        //        var keywords = multisample.AddSimpleNode("keywords");

        //        foreach (var keyword in sourceMetaData.AnyOf("Mood", "Timbre", "Type"))
        //            keywords.AddSimpleNode("keyword", keyword);

        //        foreach (var keyword in sourceMetaData["Keywords"].SelectMany(_ => _.Split(',')))
        //            keywords.AddSimpleNode("keyword", keyword);

        //        var layer = multisample.AddSimpleNode("layer", null, "name", "Default");

        //        var sampleInfo = (
        //                from XmlNode srcNode in xml.SelectNodes("//InstrumentMultisample/MultisampleZone")
        //                let MinPitch = srcNode.Attributes["MinPitch"].Value
        //                let MaxPitch = srcNode.Attributes["MaxPitch"].Value
        //                let MinVelocity = srcNode.Attributes["MinVelocity"].Value
        //                let MaxVelocity = srcNode.Attributes["MaxVelocity"].Value
        //                let instrumentName = srcNode.SelectSingleNode("SoundGroupWithNames/@SampledInstrumentName").Value
        //                let defaultLayerPath = Path.Combine(instrumentName, $"Pitch {MinPitch}-{MaxPitch}", "Default Layer.xml")
        //                let defaultLayerFile = folder.FirstOrDefault(_ => _.RelativePath == defaultLayerPath)
        //                where defaultLayerFile != null
        //                let defaultLayerDoc = defaultLayerFile.OpenXml()
        //                let hitVelocity = defaultLayerDoc.SelectSingleNode("LayerHitStack/HitVelocity")
        //                let sampleWaveform = hitVelocity.SelectSingleNode("SampleWaveform")
        //                let audioFilePath = sampleWaveform.Attributes["AudioFilePath"].Value.Replace('/', '\\').ToLowerInvariant()
        //                let audioFile = folder.Single(_ => _.RelativePath.ToLowerInvariant().EndsWith(audioFilePath))
        //                let WavReader = new WaveFileReader(audioFile.Open())
        //                select new
        //                {
        //                    MinVelocity,
        //                    MaxVelocity,
        //                    MinPitch,
        //                    MaxPitch,
        //                    BaseNote = sampleWaveform.Attributes["BaseNote"].Value,
        //                    Level = sampleWaveform.Attributes["Level"].Value,
        //                    RoundRobinSequenceNum = sampleWaveform.Attributes["RoundRobinSequenceNum"].Value,
        //                    AudioFile = audioFile,
        //                    LocalName = Path.GetFileName(audioFilePath),
        //                    WavReader,
        //                    TotalTime = WavReader.TotalTime
        //                }).ToArray();

        //        var xx = from info in sampleInfo
        //                 group info by info.LocalName into y
        //                 let info = y.First()
        //                 select new
        //                 {
        //                     info.LocalName
        //                 };

        //        var formatProvider = new System.Globalization.CultureInfo("en-US");

        //        foreach (var info in sampleInfo)
        //        {
        //            var sampleEnd = 100;// (audioFileInfo.Length - 200) / 6;
        //            var level = ParseOmniFloat(info.Level) - .5;

        //            var sample = layer.AddSimpleNode("sample", null,
        //                 "file", info.LocalName,
        //                 "gain", level.ToString("0.000", formatProvider.NumberFormat),
        //                 "sample-start", "0.0",  //milliseconds
        //                 "sample-stop", info.TotalTime.ToString("##0.000", formatProvider.NumberFormat)
        //                 );

        //            var key = sample.AddSimpleNode("key", string.Empty,
        //                 "root", info.BaseNote,
        //                 "track", "true",
        //                 "tune", "0.0"
        //                );
        //            if (info.MaxPitch != "127")
        //                key.AddAttribute("high", info.MaxPitch);
        //            if (info.MinPitch != "0")
        //                key.AddAttribute("low", info.MinPitch);

        //            var velocity = sample.AddSimpleNode("velocity", string.Empty);
        //            if (info.MaxVelocity != "127")
        //                velocity.AddAttribute("high", info.MaxVelocity);
        //            if (info.MinVelocity != "0")
        //                velocity.AddAttribute("low", info.MinVelocity);

        //            var loop = sample.AddSimpleNode("loop", string.Empty
        //                , "mode", "off"
        //                , "start", "0"
        //                , "stop", "0"
        //                );

        //            layer.AppendChild(sample);
        //        }
        //        if (!targetFolder.Exists)
        //            targetFolder.Create();

        //        var targetFile = Path.Combine(targetFolder.FullName, Path.GetFileNameWithoutExtension(sourceFile.RelativePath) + ".multisample");
        //        using (var zipToOpen = new FileStream(targetFile, FileMode.Create))
        //        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create, false))
        //        {

        //            var existingSamples = new HashSet<string>();
        //            foreach (var info in sampleInfo)
        //                if (!existingSamples.Contains(info.LocalName))
        //                {
        //                    existingSamples.Add(info.LocalName);
        //                    using (var stream = archive.CreateEntry(info.LocalName, CompressionLevel.NoCompression).Open())
        //                        WriteWavContentsToStream(stream, info.AudioFile.Contents);
        //                }
        //            var multisampleEntry = archive.CreateEntry("multisample.xml", CompressionLevel.NoCompression);
        //            using (var stream = multisampleEntry.Open())
        //                document.Save(stream);
        //        }

        //        // if(!canProcessThis)

        //        return;
        //    }
        //}

        //private static void WriteWavContentsToStream(Stream stream, byte[] wavContents)
        //{
        //    using (var ms = new MemoryStream(wavContents))
        //    using (var wavReader = new WaveFileReader(ms))
        //    {
        //        TimeSpan span = wavReader.TotalTime;
        //        var provider = new OmnisphereSampleProvider(wavReader);
        //        int totalFloats = (int)wavReader.SampleCount * wavReader.WaveFormat.Channels;

        //        float[] buffer = new float[totalFloats];
        //        provider.Read(buffer, 0, (int)totalFloats);
        //        wavReader.Seek(0, SeekOrigin.Begin);
        //        float max = buffer.Select(Math.Abs).Max();
        //        var Sam = new StereoToMonoSampleProvider(provider);
        //        var wavprov = new SampleToWaveProvider16(Sam) { Volume = (1.0f / max) };

        //        using (var mm = new MemoryStream())
        //        {
        //            WaveFileWriter.WriteWavFileToStream(mm, wavprov);
        //            mm.Seek(0, SeekOrigin.Begin);
        //            mm.WriteTo(stream);
        //        }
        //    }
        //}

        private static float ParseOmniFloat(string hex)
        {
            var asInt32 = Convert.ToInt32(hex, 16);
            return BitConverter.ToSingle(BitConverter.GetBytes(asInt32), 0);
        }
    }
}
