using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Data;
using PROTOCOL;
using CHART;
using FPS;

namespace USPC
{
    partial class FRSensorView : Form
    {
        List<Chart> charts;
        List<ChartCursorColumn> cursors;
        USPCData data;

        public FRSensorView(Form _owner,USPCData _data)
        {
            InitializeComponent();
            Owner = _owner;
            KeyPreview = true;
            KeyDown += new KeyEventHandler(FRSensorView_KeyDown);
            data = _data;
            charts = new List<Chart>();
            cursors = new List<ChartCursorColumn>();
            Text = string.Format("Зона: {0}, {1:00.00}", 0, data.minZoneThickness[0]);
        }

        void FRSensorView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;
                default:
                    break;
            }
        }

        public void Init()
        {
            int sensorsCount = USPCData.countSensors;
            lay.RowCount = sensorsCount;
            lay.RowStyles.Clear();
            for (int i = 0; i < lay.RowCount; i++)
                lay.RowStyles.Add(new RowStyle(SizeType.Percent, (float)(100.0 / sensorsCount)));

            for (int i = 0; i < sensorsCount; i++)
            {
                Chart c = new Chart();
                //Для обработки стрелочек
                lay.SetRow(c, i);
                c.Dock = DockStyle.Fill;
                c.Tag = i;
                var a = new ChartArea("Thick");
                a.AxisX.Minimum = 0;
                a.AxisX.Maximum = USPCData.countZones;
                a.AxisX.Interval = 10;
                a.AxisY.Minimum = 0;
                a.AxisY.Maximum = Program.typeSize.maxDetected;
                a.AxisY.Interval = 5;
                c.ChartAreas.Clear();
                c.ChartAreas.Add(a);
                c.Series.Clear();
                var ser = new Series
                {
                    IsVisibleInLegend = false,
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Column,
                    CustomProperties = "DrawingStyle=Emboss, PointWidth=1"
                };
                c.Series.Add(ser);
                c.putDataOnChart(data.minZoneSensorThickness[i]);
                c.putColorDecision(data.minZoneSensorThickness[i], i);
                c.DoubleClick += new EventHandler(c_DoubleClick);
                c.Parent = lay;
                var legend = new Legend();
                legend.Docking = Docking.Left;
                LegendItem li = new LegendItem();
                li.Cells.Add(new LegendCell(string.Format("Датчик {0}", i+1)));
                legend.CustomItems.Add(li);
                li = new LegendItem();
                li.Cells.Add(new LegendCell(string.Format("{0:00.00}",data.minZoneThickness[0])));
                legend.CustomItems.Add(li);
                c.Legends.Add(legend);
                charts.Add(c);
                ChartCursorColumn cur = new ChartCursorColumn(c, Color.Black, 0, 0, USPCData.countZones, chartCursorMove);
                //cur.onMove += chartCursorMove;
                cursors.Add(cur);
            }
        }

        void c_DoubleClick(object sender, EventArgs e)
        {
            Chart c = sender as Chart;
            MouseEventArgs mea = e as MouseEventArgs;
            HitTestResult htRes = c.HitTest(mea.X, mea.Y, ChartElementType.DataPoint);
            int zone = htRes.PointIndex;
            int sensor = (int)c.Tag;
            FRDetails md = new FRDetails(this, data, zone, sensor, 0);
            md.Show();
        }
        void chartCursorMove(int x)
        {
            //foreach (ChartCursorColumn cur in cursors)
            //    cur.moveTo(x);
            for (int i = 0; i < USPCData.countSensors; i++)
            {
                cursors[i].moveTo(x);
                charts[i].Legends[0].CustomItems[1].Cells[0].Text = string.Format("{0:00.00}", cursors[i].y);
            }
            Text = string.Format("Зона: {0}, {1:00.00}", x, data.minZoneThickness[x]);
        }

        private void FRSensorView_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }

        private void FRSensorView_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(this);
        }
    }
}
