namespace USPC
{
    partial class Gate
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbTOF = new System.Windows.Forms.Label();
            this.lbDistanceValue = new System.Windows.Forms.Label();
            this.lbAmpValue = new System.Windows.Forms.Label();
            this.lbAmp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbTOF
            // 
            this.lbTOF.AutoSize = true;
            this.lbTOF.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTOF.Location = new System.Drawing.Point(91, 45);
            this.lbTOF.Name = "lbTOF";
            this.lbTOF.Size = new System.Drawing.Size(35, 20);
            this.lbTOF.TabIndex = 4;
            this.lbTOF.Text = "mm";
            // 
            // lbDistanceValue
            // 
            this.lbDistanceValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDistanceValue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbDistanceValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDistanceValue.Location = new System.Drawing.Point(3, 45);
            this.lbDistanceValue.Name = "lbDistanceValue";
            this.lbDistanceValue.Size = new System.Drawing.Size(82, 21);
            this.lbDistanceValue.TabIndex = 5;
            this.lbDistanceValue.Text = "0";
            // 
            // lbAmpValue
            // 
            this.lbAmpValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAmpValue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbAmpValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAmpValue.Location = new System.Drawing.Point(3, 9);
            this.lbAmpValue.Name = "lbAmpValue";
            this.lbAmpValue.Size = new System.Drawing.Size(52, 21);
            this.lbAmpValue.TabIndex = 6;
            this.lbAmpValue.Text = "0";
            // 
            // lbAmp
            // 
            this.lbAmp.AutoSize = true;
            this.lbAmp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAmp.Location = new System.Drawing.Point(54, 9);
            this.lbAmp.Name = "lbAmp";
            this.lbAmp.Size = new System.Drawing.Size(23, 20);
            this.lbAmp.TabIndex = 3;
            this.lbAmp.Text = "%";
            // 
            // Gate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbTOF);
            this.Controls.Add(this.lbDistanceValue);
            this.Controls.Add(this.lbAmpValue);
            this.Controls.Add(this.lbAmp);
            this.Name = "Gate";
            this.Size = new System.Drawing.Size(123, 79);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTOF;
        private System.Windows.Forms.Label lbDistanceValue;
        private System.Windows.Forms.Label lbAmpValue;
        private System.Windows.Forms.Label lbAmp;
    }
}
