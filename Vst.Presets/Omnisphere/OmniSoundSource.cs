using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using Vst.Presets.Utilities;

namespace Vst.Presets.Omnisphere
{
    public class WavFile
    {
        private OmnisphereFile sampleFile;
        public string SamplePath { get; set; }
        public TimeSpan TotalTime { get; set; }
        public IWaveProvider WavReader { get; set; }
        public long SampleCount { get; internal set; }
    }

    public class KeyRange
    {
        public string Name { get; set; }
        public byte MinPitch { get; set; }
        public byte MaxPitch { get; set; }
        public byte MinVelocity { get; set; }
        public byte MaxVelocity { get; set; }
        public byte BaseNote { get; internal set; }
        public byte RRSequenceNum { get; internal set; }
        public string SamplePath { get; internal set; }
        public float Level { get; internal set; }
    }

    public class OmniSoundSource
    {
        public string Category { get; private set; }
        public string Creator { get; private set; }
        public string Description { get; private set; }
        public string[] Keywords { get; private set; }

        public WavFile[] SampleFiles { get; internal set; }
        public KeyRange[] KeyRanges { get; internal set; }

        private static float ParseOmniFloat(string hex)
        {
            var asInt32 = Convert.ToInt32(hex, 16);
            return BitConverter.ToSingle(BitConverter.GetBytes(asInt32), 0);
        }
        
        public static OmniSoundSource Create(OmnisphereFile[] folder, OmnisphereFile sourceFile)
        {
            XmlNode xml = sourceFile.OpenXml().SelectSingleNode("//InstrumentMultisample");
            var meta = new OmniMetaData(xml.Attributes["ATTRIB_VALUE_DATA"]);

            var keywords = meta.Moods
                            .Union(meta.Types)
                            .Union(meta.Moods)
                            .Union(meta.GetAttributesFor("Timbre"))
                            .Concat(meta.GetAttributesFor("Keywords").SelectMany(_ => _.Split(',')))
                            .Distinct()
                            .ToArray();

            var creator = meta.Authors.FirstOrDefault();
            var description = meta.Comment;
            var category = Path.GetFileNameWithoutExtension(sourceFile.RelativePath);        ////TODO: Parse zmap.index for the category?   Category is always the name of the DB folder

            var multisamples = (
                        from XmlNode srcNode in xml.SelectNodes("/MultisampleZone")
                        let MinPitch = srcNode.Attributes["MinPitch"].Value
                        let MaxPitch = srcNode.Attributes["MaxPitch"].Value
                        let MinVelocity = srcNode.Attributes["MinVelocity"].Value
                        let MaxVelocity = srcNode.Attributes["MaxVelocity"].Value
                        let InstrumentName = srcNode.SelectSingleNode("SoundGroupWithNames/@SampledInstrumentName").Value
                        let defaultLayerPath = Path.Combine(InstrumentName, $"Pitch {MinPitch}-{MaxPitch}", "Default Layer.xml")
                        let defaultLayerFile = folder.FirstOrDefault(_ => _.RelativePath.EndsWith(defaultLayerPath))
                        where defaultLayerFile != null
                        let defaultLayerDoc = defaultLayerFile.OpenXml()
                        let waveformNode = defaultLayerDoc.SelectSingleNode("LayerHitStack/HitVelocity/SampleWaveform")
                        let AudioPath = waveformNode.Attributes["AudioFilePath"].Value.Replace('/', '\\').ToLowerInvariant()
                        select new KeyRange
                        {
                            MinPitch = Convert.ToByte(MinPitch),
                            MaxPitch = Convert.ToByte(MaxPitch),
                            MinVelocity = Convert.ToByte(MinVelocity),
                            MaxVelocity = Convert.ToByte(MaxVelocity),
                            Name = InstrumentName,
                            BaseNote = Convert.ToByte(waveformNode.Attributes["BaseNote"].Value),
                            Level = ParseOmniFloat(waveformNode.Attributes["Level"].Value),
                            RRSequenceNum = Convert.ToByte(waveformNode.Attributes["RoundRobinSequenceNum"]?.Value),
                            SamplePath = AudioPath
                        }
                      ).ToArray();

            if (multisamples.Length != xml.SelectNodes("/MultisampleZone").Count)
                //if (xxx.Any(_ => _.defaultLayerFile == null))
                throw new Exception("Sample File not found");   // improve this
                                                                //continue;
            var samples = from multi in multisamples
                          group multi by multi.SamplePath into distinctFiles
                          let SamplePath = distinctFiles.Key
                          let AudioFile = folder.Single(_ => _.RelativePath.ToLowerInvariant().EndsWith(SamplePath))
                          let WavReader = new WaveFileReader(AudioFile.Open())
                          select new WavFile
                          {
                              SamplePath = SamplePath,
                              TotalTime = WavReader.TotalTime,
                              SampleCount = WavReader.SampleCount,
                              WavReader = WavReader
                          };

            return new OmniSoundSource
            {
                Category = category,
                Creator = creator,
                Description = description,
                Keywords = keywords,
                SampleFiles = samples.ToArray(),
                KeyRanges = multisamples.ToArray()
            };
        }
    }
}
