namespace USPC
{
    partial class FRResultView
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartResult = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartResult)).BeginInit();
            this.SuspendLayout();
            // 
            // chartResult
            // 
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.LineColor = System.Drawing.Color.Black;
            chartArea1.CursorX.LineWidth = 3;
            chartArea1.CursorX.Position = 0D;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 95F;
            chartArea1.InnerPlotPosition.Width = 90F;
            chartArea1.InnerPlotPosition.X = 3F;
            chartArea1.Name = "Default";
            this.chartResult.ChartAreas.Add(chartArea1);
            this.chartResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartResult.Location = new System.Drawing.Point(0, 0);
            this.chartResult.Name = "chartResult";
            series1.ChartArea = "Default";
            series1.CustomProperties = "DrawingStyle=Emboss, PointWidth=1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartResult.Series.Add(series1);
            this.chartResult.Size = new System.Drawing.Size(1044, 589);
            this.chartResult.TabIndex = 4;
            this.chartResult.DoubleClick += new System.EventHandler(this.chartResult_Click);
            // 
            // FRResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 589);
            this.Controls.Add(this.chartResult);
            this.Name = "FRResultView";
            this.Text = "Результат контроля";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRResultView_FormClosing);
            this.Load += new System.EventHandler(this.FRResultView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartResult;
    }
}