using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Vst.Presets.Bitwig;

namespace Vst.Presets.Synthmaster
{
    public static class SmprTagInferer
    {
        static IDictionary<string, string> TagHints = new SortedList<string, string>()
        {
            {"brass",       "brass"},
            {"bass",        "bass"},
            {"finger",      "bass"},
            {"beat",        "sequence"},
            {" arp",        "sequence"},
            {"seq",         "sequence"},
            {"lead",        "lead"},
            {"pad",         "pad"},
            {"string",      "strings"},
            {"saw",         "synth"},
            {"synth",       "synth"},
            {"bright",      "bright"},
            {"warm",        "warm"},
            {"soft",        "soft"},
            {"glass" ,      "glassy"},
            {"fat" ,        "fat"},
            {"hard" ,       "hard"},
            {"ana" ,        "analog"},
            {"vintage" ,    "vintage"},
            {"thick"  ,     "thick"},
            {"rise" ,       "rise"},
            {"bell"  ,      "bells"},
            {"key",         "key"},
            {"snare",       "percussion"},
            {"kick",        "percussion"},
            {"drum",        "percussion"},
            {"clap",        "percussion"},
            {"additive" ,   "additive" },
            {"acid" ,         "acid" },
            {"agressive" ,   "agressive" },
            {"angry" ,     "agressive" },
            {"fx",          "fx"},
            {"alarm" ,   "fx" },
            {"alien" ,   "fx" },
            {"ambient" ,   "ambient" },
            {"angel" ,   "angelic" },
            {"bit" ,   "digital" },
            {"digit" ,   "digital" },
            {"blip" ,   "fx" },
            {"bliss" ,   "warm" },
            {"beaut" ,   "beautiful" },
            {"euph" ,   "beautiful" },
            {"dream" ,   "beautiful" },
            {"chill" ,   "slow" },
            {"slow" ,   "slow" },
            {"fast" ,   "fast" },
            {"bounce" ,   "synth" },
            {"trance" ,   "synth" },
            {"dance" ,   "synth" },
            {"house" ,   "synth" },
            {"chord" ,   "chords" },
            {"organ" ,   "organ" },
            {"whur" ,   "organ" },
            {"suitcase" ,   "organ" },
            {"rhodes" ,   "organ" },
            {"church" ,   "organ" },
            {"dirty" ,   "distorted" },
            {"dist" ,   "distorted" },
            {"drop" ,   "drop" },
            {"electro" ,   "electronic" },
            {"pian" ,   "piano" },
            {"sweep" ,   "filtered" },
            {"filt" ,   "filtered" },
            {"forman" ,   "filtered" },
            {"fizz" ,   "bright" },
            {"flange" ,   "evolving" },
            {"chorus" ,   "evolving" },
            {"motion" ,   "evolving" },
            {"phase" ,   "evolving" },
            {"morph" ,   "evolving" },
            {"move" ,   "evolving" },
            {"gradu" ,   "evolving" },
            {"flute" ,   "winds" },
            {"wind" ,   "winds" },
            {"oboe" ,   "winds" },
            {"sax" ,   "winds" },
            {"trump" ,   "winds" },
            {"fm" ,   "digital fm" },
            {"gliss" ,   "glissendo" },
            {"glide" ,   "glissendo" },
            {"glock" ,   "hammered" },
            {"guit" ,   "guitar" },
            {"nostal" ,   "warm" },
            {"heaven" ,   "warm" },
            {"reso" ,   "resonant" },
            {"helico" ,   "fx" },
            {"lush" ,   "warm" },
            {"lusc" ,   "warm" },
            {"mello" ,   "warm" },
            {"magic" ,   "magic" },
            {"mono" ,   "mono" },
        };

        public static IEnumerable<string> DeriveTagsFromPath(string filePath)
        {
            var tags = new List<string>();
            var parts = filePath.ToLowerInvariant().Split(' ', '\\', '-', '.', '/', '(', ')', '_');

            foreach (var part in parts)
                foreach (var key in TagHints.Keys.Where(k => k.Length > 2))
                    if (part.Contains(key))
                        tags.Add(TagHints[key]);

            return tags.Distinct();
        }

