using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using System.Diagnostics;
using System.Reflection;
using BwsPresetTool.Bitwig;
using BwsPresetTool.VST;

namespace BwsPresetTool
{
    public partial class VstPluginsForm : Form
    {
        public VstPluginsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadAllPluginsInPath(@"C:\Program Files\VstPlugIns\KV331 Audio\");
        }

        private void LoadAllPluginsInPath(string path)
        {
            var info = new DirectoryInfo(path);
            foreach (var file in info.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var host = new HostCommandStub();
                    var plugin = VstPluginContext.Create(file.FullName, host);
                    if (plugin == null)
                        continue;

                    var _ = plugin.PluginCommandStub;
                    var id = plugin.PluginInfo.PluginID;
                    var magic = Encoding.ASCII.GetString(BitConverter.GetBytes(id).Reverse());

                    var fields = new[] {
                        _.GetEffectName(),
                        _.GetProductString(),
                        id.ToString("x8"),
                        magic,
                        _.GetVendorString(),
                        _.GetVendorVersion().ToString(),
                        file.FullName };

                    PluginListVw.Items.Add(new ListViewItem(fields) { Tag = file.FullName });
                    plugin.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, e.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PluginListVw_DoubleClick(object sender, EventArgs e)
        {
            if (PluginListVw.SelectedItems.Count > 0)
            {
                var pluginPath = (string)PluginListVw.SelectedItems[0].Tag;

                var host = new VstPluginHost(pluginPath);
                if (host != null)
                    host.Show();
//#if debug
//                var host = new VstPluginHost(pluginPath);
//                if (host != null)
//                    host.Show();
//#else
//                var info = new ProcessStartInfo(Application.ExecutablePath, "\"" + pluginPath + "\"") ;
//                var p = Process.Start(info);
//#endif
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (ListViewItem ctx in PluginListVw.Items)
                (ctx.Tag as IDisposable)?.Dispose();

            PluginListVw.Items.Clear();

            base.OnFormClosed(e);
        }


    }
}
