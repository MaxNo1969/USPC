namespace USPC
{
    partial class FRResult
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
            this.lay = new System.Windows.Forms.TableLayoutPanel();
            this.ResultChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ThickView = new USPC.UC4SensorView();
            this.LinearView = new USPC.UC4SensorView();
            this.CrossView = new USPC.UC4SensorView();
            this.lay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultChart)).BeginInit();
            this.SuspendLayout();
            // 
            // lay
            // 
            this.lay.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.lay.ColumnCount = 1;
            this.lay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lay.Controls.Add(this.ThickView, 0, 2);
            this.lay.Controls.Add(this.LinearView, 0, 1);
            this.lay.Controls.Add(this.CrossView, 0, 0);
            this.lay.Controls.Add(this.ResultChart, 0, 3);
            this.lay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lay.Location = new System.Drawing.Point(0, 0);
            this.lay.Name = "lay";
            this.lay.RowCount = 4;
            this.lay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.lay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.lay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.lay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.lay.Size = new System.Drawing.Size(1205, 608);
            this.lay.TabIndex = 0;
            // 
            // ResultChart
            // 
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.Name = "Default";
            this.ResultChart.ChartAreas.Add(chartArea1);
            this.ResultChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultChart.Location = new System.Drawing.Point(6, 459);
            this.ResultChart.Name = "ResultChart";
            series1.ChartArea = "Default";
            series1.Name = "Series1";
            this.ResultChart.Series.Add(series1);
            this.ResultChart.Size = new System.Drawing.Size(1193, 143);
            this.ResultChart.TabIndex = 3;
            // 
            // ThickView
            // 
            this.ThickView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThickView.Location = new System.Drawing.Point(6, 308);
            this.ThickView.Name = "ThickView";
            this.ThickView.Size = new System.Drawing.Size(1193, 142);
            this.ThickView.TabIndex = 2;
            // 
            // LinearView
            // 
            this.LinearView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LinearView.Location = new System.Drawing.Point(6, 157);
            this.LinearView.Name = "LinearView";
            this.LinearView.Size = new System.Drawing.Size(1193, 142);
            this.LinearView.TabIndex = 1;
            // 
            // CrossView
            // 
            this.CrossView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CrossView.Location = new System.Drawing.Point(6, 6);
            this.CrossView.Name = "CrossView";
            this.CrossView.Size = new System.Drawing.Size(1193, 142);
            this.CrossView.TabIndex = 0;
            // 
            // FRResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 608);
            this.Controls.Add(this.lay);
            this.Name = "FRResult";
            this.Text = "Результат контроля";
            this.lay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel lay;
        private System.Windows.Forms.DataVisualization.Charting.Chart ResultChart;
        public UC4SensorView ThickView;
        public UC4SensorView LinearView;
        public UC4SensorView CrossView;
    }
}