        private const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

        class NameRule
        {
            public readonly Regex Match;
            public readonly string Replace;
            public readonly string Category;
            public readonly string[] Tags;

            public NameRule(string match, string replace, string category, params string[] tags)
            {
                this.Match = new Regex(match, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                this.Replace = replace;
                this.Category = category;
                this.Tags = tags ?? new string[] { };
            }
        }

        static List<NameRule> NameRules = new List<NameRule>()
        {
            new NameRule(@"^(?<=z)([A-Z]{2,}.+)"            ,"$1"           , null       ),
            new NameRule(@"^(?i)(PIANO)"                    ,"$1"           , "Piano"    ),
            new NameRule(@"^(?i)(BS|BA(S*))\b"              ,""             , "Bass"     ),
            new NameRule(@"^(?i)(BR(A|S)?)\b"               ,""             , "Brass"    ),
            new NameRule(@"^(?i)(BELL)\b"                   ,""             , "Bell"    ),
            new NameRule(@"(?i)(Bell)"                      ,null           , "Bell"    ),
            new NameRule(@"^(?i)(LD|LED)\b"                 ,""             , "Lead"     ),
            new NameRule(@"(?i)(SINE|SAW|SQ|TRAN|LEAD|LD|LED)"   ,null      , "Lead"     ),
            new NameRule(@"^(?i)(S?FX)\b"                   ,""             , "FX"       ),
            new NameRule(@"^(?i)(PA|PD|PAD)\b"              ,""             , "Pad"      ),
            new NameRule(@"(?i)(PD|PADS?)"                  ,null           , "Pad"      ),
            new NameRule(@"^(?i)(PLK?|PK|PLUCK)\b"          ,""             , "Pluck"    ),
            new NameRule(@"^(?i)(CHD|CHORD)\b"              ,""             , "Chord"    ),
            new NameRule(@"^(?i)(KY|KE|KEYS?)\b"            ,""             , "Keyboards"     ),
            new NameRule(@"(?i)(KY|KEYS?)\b"                ,null           , "Keyboards"     ),
            new NameRule(@"^(?i)(ST|STR)\b"                 ,""             , "Strings"  ),
            new NameRule(@"^(?i)(OR|ORG)\b"                 ,""             , "Organ"    ),
            new NameRule(@"^(?i)(ARP?|SQ|SEQ|BPM)\b"        ,""             , "Sequence" ),
            new NameRule(@"^(?i)(GUITARS?|GT|GTR)\b"        ,""             , "Guitar"   ),
            new NameRule(@"^(?i)(SY|SYN|SYNTH|SN)\b"        ,""             , "Synth"    ),
            new NameRule(@"(?i)(SY|SYN|SYNTH|SN)\b"         ,null           , "Synth"    ),
            new NameRule(@"^(?i)(AR|ARP|SQ|SEQ|BPM)\b"      ,""             , "Sequence" ),
            new NameRule(@"(?i)(AR|ARP|SQ|SEQ|BPM)\b"       ,null           , "Sequence" ),

            new NameRule(@"^(?i)(PER|PRC)\b"                ,""             , "Percussion"    ),
            new NameRule(@"(?i)(((Hi)?Hat)|Kick|Snare|Tom)" ,null           , "Percussion"    ),
            new NameRule(@"(?i)\b(PAD|PD)"                  ,null           , "Pad"    ),

            new NameRule(@"^(?i)(soft)"                     ,null           , null, "soft" ),
            new NameRule(@"^(?i)(hard)"                     ,null           , null, "hard" ),
            new NameRule(@"^(?i)(warm)"                     ,null           , null, "warm" ),
            new NameRule(@"^(?i)(bright)"                   ,null           , null, "bright" ),

            new NameRule(@"^(\s?BT\s?)"                     ,""             , null ),
            new NameRule(@"^(?<=\d)(0s)"                    ,"0's "         , null ),
            new NameRule(@"(\s+(ARK|MLM)$)"                 ,""             , null ),
            new NameRule(@"(^[A-Z]{2}\s|\s[A-Z]{2}$)"       ,""             , null ),
            new NameRule(@"(?i)(mw|vel|pb|i|\(i\))$"        ,""             , null ),
            new NameRule(@"(?i)(\(([+]|mw|vel|pb)+\))$"     ,""             , null ),
            new NameRule(@"\(\s*\)"                         ,""             , null )
        };

