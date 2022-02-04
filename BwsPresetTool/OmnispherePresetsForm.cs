using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using BwsPresetTool.Bitwig;
using BwsPresetTool.VST;

namespace BwsPresetTool
{
    public partial class OmnispherePresetsForm : Form
    {
        public OmnispherePresetsForm()
        {
            InitializeComponent();
        }

        private void OmnispherePresetsForm_Load(object sender, EventArgs e)
        {
            var context = SynchronizationContext.Current;
            Task.Factory.StartNew(() => LoadMetaData(context));
        }

        private void LoadMetaData(SynchronizationContext context)
        {
            var info = new DirectoryInfo(@"C:\nks-presets-collection\src\Omnisphere\presets");
            var allFiles = info.EnumerateFiles("*.pchk", SearchOption.AllDirectories);

            foreach (var file in allFiles)
                ReadOmniMeta(file.FullName, context);

            //Parallel.ForEach(info.EnumerateFiles("*.pchk", SearchOption.AllDirectories), c => ReadOmniMeta(c.FullName));
        }

        private void ReadOmniMeta(string PchkPath, SynchronizationContext context)
        {
            var data = File.ReadAllBytes(PchkPath);
            var xmlString = Encoding.ASCII.GetString(data, 4, data.Length - 5);
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            var tt = xml.SelectNodes("/SynthMaster/SynthSubEngine/SynthEngine/SYNTHENG/ENTRYDESCR");
            if (tt == null || tt.Count < 1)
                return;

            var xn = tt[0].Attributes;
            var name = xn["name"].InnerText;
            //var name = Path.GetFileNameWithoutExtension(PchkPath);

            var library = xn["library"].InnerText;
            var attributes = (from pair in xn["ATTRIB_VALUE_DATA"].InnerText.Split(';')
                              let keyvalues = pair.Split('=')
                              where keyvalues.Length == 2
                              select new KeyValuePair<string, string>(keyvalues[0], keyvalues[1])
                              ).ToLookup(c => c.Key, c => c.Value);

            var tags = attributes["Mood"].Concat(attributes["Genre"]).Concat(attributes["Type"])
                        .Select(c => c.Replace(" - ", " ").Replace("_", " "))
                        .Where(c => !string.Equals(c, name))
                        .Distinct()
                        .ToList();

            var author = attributes["Author"].FirstOrDefault() ?? "";
            //var tagNames = new[] { "Acoustic", "Layered", "Bowed", "Electric", "Distorted", "Muted", "Vintage" };
            //var catNames = new[] { "Drums", "Bell", "Brass", "Chip", "Choir", "Organ", "Plucks", "Wind", "Winds", "BPM", "Guitar", "Piano", "Bass", "Lead", "Synth", "Drums", "Strings" };
            var type = attributes["Type"].FirstOrDefault() ?? tags.FirstOrDefault() ?? name;

            //if (name.Contains(" - ") && catNames.Any(c => name.StartsWith(c, StringComparison.InvariantCultureIgnoreCase) || name.StartsWith("z" + c, StringComparison.InvariantCultureIgnoreCase)))
            //    name = name.Split('-')[1].Trim();

            //name = name.Replace(" - ", " ").Replace("_", " ").Replace(">", "").Replace(":", "").Replace("<", "").Replace("\\", "").Replace("?", "").Replace("*", "");


            var tags2 = tags.Select(c => c.Replace("Pianos", "Piano").Replace("Guitars", "Guitar")).Where(c => c != type).Distinct();
            var comment = (attributes["Description"].FirstOrDefault() ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\\n\\n", "\\n");


            context.Post(d => { this.lvAttributes.Items.Add((ListViewItem)d); },
                        new ListViewItem(new[]
                        {
                             xn["name"].InnerText,
                             xn["library"].InnerText,
                             string.Join(" ", attributes["Type"].ToArray()),
                             string.Join(" ", attributes["Genre"].ToArray()),
                             string.Join(" ", attributes["Mood"].ToArray()),
                             PchkPath.Replace(@"C:\nks-presets-collection\src\Omnisphere\presets",""),
                             attributes["Description"].FirstOrDefault()
                        })
                        {
                            Tag = PchkPath
                        }
                );
        }


        private static void WriteOmniMeta(string PchkPath)
        {
            var data = File.ReadAllBytes(PchkPath);
            var xmlString = Encoding.ASCII.GetString(data, 4, data.Length - 5);
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            var tt = xml.SelectNodes("/SynthMaster/SynthSubEngine/SynthEngine/SYNTHENG/ENTRYDESCR");
            if (tt == null || tt.Count < 1)
                return;

            var xn = tt[0].Attributes;
            var name = xn["name"].InnerText;
            //var name = Path.GetFileNameWithoutExtension(PchkPath);
            var library = xn["library"].InnerText;
            var attributes = (from pair in xn["ATTRIB_VALUE_DATA"].InnerText.Split(';')
                              let keyvalues = pair.Split('=')
                              where keyvalues.Length == 2
                              select new KeyValuePair<string, string>(keyvalues[0], keyvalues[1])
                              ).ToLookup(c => c.Key, c => c.Value);

            var tags = attributes["Mood"].Concat(attributes["Genre"]).Concat(attributes["Type"])
                        .Select(c => c.Replace(" - ", " ").Replace("_", " "))
                        .Where(c => !string.Equals(c, name))
                        .Distinct()
                        .ToList();

            var author = attributes["Author"].FirstOrDefault() ?? "";
            var tagNames = new[] { "Acoustic", "Layered", "Bowed", "Electric", "Distorted", "Muted", "Vintage" };
            var catNames = new[] { "Drums", "Bell", "Brass", "Chip", "Choir", "Organ", "Plucks", "Wind", "Winds", "BPM", "Guitar", "Piano", "Bass", "Lead", "Synth", "Drums", "Strings" };
            var type = attributes["Type"].FirstOrDefault() ?? tags.FirstOrDefault() ?? name;
            //var type = "Proof Test";

            if (name.Contains(" - ") && catNames.Any(c => name.StartsWith(c, StringComparison.InvariantCultureIgnoreCase) || name.StartsWith("z" + c, StringComparison.InvariantCultureIgnoreCase)))
                name = name.Split('-')[1].Trim();

            name = name.Replace(" - ", " ").Replace("_", " ").Replace(">", "").Replace(":", "").Replace("<", "").Replace("\\", "").Replace("?", "").Replace("*", "");

            foreach (var tname in tagNames)
                if (type.Contains(tname))
                {
                    tags.Add(type);
                    type = type.Replace(tname, "").Trim();
                }
            if (string.IsNullOrEmpty(type))
                type = name;

            foreach (var cname in catNames)
                if (type.Contains(cname) && type != cname)
                {
                    tags.Add(type);
                    type = cname;
                }

            var tags2 = tags.Select(c => c.Replace("Pianos", "Piano").Replace("Guitars", "Guitar")).Where(c => c != type).Distinct();
            var comment = (attributes["Description"].FirstOrDefault() ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace("\\n\\n", "\\n");
            var metaPath = Path.ChangeExtension(PchkPath, ".meta");  //string.Format(@"{0}\{1}.meta", folder, fname);

            if (File.Exists(metaPath))
                File.Delete(metaPath);
            using (var writer = new StreamWriter(File.OpenWrite(metaPath)))
            {
                writer.AutoFlush = true;
                writer.WriteLine("{");
                writer.WriteLine("  \"author\": \"{0}\",", author);
                writer.WriteLine("  \"bankchain\": [\"Omnisphere\", \"{0}\", \"\"],", type);
                writer.WriteLine("  \"comment\": \"{0}\", ", comment);
                writer.WriteLine("  \"deviceType\": \"INST\",");
                writer.WriteLine("  \"name\": \"{0}\",", name);
                writer.WriteLine("  \"types\": [");
                writer.WriteLine("    [{0}]",
                    tags2.Any()
                    ? string.Join(", ", tags2.Select(c => string.Format("\"{0}\"", c)))
                    : "null"
                    );
                writer.WriteLine("  ],");
                writer.WriteLine("  \"uuid\": \"{0:D}\",", Guid.NewGuid());
                writer.WriteLine("  \"vendor\": \"Spectrasonics\"");
                writer.WriteLine("}");
                writer.Flush();
            }

            /*
            <ENTRYDESCR  name="Flying Wings of Love"  
                  library="Omnisphere Library" 

                  ATTRIB_VALUE_DATA="Author=Ignacio Longo;
                                     Author=Eric Persing;
                                      Complexity=Complex;
                                      Dev=v2.0 b8;
                                      Genre=Ambient;
                                      Genre=Film;
                                      Genre=Experimental;
                                      Genre=World Music;
                                      Mood=Dreamlike;
                                      Mood=Floating;
                                      Mood=Light;
                                      Mood=Distant;
                                      Mood=Hopeful;
                                      Mood=Joyful;
                                      Mood=Meditative;
                                      Mood=Mystical;
                                      Mood=Peaceful;
                                      Mood=Spacey;
                                      Mood=Suspenseful;
                                      Type=BPM Arps;
                                      Type=BPM Organic;
                                      Version=Omnisphere 2 Library;
                                      Featured Order=100000001490.000000;
                                      Osc Type=Sample Only;
                                      Description=Hold some simple intervals down and listen to what happens! Your wings will take flight and you can change the notes mid-air! Use the Mod Wheel to remove the ambience.;
                                      size=81514000;" >
        */

        }

    }
}
