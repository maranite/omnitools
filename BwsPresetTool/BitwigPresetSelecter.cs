using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using BwsPresetTool.Bitwig;

namespace BwsPresetTool
{
    public partial class BitwigPresetSelecter : Form
    {
        private void BitwigPresetSelecter_Load(object sender, EventArgs e)
        {
            PopulateControls();
        }

        private void AddSubFoldersToTreeView(TreeNode root)
        {
            Directory.EnumerateDirectories(root.FullPath, "*", SearchOption.AllDirectories);

        }

        private void PopulateControls()
        {
            var basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"Bitwig Studio\Library\Presets"
                );
            var root = FoldersTreeView.Nodes.Add(basePath, "Presets");

            AddSubFoldersToTreeView(root);

        }
    }
}
