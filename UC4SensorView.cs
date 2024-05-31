using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using System.Collections;

namespace USPC
{
    public partial class UC4SensorView : UserControl
    {
        public UC4SensorView()
        {
            InitializeComponent();
            DoubleBuffered = false;
            Random r = new Random();
            PrepareChart(ch1);
            int[] lst = new int[50];
            for (int i = 0; i < 50; i++)
            {
                lst[i] = 90+r.Next(10);
            }
            PutDataOnChart(ch1,lst);
            PrepareChart(ch2);
            PrepareChart(ch3);
            PrepareChart(ch4);
        }
        public static void PrepareChart(Chart _c)
        {
            _c.Dock = DockStyle.Fill;
            //_c.Tag = i;
            var a = new ChartArea("Default");
            a.AxisX.Minimum = 0;
            a.AxisX.Maximum = USPCData.countZones;
            a.AxisX.Interval = 10;
            a.AxisX.LabelStyle.Enabled = false;
            a.AxisX.MajorGrid.Enabled = false;
            a.AxisY.Minimum = 0;
            a.AxisY.Maximum = 100;
            a.AxisY.Interval = 5;
            a.AxisY.LabelStyle.Enabled = false;
            a.AxisY.MajorGrid.Enabled = false;
            _c.ChartAreas.Clear();
            _c.ChartAreas.Add(a);
            _c.Series.Clear();
            var ser = new Series
            {
                Name = "ser",
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Column,
                CustomProperties = "DrawingStyle=Emboss, PointWidth=1"
            };
            _c.Series.Add(ser);
        }
        public static void PutDataOnChart(Chart _c,Array _data)
        {
            if (_data == null) return;
            _c.Series[0].Points.Clear();
            for (int i = 0; i < _data.Length; i++)
            {
                var val = _data.GetValue(i);
                _c.Series[0].Points.AddXY(i, val);
            }
        }
        /// <summary>
        /// Переопределяем виртуальную функцию для предотвращения мерцания
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Поскольку перекрываем всю поверхность то ничего не делаем        
        }

    }
}
