namespace BwsPresetTool
{
    partial class OmnispherePresetsForm
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
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLibrary = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTypes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMoods = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chKeywords = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGenre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvAttributes
            // 
            this.lvAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chLibrary,
            this.chTypes,
            this.chGenre,
            this.chMoods,
            this.chKeywords,
            this.chComment});
            this.lvAttributes.Location = new System.Drawing.Point(12, 12);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.Size = new System.Drawing.Size(1227, 429);
            this.lvAttributes.TabIndex = 0;
            this.lvAttributes.UseCompatibleStateImageBehavior = false;
            this.lvAttributes.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 160;
            // 
            // chLibrary
            // 
            this.chLibrary.Text = "Library";
            this.chLibrary.Width = 160;
            // 
            // chTypes
            // 
            this.chTypes.Text = "Types";
            this.chTypes.Width = 160;
            // 
            // chMoods
            // 
            this.chMoods.Text = "Moods";
            this.chMoods.Width = 160;
            // 
            // chKeywords
            // 
            this.chKeywords.Text = "Keywords";
            this.chKeywords.Width = 160;
            // 
            // chComment
            // 
            this.chComment.Text = "Comment";
            this.chComment.Width = 300;
            // 
            // chGenre
            // 
            this.chGenre.Text = "Genre";
            this.chGenre.Width = 160;
            // 
            // OmnispherePresetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 453);
            this.Controls.Add(this.lvAttributes);
            this.Name = "OmnispherePresetsForm";
            this.Text = "OmnispherePresetsForm";
            this.Load += new System.EventHandler(this.OmnispherePresetsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvAttributes;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chLibrary;
        private System.Windows.Forms.ColumnHeader chTypes;
        private System.Windows.Forms.ColumnHeader chMoods;
        private System.Windows.Forms.ColumnHeader chKeywords;
        private System.Windows.Forms.ColumnHeader chComment;
        private System.Windows.Forms.ColumnHeader chGenre;
    }
}