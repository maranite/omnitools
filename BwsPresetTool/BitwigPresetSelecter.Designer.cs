namespace BwsPresetTool
{
    partial class BitwigPresetSelecter
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FoldersTreeView = new System.Windows.Forms.TreeView();
            this.CategoriesListBox = new System.Windows.Forms.ListBox();
            this.TagsListBox = new System.Windows.Forms.ListBox();
            this.PresetsListView = new System.Windows.Forms.ListView();
            this.fileWatcher = new System.IO.FileSystemWatcher();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(label4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.FoldersTreeView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CategoriesListBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TagsListBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.PresetsListView, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1187, 650);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(36, 13);
            label1.TabIndex = 0;
            label1.Text = "Folder";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(299, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(49, 13);
            label2.TabIndex = 1;
            label2.Text = "Category";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(595, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(31, 13);
            label3.TabIndex = 2;
            label3.Text = "Tags";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(891, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(42, 13);
            label4.TabIndex = 3;
            label4.Text = "Presets";
            // 
            // FoldersTreeView
            // 
            this.FoldersTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FoldersTreeView.Location = new System.Drawing.Point(3, 19);
            this.FoldersTreeView.Name = "FoldersTreeView";
            this.FoldersTreeView.Size = new System.Drawing.Size(290, 628);
            this.FoldersTreeView.TabIndex = 5;
            // 
            // CategoriesListBox
            // 
            this.CategoriesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CategoriesListBox.FormattingEnabled = true;
            this.CategoriesListBox.Location = new System.Drawing.Point(299, 19);
            this.CategoriesListBox.Name = "CategoriesListBox";
            this.CategoriesListBox.Size = new System.Drawing.Size(290, 628);
            this.CategoriesListBox.TabIndex = 6;
            // 
            // TagsListBox
            // 
            this.TagsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TagsListBox.FormattingEnabled = true;
            this.TagsListBox.Location = new System.Drawing.Point(595, 19);
            this.TagsListBox.Name = "TagsListBox";
            this.TagsListBox.Size = new System.Drawing.Size(290, 628);
            this.TagsListBox.TabIndex = 7;
            // 
            // PresetsListView
            // 
            this.PresetsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PresetsListView.Location = new System.Drawing.Point(891, 19);
            this.PresetsListView.Name = "PresetsListView";
            this.PresetsListView.Size = new System.Drawing.Size(293, 628);
            this.PresetsListView.TabIndex = 9;
            this.PresetsListView.UseCompatibleStateImageBehavior = false;
            // 
            // fileWatcher
            // 
            this.fileWatcher.EnableRaisingEvents = true;
            this.fileWatcher.Filter = "*.bwpreset";
            this.fileWatcher.IncludeSubdirectories = true;
            this.fileWatcher.SynchronizingObject = this;
            // 
            // BitwigPresetSelecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 650);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BitwigPresetSelecter";
            this.Text = "Preset Selecter";
            this.Load += new System.EventHandler(this.BitwigPresetSelecter_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView FoldersTreeView;
        private System.Windows.Forms.ListBox CategoriesListBox;
        private System.Windows.Forms.ListBox TagsListBox;
        private System.Windows.Forms.ListView PresetsListView;
        private System.IO.FileSystemWatcher fileWatcher;
    }
}