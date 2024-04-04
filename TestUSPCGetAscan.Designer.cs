﻿namespace USPC
{
    partial class TestUSPCGetAscan
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestUSPCGetAscan));
            this.AscanChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timerAscan = new System.Windows.Forms.Timer(this.components);
            this.tb = new System.Windows.Forms.ToolStrip();
            this.tbbtnStart = new System.Windows.Forms.ToolStripButton();
            this.tbbtnStop = new System.Windows.Forms.ToolStripButton();
            this.gateIF = new USPC.Gate();
            this.gate2 = new USPC.Gate();
            this.gate1 = new USPC.Gate();
            ((System.ComponentModel.ISupportInitialize)(this.AscanChart)).BeginInit();
            this.tb.SuspendLayout();
            this.SuspendLayout();
            // 
            // AscanChart
            // 
            this.AscanChart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.Name = "Default";
            this.AscanChart.ChartAreas.Add(chartArea1);
            this.AscanChart.Location = new System.Drawing.Point(0, 28);
            this.AscanChart.Name = "AscanChart";
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series1.Name = "AscanPlot";
            series2.ChartArea = "Default";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.Red;
            series2.Name = "Gate1PosPlot";
            series3.ChartArea = "Default";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Color = System.Drawing.Color.Red;
            series3.Name = "Gate1NegPlot";
            series4.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series4.ChartArea = "Default";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Color = System.Drawing.Color.Red;
            series4.Name = "Gate1Pos2Plot";
            series5.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series5.ChartArea = "Default";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series5.Color = System.Drawing.Color.Red;
            series5.Name = "Gate1Neg2Plot";
            series6.ChartArea = "Default";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(239)))));
            series6.Name = "Gate2PosPlot";
            series7.ChartArea = "Default";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series7.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(239)))));
            series7.Name = "Gate2NegPlot";
            series8.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series8.ChartArea = "Default";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(239)))));
            series8.Name = "Gate2Pos2Plot";
            series9.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            series9.ChartArea = "Default";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series9.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(239)))));
            series9.Name = "Gate2Neg2Plot";
            series10.ChartArea = "Default";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series10.Color = System.Drawing.Color.Yellow;
            series10.Name = "GateIFPosPlot";
            series11.ChartArea = "Default";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series11.Color = System.Drawing.Color.Yellow;
            series11.Name = "GateIFNegPlot";
            this.AscanChart.Series.Add(series1);
            this.AscanChart.Series.Add(series2);
            this.AscanChart.Series.Add(series3);
            this.AscanChart.Series.Add(series4);
            this.AscanChart.Series.Add(series5);
            this.AscanChart.Series.Add(series6);
            this.AscanChart.Series.Add(series7);
            this.AscanChart.Series.Add(series8);
            this.AscanChart.Series.Add(series9);
            this.AscanChart.Series.Add(series10);
            this.AscanChart.Series.Add(series11);
            this.AscanChart.Size = new System.Drawing.Size(741, 448);
            this.AscanChart.TabIndex = 0;
            title1.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title1.ForeColor = System.Drawing.Color.White;
            title1.Name = "main";
            title1.Text = "A-Scan";
            this.AscanChart.Titles.Add(title1);
            // 
            // timerAscan
            // 
            this.timerAscan.Tick += new System.EventHandler(this.timerAscan_Tick);
            // 
            // tb
            // 
            this.tb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbtnStart,
            this.tbbtnStop});
            this.tb.Location = new System.Drawing.Point(0, 0);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(1004, 25);
            this.tb.TabIndex = 3;
            // 
            // tbbtnStart
            // 
            this.tbbtnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbtnStart.Image = ((System.Drawing.Image)(resources.GetObject("tbbtnStart.Image")));
            this.tbbtnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbtnStart.Name = "tbbtnStart";
            this.tbbtnStart.Size = new System.Drawing.Size(35, 22);
            this.tbbtnStart.Text = "Start";
            this.tbbtnStart.Click += new System.EventHandler(this.tbbtnStart_Click);
            // 
            // tbbtnStop
            // 
            this.tbbtnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbtnStop.Image = ((System.Drawing.Image)(resources.GetObject("tbbtnStop.Image")));
            this.tbbtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbtnStop.Name = "tbbtnStop";
            this.tbbtnStop.Size = new System.Drawing.Size(35, 22);
            this.tbbtnStop.Text = "Stop";
            this.tbbtnStop.Click += new System.EventHandler(this.tbbtnStop_Click);
            // 
            // gateIF
            // 
            this.gateIF.Location = new System.Drawing.Point(757, 302);
            this.gateIF.Name = "gateIF";
            this.gateIF.Size = new System.Drawing.Size(123, 79);
            this.gateIF.TabIndex = 6;
            // 
            // gate2
            // 
            this.gate2.Location = new System.Drawing.Point(757, 180);
            this.gate2.Name = "gate2";
            this.gate2.Size = new System.Drawing.Size(123, 79);
            this.gate2.TabIndex = 5;
            // 
            // gate1
            // 
            this.gate1.Location = new System.Drawing.Point(757, 60);
            this.gate1.Name = "gate1";
            this.gate1.Size = new System.Drawing.Size(123, 79);
            this.gate1.TabIndex = 4;
            // 
            // TestUSPCGetAscan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 570);
            this.Controls.Add(this.gateIF);
            this.Controls.Add(this.gate2);
            this.Controls.Add(this.gate1);
            this.Controls.Add(this.tb);
            this.Controls.Add(this.AscanChart);
            this.Name = "TestUSPCGetAscan";
            this.Text = "TestUSPCGetAscan";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestUSPCGetAscan_FormClosing);
            this.Load += new System.EventHandler(this.TestUSPCGetAscan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AscanChart)).EndInit();
            this.tb.ResumeLayout(false);
            this.tb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart AscanChart;
        private System.Windows.Forms.Timer timerAscan;
        private System.Windows.Forms.ToolStrip tb;
        private System.Windows.Forms.ToolStripButton tbbtnStart;
        private System.Windows.Forms.ToolStripButton tbbtnStop;
        private Gate gate1;
        private Gate gate2;
        private Gate gateIF;
    }
}