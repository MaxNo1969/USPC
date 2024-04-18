namespace USPC
{
    partial class FRTestAcqNet
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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.AcqChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblGate1MaxTof = new System.Windows.Forms.Label();
            this.lblGate2MaxTof = new System.Windows.Forms.Label();
            this.lblGateIFMaxTof = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.AcqChart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(0, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(81, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Стоп";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // AcqChart
            // 
            this.AcqChart.BackColor = System.Drawing.Color.Black;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.Name = "Default";
            this.AcqChart.ChartAreas.Add(chartArea1);
            this.AcqChart.Location = new System.Drawing.Point(0, 35);
            this.AcqChart.Name = "AcqChart";
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Color = System.Drawing.Color.Lime;
            series1.Name = "Gate1TOF";
            series2.ChartArea = "Default";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            series2.Name = "Gate2TOF";
            series3.ChartArea = "Default";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Color = System.Drawing.Color.Red;
            series3.Name = "GateIFTOF";
            series4.ChartArea = "Default";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Color = System.Drawing.Color.Red;
            series4.Name = "Gate1Amp";
            series5.ChartArea = "Default";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series5.Color = System.Drawing.Color.Cyan;
            series5.Name = "Gate2Amp";
            this.AcqChart.Series.Add(series1);
            this.AcqChart.Series.Add(series2);
            this.AcqChart.Series.Add(series3);
            this.AcqChart.Series.Add(series4);
            this.AcqChart.Series.Add(series5);
            this.AcqChart.Size = new System.Drawing.Size(927, 551);
            this.AcqChart.TabIndex = 3;
            title1.BackColor = System.Drawing.Color.Black;
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            title1.ForeColor = System.Drawing.Color.White;
            title1.Name = "Title1";
            title1.Text = "Acquision";
            this.AcqChart.Titles.Add(title1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Gate 1 TOF";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(549, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Gate 2 TOF";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(715, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Gate IF TOF";
            // 
            // lblGate1MaxTof
            // 
            this.lblGate1MaxTof.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGate1MaxTof.Location = new System.Drawing.Point(443, 7);
            this.lblGate1MaxTof.Name = "lblGate1MaxTof";
            this.lblGate1MaxTof.Size = new System.Drawing.Size(100, 23);
            this.lblGate1MaxTof.TabIndex = 7;
            this.lblGate1MaxTof.Text = "0";
            this.lblGate1MaxTof.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblGate2MaxTof
            // 
            this.lblGate2MaxTof.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGate2MaxTof.Location = new System.Drawing.Point(609, 7);
            this.lblGate2MaxTof.Name = "lblGate2MaxTof";
            this.lblGate2MaxTof.Size = new System.Drawing.Size(100, 23);
            this.lblGate2MaxTof.TabIndex = 10;
            this.lblGate2MaxTof.Text = "0";
            this.lblGate2MaxTof.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblGateIFMaxTof
            // 
            this.lblGateIFMaxTof.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGateIFMaxTof.Location = new System.Drawing.Point(787, 6);
            this.lblGateIFMaxTof.Name = "lblGateIFMaxTof";
            this.lblGateIFMaxTof.Size = new System.Drawing.Size(100, 23);
            this.lblGateIFMaxTof.TabIndex = 11;
            this.lblGateIFMaxTof.Text = "0";
            this.lblGateIFMaxTof.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FRTestAcqNet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 586);
            this.Controls.Add(this.lblGateIFMaxTof);
            this.Controls.Add(this.lblGate2MaxTof);
            this.Controls.Add(this.lblGate1MaxTof);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AcqChart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "FRTestAcqNet";
            this.Text = "Чтение платы";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRTestAcq_FormClosing);
            this.Resize += new System.EventHandler(this.FRTestAcq_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.AcqChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.DataVisualization.Charting.Chart AcqChart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblGate1MaxTof;
        private System.Windows.Forms.Label lblGate2MaxTof;
        private System.Windows.Forms.Label lblGateIFMaxTof;
    }
}