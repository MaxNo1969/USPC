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
        public int zone;
        public int sensor;
        public FRResultView(Form _form)
        {
            InitializeComponent();
            Owner = _form;
            KeyPreview = true;
            setupResultChart();
        }

        public void UpdateChart(double[] _data, bool _isThick)
        {
            if (_data == null || _data.Length == 0) return;
            chartResult.Series[0].Points.Clear();
            double step = 200 / _data.Length;
            for (int i = 0; i < _data.Length; i++)
            {
                double val = (double)_data.GetValue(i);
                int ind = chartResult.Series[0].Points.AddXY(i * step, val);
                chartResult.Series[0].Points[ind].Color = (_isThick) ? DrawResults.GetThicknessColor(val) : DrawResults.GetDefectColor((int)val);
            }

        }

        public void UpdateChart(int _zone,int _sensor)
        {
            ListZones values = Program.result.values;
            int count = values[zone][sensor].Count;
            double[] data = new double[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = values[zone][sensor][i];
            }

            if (sensor < 4)
            {
                Text = string.Format("Толщинометрия: Зона:{0} Датчик: {1}", zone, sensor);
                UpdateChart(data, true);
            }
            else if (sensor < 8)
            {
                Text = string.Format("Продольная дефектоскопия: Зона:{0} Датчик: {1}", zone, sensor - 4);
                UpdateChart(data, false);
            }
            else
            {
                Text = string.Format("Поперечная дефектоскопия: Зона:{0} Датчик: {1}", zone, sensor - 8);
                UpdateChart(data, false);
            }
        }

        /// <summary>
        /// Настройка основного чарта
        /// </summary>
        private void setupResultChart()
        {
            
            //chartResult.ChartAreas[0].InnerPlotPosition.Auto = false;
            //chartResult.ChartAreas[0].InnerPlotPosition.X = 0;
            //chartResult.ChartAreas[0].InnerPlotPosition.Y = 0;
            //chartResult.ChartAreas[0].InnerPlotPosition.Width = 100;
            //chartResult.ChartAreas[0].InnerPlotPosition.Height = 100;

            chartResult.ChartAreas[0].AxisX.Minimum = 0;
            chartResult.ChartAreas[0].AxisX.Maximum = 200;
            chartResult.ChartAreas[0].AxisX.Interval = 10;
            //chartResult.ChartAreas[0].AxisY.Minimum = 0;
            //chartResult.ChartAreas[0].AxisY.Maximum = Program.typeSize.maxDetected;
            //chartResult.ChartAreas[0].AxisY.Interval = 1;

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                    {
                        if (zone < Program.result.zones - 1)
                            zone++;
                        else
                            zone = 0;
                        UpdateChart(zone, sensor);
                        return true;
                    }
                case Keys.Left:
                    {
                        if (zone > 0)
                            zone--;
                        else
                            zone = Program.result.zones - 1;
                        UpdateChart(zone, sensor);
                        return true;
                    }
                case Keys.Up:
                    {
                        if (sensor > 0)
                            sensor--;
                        else
                            sensor = Program.result.sensors - 1;
                        UpdateChart(zone, sensor);
                        return true;
                    }
                case Keys.Down:
                    {
                        if (sensor < Program.result.sensors - 1)
                            sensor++;
                        else
                            sensor = 0;
                        UpdateChart(zone, sensor);
                        return true;
                    }
                case Keys.Escape:
                    {
                        Close();
                        return true;
                    }
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

    }
}
