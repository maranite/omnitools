using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Vst.Presets.Utilities;
using Vst.Presets.Bitwig;
using Vst.Presets.VST;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Vst.Presets.Synthmaster
{

    public static class SynthMasterUtilities
    {
        public static BitwigPreset ConvertTo(this SynthMasterPreset smpr, BitwigPreset template)
        {
            var presetName = Path.GetFileNameWithoutExtension(smpr.FileName);
            var result = template.Clone();
            SmprTagInferer.CleanNameAndInfer(smpr, result);
            result.VstPreset = result.VstPreset.Clone(smpr.PresetName, smpr.Data);
            return result;
        }

        public static void ConvertAllSynthmasterToBitwig(
            this BitwigPreset bwPreset,
            DirectoryInfo source,
            DirectoryInfo target,
            bool byCategory = true)
        {
            var map = source.EnumerateAndRebase(target, "*.smpr", ".bwpreset", SearchOption.AllDirectories).ToArray();
            Parallel.ForEach(map, x =>
            {
                there_is_no: try
                {
                    var smpr = new SynthMasterPreset(x.Source);
                    var newPreset = smpr.ConvertTo(bwPreset);

                    if (!byCategory)
                        newPreset.Save(x.Target);
                    else
                    {
                        var outPath = Path.Combine(x.Target.Directory.FullName, bwPreset.Category, bwPreset.PresetName, ".bwpreset");
                        newPreset.Save(outPath);
                    }
                }
                catch (System.IO.IOException)
                {
                    Thread.Sleep(1000);
                    goto there_is_no;
                }
            });
        }
    }
}
