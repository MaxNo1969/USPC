﻿using System;
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
using USPC.Data;

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
            PrepareChart(ch2);
            PrepareChart(ch3);
            PrepareChart(ch4);
        }
        public static void PrepareChart(Chart _c)
        {
            _c.Dock = DockStyle.Fill;
            //_c.Tag = i;
            var a = new ChartArea("Default");
            //a.InnerPlotPosition.Auto = false;
            //a.InnerPlotPosition.X = 0;
            //a.InnerPlotPosition.Y = 0;
            //a.InnerPlotPosition.Width = 100;
            //a.InnerPlotPosition.Height = 100;

            //a.AxisY.Crossing = 0;
            a.AxisY.MajorTickMark.Enabled = false;
            
            a.AxisX.Minimum = 0;
            a.AxisX.Maximum = Program.countZones;
            a.AxisX.Interval = 10;
            a.AxisX.LabelStyle.Enabled = true;
            a.AxisX.MajorGrid.Enabled = true;
            
            a.AxisY.Minimum = 0;
            //a.AxisY.Maximum = 100;
            a.AxisY.Maximum = double.NaN;

            a.AxisY.Interval = 0;
            a.AxisY.LabelStyle.Enabled = false;
            a.AxisY.MajorGrid.Enabled = false;

            a.CursorX.Interval = 1;
            a.CursorX.IsUserEnabled = true;
            a.CursorX.IsUserSelectionEnabled = false;
            a.CursorX.LineColor = Color.Black;
            a.CursorX.LineWidth = 3;
            a.CursorX.Position = 0;

            _c.ChartAreas.Clear();
            _c.ChartAreas.Add(a);
            _c.Series.Clear();
            var ser = new Series
            {
                Name = "ser",
                Color = Color.ForestGreen,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                BorderColor = Color.Black,
                BorderWidth = 1,
                ChartType = SeriesChartType.Column,
                //CustomProperties = "DrawingStyle=Emboss, PointWidth=1"                
                CustomProperties = "PointWidth=1"                
            };
            _c.Series.Add(ser);
        }
        public static void PutDefDataOnChart(Chart _c,double[] _data)
        {
            if (_data == null) return;
            _c.Series[0].Points.Clear();
            for (int i = 0; i < _data.Length; i++)
            {
                int ind = _c.Series[0].Points.AddXY(i+1, 100);
                int val = (int)_data[i];
                _c.Series[0].Points[ind].Color = DrawResults.GetDefectColor(val);
            }
        }
        public static void PutThickDataOnChart(Chart _c, double[] _data)
        {
            if (_data == null) return;
            _c.Series[0].Points.Clear();
            for (int i = 0; i < _data.Length; i++)
            {
                double val = _data[i];
                int ind = _c.Series[0].Points.AddXY(i+1, val);
                _c.Series[0].Points[ind].Color = DrawResults.GetThicknessColor(val);
            }
        }

        public static void ClearChart(Chart _c)
        {
            _c.Series[0].Points.Clear();
        }
        /// <summary>
        /// Переопределяем виртуальную функцию для предотвращения мерцания
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Поскольку перекрываем всю поверхность то ничего не делаем        
        }

        private void ch_Click(object sender, EventArgs e)
        {
            Chart c = sender as Chart;
            int sensor = (int)c.Tag;
            int zone = (int)c.ChartAreas[0].CursorX.Position-1;
            if (zone > Program.result.zone || zone < 0) return;
            ListZones values = Program.result.values;
            int count = values[zone][sensor].Count;
            double[] data = new double[count];
            for (int i = 0; i < count; i++)
            {
                if (sensor < 4)
                {
                    uint tof = (values[zone][sensor][i].G1TofWt & Ascan.TOF_MASK) * 5;
                    data[i] = ThickConverter.TofToMm(tof);
                }
                else
                    data[i] = values[zone][sensor][i].G1Amp;
            }
            FRZoneView frm = new FRZoneView(Program.frMain);
            frm.sensor = sensor;
            frm.zone = zone;
            frm.UpdateChart(zone, sensor, Program.result.zonesLengths[zone]);
            frm.Show();
        }
    }
}
