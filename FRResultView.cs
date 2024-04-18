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
            chartResult.putDataOnChart(Program.data.commonStatus);
            chartResult.putColorDecision(Program.data);
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
            chartResult.ChartAreas[0].AxisX.Maximum = Program.countZones;
            chartResult.ChartAreas[0].AxisX.Interval = 10;
            chartResult.ChartAreas[0].AxisY.Minimum = 2;
            chartResult.ChartAreas[0].AxisY.Maximum = 6;
            chartResult.ChartAreas[0].AxisY.Interval = 0.1;

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

    }
}
