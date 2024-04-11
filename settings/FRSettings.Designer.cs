namespace USPC
{
    partial class FRSettings
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
            this.pg = new System.Windows.Forms.PropertyGrid();
            this.m = new System.Windows.Forms.MenuStrip();
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m.SuspendLayout();
            this.SuspendLayout();
            // 
            // pg
            // 
            this.pg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pg.Location = new System.Drawing.Point(0, 24);
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(599, 398);
            this.pg.TabIndex = 0;
            this.pg.ToolbarVisible = false;
            // 
            // m
            // 
            this.m.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьToolStripMenuItem});
            this.m.Location = new System.Drawing.Point(0, 0);
            this.m.Name = "m";
            this.m.Size = new System.Drawing.Size(599, 24);
            this.m.TabIndex = 1;
            // 
            // сохранитьToolStripMenuItem
            // 
            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.сохранитьToolStripMenuItem.Text = "Сохранить";
            this.сохранитьToolStripMenuItem.Click += new System.EventHandler(this.miSaveSettings_Click);
            // 
            // FRSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 422);
            this.Controls.Add(this.pg);
            this.Controls.Add(this.m);
            this.MainMenuStrip = this.m;
            this.Name = "FRSettings";
            this.Text = "Настройки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRSettings_FormClosing);
            this.Load += new System.EventHandler(this.FRSettings_Load);
            this.m.ResumeLayout(false);
            this.m.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pg;
        private System.Windows.Forms.MenuStrip m;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
    }
}