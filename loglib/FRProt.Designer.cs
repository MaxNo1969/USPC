namespace PROTOCOL
{
    partial class FRProt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRProt));
            this.lvProt = new System.Windows.Forms.ListView();
            this.ilProt = new System.Windows.Forms.ImageList(this.components);
            this.tbProt = new System.Windows.Forms.ToolStrip();
            this.SuspendLayout();
            // 
            // lvProt
            // 
            this.lvProt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProt.FullRowSelect = true;
            this.lvProt.GridLines = true;
            this.lvProt.Location = new System.Drawing.Point(0, 28);
            this.lvProt.MultiSelect = false;
            this.lvProt.Name = "lvProt";
            this.lvProt.ShowGroups = false;
            this.lvProt.Size = new System.Drawing.Size(794, 572);
            this.lvProt.SmallImageList = this.ilProt;
            this.lvProt.TabIndex = 0;
            this.lvProt.UseCompatibleStateImageBehavior = false;
            this.lvProt.View = System.Windows.Forms.View.Details;
            // 
            // ilProt
            // 
            this.ilProt.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilProt.ImageStream")));
            this.ilProt.TransparentColor = System.Drawing.Color.Transparent;
            this.ilProt.Images.SetKeyName(0, "debug");
            this.ilProt.Images.SetKeyName(1, "info");
            this.ilProt.Images.SetKeyName(2, "warning");
            this.ilProt.Images.SetKeyName(3, "error");
            // 
            // tbProt
            // 
            this.tbProt.AutoSize = false;
            this.tbProt.Location = new System.Drawing.Point(0, 0);
            this.tbProt.Name = "tbProt";
            this.tbProt.Size = new System.Drawing.Size(794, 50);
            this.tbProt.TabIndex = 1;
            this.tbProt.Text = "toolStrip1";
            // 
            // FRProt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 600);
            this.Controls.Add(this.tbProt);
            this.Controls.Add(this.lvProt);
            this.DoubleBuffered = true;
            this.Name = "FRProt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Протокол";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRProt_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvProt;
        private System.Windows.Forms.ImageList ilProt;
        private System.Windows.Forms.ToolStrip tbProt;

    }
}