        static List<string> BitwigCategories = new List<string>()
        {
            "Acoustic Drums",
            "Bass",
            "Bell",
            "Brass",
            "Chip",
            "Choir",
            "Chromatic Percussion",
            "Clap",
            //"Default",
            "Delay",
            "Destruction",
            "Drone",
            "Drums",
            "Dynamics",
            "Ensemble",
            "Ethnic",
            "EQ",
            "Filter",
            "FX",
            "Guitar",
            "Hi-Hat",
            "Keyboards",
            "Kick",
            "Lead",
            "Mastering",
            "Modulation",
            "Monosynth",
            "Multi-FX",
            "Note",
            "Orchestral",
            "Organ",
            "Pad",
            "Percussion",
            "Piano",
            "Pipe",
            "Plucks",
            "Reverb",
            "Snare",
            "Strings",
            "Synth",
            "Winds"
        };

        static Regex InvalidChars = new Regex("[\\~#%&*{}/:<>?|\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex MultipleSpaces = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex StartingHyphens = new Regex("^[ -]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex DecamelCase = new Regex("(?<=[a-z])([A-Z])", RegexOptions.Compiled);

        public static void CleanNameAndInfer(SynthMasterPreset smpr, BitwigPreset preset)
        {
            preset.Tags = Enumerable.Empty<string>();
            preset.Author = smpr.Author;
            preset.Tags = smpr.Attributes;
            preset.Comment = smpr.Description.Replace('\r', ' ').Replace('\n', ' ').Replace("  ", " ");
            preset.Category = smpr.InstrumentTypes
                .Select(_ => _
                .Trim()
                .Replace("Bells", "Bell")
                .Replace("Keys", "Keyboards")
                .Replace("Rhythm", "Percussion")
                .Replace("Pluck", "Plucks")
            )
            .Select(_ => new { Category = _, Ordinal = (_ == "Bass" || _ == "Lead" || _ == "Keyboards" || _ == "Pad") ? 1 : 0 })
            .OrderBy(_ => _.Ordinal)
            .Select(_ => _.Category)
            .FirstOrDefault(BitwigCategories.Contains);

            //preset.PresetName = Path.GetFileNameWithoutExtension(smpr.FileName);
            preset.PresetName = smpr.PresetName;
            preset.PresetName = MultipleSpaces.Replace(InvalidChars.Replace(preset.PresetName, ""), " ");
            preset.PresetName = DecamelCase.Replace(preset.PresetName, " $1").Trim();

            foreach (var rule in NameRules)
            {
                if (rule.Match.IsMatch(preset.PresetName))
                {
                    if (rule.Replace != null)
                    {
                        var newName = rule.Match.Replace(preset.PresetName, rule.Replace);
                        newName = StartingHyphens.Replace(newName, "").Trim();
                        if (!string.IsNullOrEmpty(newName))
                            preset.PresetName = newName;
                    }

                    if (!string.IsNullOrEmpty(rule.Category) && string.IsNullOrEmpty(preset.Category))
                        preset.Category = rule.Category;

                    if (rule.Tags != null)
                        preset.Tags = preset.Tags.Concat(rule.Tags)
                            .Select(_ => _.ToLowerInvariant())
                            .Distinct()
                            .Where(_ => !_.StartsWith("synthmaster")
                                     && !_.StartsWith("version")
                                     && _ != "tags"
                                     && _ != "favourite"
                                     && _ != "factory"
                                     && _ != "user")
                            .ToArray();
                }
            }

            //presetName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(presetName.ToLowerInvariant().Trim()); //.Replace("0S ", "0's ");            
            if (string.IsNullOrEmpty(preset.Category))
                preset.Category = smpr.InstrumentTypes.FirstOrDefault() ?? "Unknown";
        }
    }
}
