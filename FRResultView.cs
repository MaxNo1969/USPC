using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CHART;
using FPS;
using Data;

namespace USPC
{
    public partial class FRResultView : Form
    {
        public FRResultView(Form _form)
        {
            InitializeComponent();
            Owner = _form;
            MdiParent = _form;
            setupResultChart();
            UpdateChart();
        }

        private void UpdateChart()
        {
            //chartResult.putDataOnChart(_result.values.);
            //chartResult.putColorDecision(Program.data);
        }

        /// <summary>
        /// Настройка основного чарта
        /// </summary>
        private void setupResultChart()
        {
            
            chartResult.ChartAreas[0].InnerPlotPosition.Auto = false;
            chartResult.ChartAreas[0].InnerPlotPosition.X = 3;
            chartResult.ChartAreas[0].InnerPlotPosition.Y = 0;
            chartResult.ChartAreas[0].InnerPlotPosition.Width = 96;
            chartResult.ChartAreas[0].InnerPlotPosition.Height = 96;

            chartResult.ChartAreas[0].AxisX.Minimum = 0;
            chartResult.ChartAreas[0].AxisX.Maximum = USPCData.countZones;
            chartResult.ChartAreas[0].AxisX.Interval = 10;
            //chartResult.ChartAreas[0].AxisY.Minimum = Program.typeSize.minDetected;
            chartResult.ChartAreas[0].AxisY.Minimum = 0;
            chartResult.ChartAreas[0].AxisY.Maximum = Program.typeSize.maxDetected;
            chartResult.ChartAreas[0].AxisY.Interval = 1;

            chartResult.Series.Clear();
            var ser = new Series
            {
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Column,
                Color = Color.Green,
                CustomProperties = "DrawingStyle=Emboss, PointWidth=1"
            };
            chartResult.Series.Add(ser);
        }

        private void chartResult_Click(object sender, EventArgs e)
        {
            Chart c = sender as Chart;
            MouseEventArgs mea = e as MouseEventArgs;
            HitTestResult htRes = c.HitTest(mea.X, mea.Y, ChartElementType.DataPoint);
            int zone = htRes.PointIndex;
            //FRSensorView frSensorView = new FRSensorView(this, Program.data);
            //frSensorView.Init();
            //frSensorView.Show();
        }

        private void FRResultView_Load(object sender, EventArgs e)
        {
            //восстановление размеров главного окна        
            FormPosSaver.load(this);
        }

        private void FRResultView_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(this);
        }
    }
}
