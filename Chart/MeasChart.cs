using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Drawing;
using Data;
using USPC;


namespace CHART
{
    class MeasChart:Chart
    {
        USPCData data;
        int zone;
        public void setZone(int _zone)
        {
            zone = _zone;
        }
        int sensor;
        int maxX;
        public ChartCursorColumn curMeas;
        public int x
        {
            get { return curMeas.x; }
        }
        public double y
        {
            get { return curMeas.y; }
        }
        public MeasChart() : base() { }

        public void Init(Control _parent, int _yDelta, USPCData _data, int _zone, int _sensor, int _offset, 
            OnCursorMove onMove = null)
        {
            data = _data;
            zone = _zone;
            sensor = _sensor;

            Parent = _parent;
            Width = Parent.Width;
            Height = Parent.Height - _yDelta;

            Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            Location = new Point(0, 0);

            maxX = (data.offsets[zone+1]-data.offsets[zone])/USPCData.countSensors;
            //Настройка параметров чарта
            //Titles.Add("Измерения по зоне");
            var a = new ChartArea();
            /*          
            a.InnerPlotPosition.Auto = false;
            a.InnerPlotPosition.X = 10;
            a.InnerPlotPosition.Width = 100;
            a.InnerPlotPosition.Y = 10;
            a.InnerPlotPosition.Height = 100;
            */ 
            //a.AxisY.Title = _sensor.ToString();
            //a.AxisY.TextOrientation = TextOrientation.Horizontal;

            a.AxisX.Minimum = 0;
            a.AxisX.Maximum = maxX;
            a.AxisX.Interval = 100.0;
            a.AxisX.LabelAutoFitMaxFontSize = 6;
            a.AxisX.LabelAutoFitMinFontSize = 6;
            a.AxisY.Minimum = 0;
            a.AxisY.Maximum = Program.typeSize.maxDetected;
            a.AxisY.Interval = 5;
            a.AxisY.LabelAutoFitMaxFontSize = 6;
            a.AxisY.LabelAutoFitMinFontSize = 6;
            ChartAreas.Clear();
            ChartAreas.Add(a);
            Series.Clear();
            //В эту серию поместим сам график
            var ser = new Series
            {
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                Color = Color.Green,
                ChartType = SeriesChartType.Column,
                CustomProperties = "PointWidth=1"
            };
            Series.Add(ser);
            curMeas = new ChartCursorColumn(this, Color.Black, _offset, 0, maxX, onMove);
            //curMeas.onMove += new OnCursorMove(putLegend);
            putDataOnChart();
            //Легенда инициализация
            /*
            var legend = new Legend();
            legend.Docking = Docking.Left;
            legend.Alignment = StringAlignment.Near;
            LegendItem li;
            //Зона
            li = new LegendItem();
            li.Cells.Add(new LegendCell(string.Format("Зона {0}", zone)));
            legend.CustomItems.Add(li);
            //Датчик
            li = new LegendItem();
            li.Cells.Add(new LegendCell(string.Format("Датчик {0}", sensor + 1)));
            legend.CustomItems.Add(li);
            //Отсчет
            li = new LegendItem();
            li.Cells.Add(new LegendCell(string.Format("Измерение {0}", curMeas.x)));
            legend.CustomItems.Add(li);
            //Диаметр
            li = new LegendItem();
            li.Cells.Add(new LegendCell(string.Format("{0:00.00}", curMeas.y)));
            legend.CustomItems.Add(li);
            Legends.Add(legend);
            putLegend(0);
            */ 
        }
        //Легенда (экспериментально)
        public void putLegend(int _pos)
        {
            //Legends[0].CustomItems[0].Cells[0].Text = string.Format("Зона {0}", zone);
            //Legends[0].CustomItems[1].Cells[0].Text = string.Format("Датчик {0}", sensor + 1);
            //Legends[0].CustomItems[2].Cells[0].Text = string.Format("Измерение {0}", curMeas.x);
            //Legends[0].CustomItems[3].Cells[0].Text = string.Format("{0:00.00}", curMeas.y);
        }

        public void putDataOnChart()
        {
            //this.putDataOnChart(data.evalZone(zone, sensor));
            //this.putColorDecision(data, zone, sensor);
        }
    }
}
