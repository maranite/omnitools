using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BwsPresetTool
{
    static class TagInferer
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
            "Default",
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

        static List<NameRule> NameRules = new List<NameRule>()
        {
            new NameRule(@"^(?<=z)([A-Z]{2,}.+)"        ,"$1"     , null       ),
            new NameRule(@"^(?i)(PIANO)"               ,"$1"     , "Piano"    ),
            new NameRule(@"^(?i)(BS|BA(S*))\b"         ,""       , "Bass"     ),
            new NameRule(@"^(?i)(BR(A|S)?)\b"          ,""       , "Brass"    ),
            new NameRule(@"(?i)(BELL)\b"               ,""       , "Bell"    ),
            new NameRule(@"^(?i)(LEAD|LD|LED)\b"       ,""       , "Lead"     ),
            new NameRule(@"^(?i)(S?FX)\b"              ,""       , "FX"       ),
            new NameRule(@"^(?i)(PA|PD|PAD)\b"         ,""       , "Pad"      ),
            new NameRule(@"^(?i)(PLK?|PK|PLUCK)\b"     ,""       , "Pluck"    ),
            new NameRule(@"^(?i)(KY|KE|KEY)\b"         ,""       , "Keys"     ),
            new NameRule(@"^(?i)(ST|STR)\b"            ,""       , "Strings"  ),
            new NameRule(@"^(?i)(OR|ORG)\b"            ,""       , "Organ"    ),
            new NameRule(@"^(?i)(ARP?|SQ|SEQ|BPM)\b"   ,""       , "Sequence" ),
            new NameRule(@"^(?i)(GUITARS?|GT|GTR)\b"   ,""       , "Guitar"   ),
            new NameRule(@"^(?i)(SY(N(TH)?)?|SN)\b"    ,""       , "Synth"    ),
            new NameRule(@"^(?i)(AR|ARP|SQ|SEQ|BPM)\b" ,""       , "Sequence" ),

            new NameRule(@"^(?i)(soft)"                 ,"$1"       , null, "soft" ),
            new NameRule(@"^(?i)(hard)"                 ,"$1"       , null, "hard" ),
            new NameRule(@"^(?i)(warm)"                 ,"$1"       , null, "warm" ),
            new NameRule(@"^(?i)(bright)"               ,"$1"     , null, "bright" ),


            new NameRule(@"^(\s?BT\s?)"                       ,""       , null ),
            new NameRule(@"^(?<=\d)(0s)"                ,"0's "   , null ),
            new NameRule(@"(\s+(ARK|MLM)$)"             ,""       , null ),
            new NameRule(@"(^[A-Z]{2}\s|\s[A-Z]{2}$)"   ,""       , null ),
            new NameRule(@"(?i)(mw|vel|pb|i|\(i\))$"    ,""       , null ),
            new NameRule(@"(?i)(\(([+]|mw|vel|pb)+\))$" ,""       , null ),
            new NameRule(@"\(\s*\)"                     ,""       , null )
        };

        static Regex InvalidChars = new Regex("[\\~#%&*{}/:<>?|\"]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex MultipleSpaces = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex StartingHyphens = new Regex("^[ -]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex DecamelCase = new Regex("(?<=[a-z])([A-Z])", RegexOptions.Compiled);

        public static void CleanNameAndInfer(ref string presetName, ref string category, ref string[] tags)
        {
            presetName = MultipleSpaces.Replace(InvalidChars.Replace(presetName, ""), " ");
            presetName = DecamelCase.Replace(presetName, " $1");

            foreach (var rule in NameRules)
            {
                if (rule.Match.IsMatch(presetName))
                {
                    var newName = rule.Match.Replace(presetName, rule.Replace);
                    newName = StartingHyphens.Replace(newName, "");
                    if (!string.IsNullOrEmpty(newName))
                        presetName = newName;

                    if (!string.IsNullOrEmpty(rule.Category) && string.IsNullOrEmpty(category))
                        category = rule.Category;

                    if(rule.Tags != null && rule.Tags.Length > 0)
                        tags = tags.Concat(rule.Tags).Distinct().ToArray();
                }
            }

            if (string.IsNullOrEmpty(category))
                category = tags.Where(c => BitwigCategories.Any(b => b.ToLowerInvariant() == c.ToLowerInvariant())).FirstOrDefault() ?? "";

            presetName = presetName.Trim();
            //presetName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(presetName.ToLowerInvariant().Trim()); //.Replace("0S ", "0's ");
        }

        static IDictionary<string, string> Prefixes = new SortedList<string, string>()
        {    {"BRA ", "brass"}
            ,{"BRS ", "brass"}
            ,{"BA ", "bass"}
            ,{"BAS ", "bass"}
            ,{"BS ", "bass"}
            ,{"LD ", "lead"}
            ,{"LED ", "lead"}
            ,{"SFX ", "fx"}
            ,{"FX ", "fx"}
            ,{"PA ", "pad"}
            ,{"PD ", "pad"}
            ,{"PAD ", "pad"}
            ,{"KY ", "keys"}
            ,{"KEY ", "keys"}
            ,{"KB ", "keys"}
            ,{"AR ", "sequence"}
            ,{"ARP ", "sequence"}
            ,{"SQ ", "sequence"}
            ,{"SEQ ", "sequence"}
            ,{"SY ", "synth"}
            ,{"SYN ", "synth"}
            ,{"ST ", "string"}
            ,{"STR ", "string"}
            ,{"OR ", "organ"}
            ,{"DR ", "percussion"}
            ,{"DRM ", "percussion"}
            ,{"PRC ", "percussion"}
            ,{"CH ", "chord"}
            ,{"CHD ", "chord"}
            ,{"PL ", "pluck"}
            ,{"PLK ", "pluck"}
            ,{"PN ", "piano"}
        };

        public static void CleanNameAndInfer(ref string presetName, Action<string> onNewTag)
        {
            foreach (var key in Prefixes.Keys)
                if (presetName.StartsWith(key))
                {
                    onNewTag(Prefixes[key]);
                    presetName = presetName.Substring(key.Length);
                    presetName = Regex.Replace(presetName, @"^(\s?\-\s?)", "");     // remove leading hyphens
                }
        }
    }
}
