namespace BwsPresetTool
{
    partial class VstPluginHost
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRaxChunkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveRawChunkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadFXPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFXPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.loadFXBBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFXBBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.testFXPVsFXBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.muenuResaveSynthmasterPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.savePresetsUsingMidiCCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.synthMasterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBanksAssmprToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBoxBankName = new System.Windows.Forms.ToolStripTextBox();
            this.resaveAllPresetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.z3taToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllPresetsToFXBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openRawChunkDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveRawCunkDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.convertToolStripMenuItem,
            this.synthMasterToolStripMenuItem,
            this.z3taToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(659, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRaxChunkToolStripMenuItem,
            this.saveRawChunkMenuItem,
            this.toolStripSeparator1,
            this.loadFXPToolStripMenuItem,
            this.saveFXPToolStripMenuItem,
            this.toolStripSeparator3,
            this.loadFXBBankToolStripMenuItem,
            this.saveFXBBankToolStripMenuItem,
            this.toolStripSeparator2,
            this.testFXPVsFXBToolStripMenuItem,
            this.toolStripSeparator4});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // loadRaxChunkToolStripMenuItem
            // 
            this.loadRaxChunkToolStripMenuItem.Name = "loadRaxChunkToolStripMenuItem";
            this.loadRaxChunkToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadRaxChunkToolStripMenuItem.Text = "Load Rax Chunk";
            this.loadRaxChunkToolStripMenuItem.Click += new System.EventHandler(this.loadRawChunkToolStripMenuItem_Click);
            // 
            // saveRawChunkMenuItem
            // 
            this.saveRawChunkMenuItem.Name = "saveRawChunkMenuItem";
            this.saveRawChunkMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveRawChunkMenuItem.Text = "Save Raw Chunk";
            this.saveRawChunkMenuItem.Click += new System.EventHandler(this.saveRawChunbkMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // loadFXPToolStripMenuItem
            // 
            this.loadFXPToolStripMenuItem.Name = "loadFXPToolStripMenuItem";
            this.loadFXPToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadFXPToolStripMenuItem.Text = "Load FXP";
            this.loadFXPToolStripMenuItem.Click += new System.EventHandler(this.loadFXPToolStripMenuItem_Click);
            // 
            // saveFXPToolStripMenuItem
            // 
            this.saveFXPToolStripMenuItem.Name = "saveFXPToolStripMenuItem";
            this.saveFXPToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveFXPToolStripMenuItem.Text = "Save &FXP";
            this.saveFXPToolStripMenuItem.Click += new System.EventHandler(this.saveFXPToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(158, 6);
            // 
            // loadFXBBankToolStripMenuItem
            // 
            this.loadFXBBankToolStripMenuItem.Name = "loadFXBBankToolStripMenuItem";
            this.loadFXBBankToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadFXBBankToolStripMenuItem.Text = "Load FXB Bank";
            this.loadFXBBankToolStripMenuItem.Click += new System.EventHandler(this.loadFXBBankToolStripMenuItem_Click);
            // 
            // saveFXBBankToolStripMenuItem
            // 
            this.saveFXBBankToolStripMenuItem.Name = "saveFXBBankToolStripMenuItem";
            this.saveFXBBankToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveFXBBankToolStripMenuItem.Text = "Save FXB Bank";
            this.saveFXBBankToolStripMenuItem.Click += new System.EventHandler(this.saveFXBBankToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(158, 6);
            // 
            // testFXPVsFXBToolStripMenuItem
            // 
            this.testFXPVsFXBToolStripMenuItem.Name = "testFXPVsFXBToolStripMenuItem";
            this.testFXPVsFXBToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.testFXPVsFXBToolStripMenuItem.Text = "Test FXP vs FXB";
            this.testFXPVsFXBToolStripMenuItem.Click += new System.EventHandler(this.testFXPVsFXBToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(158, 6);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muenuResaveSynthmasterPlugins,
            this.savePresetsUsingMidiCCToolStripMenuItem});
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.convertToolStripMenuItem.Text = "&Convert";
            // 
            // muenuResaveSynthmasterPlugins
            // 
            this.muenuResaveSynthmasterPlugins.Name = "muenuResaveSynthmasterPlugins";
            this.muenuResaveSynthmasterPlugins.Size = new System.Drawing.Size(216, 22);
            // 
            // savePresetsUsingMidiCCToolStripMenuItem
            // 
            this.savePresetsUsingMidiCCToolStripMenuItem.Name = "savePresetsUsingMidiCCToolStripMenuItem";
            this.savePresetsUsingMidiCCToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.savePresetsUsingMidiCCToolStripMenuItem.Text = "Save Presets using Midi CC";
            this.savePresetsUsingMidiCCToolStripMenuItem.Click += new System.EventHandler(this.savePresetsUsingMidiCCToolStripMenuItem_Click);
            // 
            // synthMasterToolStripMenuItem
            // 
            this.synthMasterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resaveAllPresetsToolStripMenuItem,
            this.saveBanksAssmprToolStripMenuItem,
            this.toolStripTextBoxBankName});
            this.synthMasterToolStripMenuItem.Name = "synthMasterToolStripMenuItem";
            this.synthMasterToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.synthMasterToolStripMenuItem.Text = "SynthMaster";
            this.synthMasterToolStripMenuItem.Visible = false;
            // 
            // saveBanksAssmprToolStripMenuItem
            // 
            this.saveBanksAssmprToolStripMenuItem.Name = "saveBanksAssmprToolStripMenuItem";
            this.saveBanksAssmprToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveBanksAssmprToolStripMenuItem.Text = "Save Banks As .smpr";
            this.saveBanksAssmprToolStripMenuItem.Click += new System.EventHandler(this.saveBanksAssmprToolStripMenuItem_Click);
            // 
            // toolStripTextBoxBankName
            // 
            this.toolStripTextBoxBankName.Name = "toolStripTextBoxBankName";
            this.toolStripTextBoxBankName.Size = new System.Drawing.Size(100, 23);
            // 
            // resaveAllPresetsToolStripMenuItem
            // 
            this.resaveAllPresetsToolStripMenuItem.Name = "resaveAllPresetsToolStripMenuItem";
            this.resaveAllPresetsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.resaveAllPresetsToolStripMenuItem.Text = "Resave all presets";
            this.resaveAllPresetsToolStripMenuItem.Click += new System.EventHandler(this.resaveAllPresetsToolStripMenuItem_Click);
            // 
            // z3taToolStripMenuItem
            // 
            this.z3taToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAllPresetsToFXBToolStripMenuItem});
            this.z3taToolStripMenuItem.Name = "z3taToolStripMenuItem";
            this.z3taToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.z3taToolStripMenuItem.Text = "Z3ta+";
            this.z3taToolStripMenuItem.Visible = false;
            // 
            // saveAllPresetsToFXBToolStripMenuItem
            // 
            this.saveAllPresetsToFXBToolStripMenuItem.Name = "saveAllPresetsToFXBToolStripMenuItem";
            this.saveAllPresetsToFXBToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveAllPresetsToFXBToolStripMenuItem.Text = "Save all Presets to FXB";
            this.saveAllPresetsToFXBToolStripMenuItem.Click += new System.EventHandler(this.saveAllPresetsToFXBToolStripMenuItem_Click);
            // 
            // pluginPanel
            // 
            this.pluginPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginPanel.Location = new System.Drawing.Point(0, 24);
            this.pluginPanel.Name = "pluginPanel";
            this.pluginPanel.Size = new System.Drawing.Size(659, 489);
            this.pluginPanel.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // openRawChunkDialog
            // 
            this.openRawChunkDialog.DefaultExt = "*.raw";
            this.openRawChunkDialog.Filter = "RAW files|*.raw|All Files|*.*";
            this.openRawChunkDialog.RestoreDirectory = true;
            this.openRawChunkDialog.ShowReadOnly = true;
            this.openRawChunkDialog.SupportMultiDottedExtensions = true;
            this.openRawChunkDialog.Title = "Load a RAW chunk";
            // 
            // saveRawCunkDialog
            // 
            this.saveRawCunkDialog.DefaultExt = "*.raw";
            this.saveRawCunkDialog.Filter = "RAW files|*.raw|All Files|*.*";
            this.saveRawCunkDialog.Title = "Save a RAW chunk";
            // 
            // VstPluginHost
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(659, 513);
            this.Controls.Add(this.pluginPanel);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VstPluginHost";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "VstPluginHost";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel pluginPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem synthMasterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBanksAssmprToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadRaxChunkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveRawChunkMenuItem;
        private System.Windows.Forms.OpenFileDialog openRawChunkDialog;
        private System.Windows.Forms.SaveFileDialog saveRawCunkDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem testFXPVsFXBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem z3taToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveFXPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFXPToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem saveFXBBankToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFXBBankToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveAllPresetsToFXBToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePresetsUsingMidiCCToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxBankName;
        private System.Windows.Forms.ToolStripMenuItem muenuResaveSynthmasterPlugins;
        private System.Windows.Forms.ToolStripMenuItem resaveAllPresetsToolStripMenuItem;
    }
}