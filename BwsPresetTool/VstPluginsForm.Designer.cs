namespace BwsPresetTool
{
    partial class VstPluginsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PluginListVw = new System.Windows.Forms.ListView();
            this.NameHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProductHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VstIDHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MagicHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VendorHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VersionHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AssemblyHdr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OpenFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // PluginListVw
            // 
            this.PluginListVw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PluginListVw.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHdr,
            this.ProductHdr,
            this.VstIDHdr,
            this.MagicHdr,
            this.VendorHdr,
            this.VersionHdr,
            this.AssemblyHdr});
            this.PluginListVw.FullRowSelect = true;
            this.PluginListVw.HideSelection = false;
            this.PluginListVw.Location = new System.Drawing.Point(12, 12);
            this.PluginListVw.MultiSelect = false;
            this.PluginListVw.Name = "PluginListVw";
            this.PluginListVw.Size = new System.Drawing.Size(645, 302);
            this.PluginListVw.TabIndex = 5;
            this.PluginListVw.UseCompatibleStateImageBehavior = false;
            this.PluginListVw.View = System.Windows.Forms.View.Details;
            this.PluginListVw.DoubleClick += new System.EventHandler(this.PluginListVw_DoubleClick);
            // 
            // NameHdr
            // 
            this.NameHdr.Text = "Name";
            this.NameHdr.Width = 132;
            // 
            // ProductHdr
            // 
            this.ProductHdr.DisplayIndex = 4;
            this.ProductHdr.Text = "Product";
            this.ProductHdr.Width = 108;
            // 
            // VstIDHdr
            // 
            this.VstIDHdr.Text = "VST ID";
            // 
            // MagicHdr
            // 
            this.MagicHdr.Text = "Magic";
            // 
            // VendorHdr
            // 
            this.VendorHdr.DisplayIndex = 5;
            this.VendorHdr.Text = "Vendor";
            this.VendorHdr.Width = 117;
            // 
            // VersionHdr
            // 
            this.VersionHdr.DisplayIndex = 1;
            this.VersionHdr.Text = "Version";
            this.VersionHdr.Width = 59;
            // 
            // AssemblyHdr
            // 
            this.AssemblyHdr.Text = "Assemlby";
            this.AssemblyHdr.Width = 86;
            // 
            // OpenFileDlg
            // 
            this.OpenFileDlg.Filter = "Plugins (*.dll)|*.dll|All Files (*.*)|*.*";
            // 
            // VstPluginsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 326);
            this.Controls.Add(this.PluginListVw);
            this.Name = "VstPluginsForm";
            this.Text = "VST Plugins";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView PluginListVw;
        private System.Windows.Forms.ColumnHeader NameHdr;
        private System.Windows.Forms.ColumnHeader ProductHdr;
        private System.Windows.Forms.ColumnHeader VendorHdr;
        private System.Windows.Forms.ColumnHeader VersionHdr;
        private System.Windows.Forms.ColumnHeader AssemblyHdr;
        private System.Windows.Forms.OpenFileDialog OpenFileDlg;
        private System.Windows.Forms.ColumnHeader VstIDHdr;
        private System.Windows.Forms.ColumnHeader MagicHdr;
    }
}

