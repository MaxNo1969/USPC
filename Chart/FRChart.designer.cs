namespace CHART
{
    partial class FRChart
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
            this.sb = new System.Windows.Forms.StatusStrip();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // sb
            // 
            this.sb.Location = new System.Drawing.Point(0, 458);
            this.sb.Name = "sb";
            this.sb.Size = new System.Drawing.Size(807, 22);
            this.sb.TabIndex = 1;
            this.sb.Text = "statusStrip1";
            // 
            // chart
            // 
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Name = "chart";
            this.chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            this.chart.Size = new System.Drawing.Size(807, 458);
            this.chart.TabIndex = 3;
            // 
            // FRChart
            // 
            this.ClientSize = new System.Drawing.Size(807, 480);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.sb);
            this.Name = "FRChart";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip sb;
        /// <summary>
        /// chart - для заполнения клиентской части
        /// </summary>
        public System.Windows.Forms.DataVisualization.Charting.Chart chart;
    }
}
