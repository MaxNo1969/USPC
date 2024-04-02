namespace USPC
{
    partial class FRTestTcp
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
            this.btnSend = new System.Windows.Forms.Button();
            this.edCommand = new System.Windows.Forms.TextBox();
            this.edResponce = new System.Windows.Forms.TextBox();
            this.lblResponce = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(258, 9);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // edCommand
            // 
            this.edCommand.Location = new System.Drawing.Point(12, 12);
            this.edCommand.Name = "edCommand";
            this.edCommand.Size = new System.Drawing.Size(240, 20);
            this.edCommand.TabIndex = 1;
            // 
            // edResponce
            // 
            this.edResponce.Location = new System.Drawing.Point(12, 67);
            this.edResponce.Multiline = true;
            this.edResponce.Name = "edResponce";
            this.edResponce.ReadOnly = true;
            this.edResponce.Size = new System.Drawing.Size(561, 174);
            this.edResponce.TabIndex = 2;
            // 
            // lblResponce
            // 
            this.lblResponce.AutoSize = true;
            this.lblResponce.Location = new System.Drawing.Point(9, 51);
            this.lblResponce.Name = "lblResponce";
            this.lblResponce.Size = new System.Drawing.Size(59, 13);
            this.lblResponce.TabIndex = 3;
            this.lblResponce.Text = "Responce:";
            // 
            // FRTestTcp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 266);
            this.Controls.Add(this.lblResponce);
            this.Controls.Add(this.edResponce);
            this.Controls.Add(this.edCommand);
            this.Controls.Add(this.btnSend);
            this.Name = "FRTestTcp";
            this.Text = "FRTestTcp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRTestTcp_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox edCommand;
        private System.Windows.Forms.TextBox edResponce;
        private System.Windows.Forms.Label lblResponce;
    }
}