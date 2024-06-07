namespace USPC
{
    partial class FRWaitLongProcess
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
            this.mes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mes
            // 
            this.mes.AutoSize = true;
            this.mes.Location = new System.Drawing.Point(12, 11);
            this.mes.Name = "mes";
            this.mes.Size = new System.Drawing.Size(0, 13);
            this.mes.TabIndex = 0;
            // 
            // FRWaitLongProcess
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(384, 34);
            this.ControlBox = false;
            this.Controls.Add(this.mes);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FRWaitLongProcess";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mes;
    }
}