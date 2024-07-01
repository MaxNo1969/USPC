using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using USPC;
using Data;

namespace CHART
{
    //Для графиков
    static class ChartExtrasLine
    {
        public static string name = "Area";
        public static void addGraph(this Chart _ch, string _name, Color _color, IEnumerable<double> _data)
        {
            ChartArea a = _ch.ChartAreas.FindByName(name);
            if (a == null) _ch.ChartAreas.Add(name);
            _ch.ChartAreas[name].AxisX.Minimum = 0;
            Series ser = _ch.Series.FindByName(_name);
            if (ser == null) ser = _ch.Series.Add(_name);
            ser.Points.Clear();
            ser.ChartType = SeriesChartType.Line;
            ser.Color = _color;
            for (int i = 0; i < _data.Count(); i++)
                ser.Points.AddXY(i, _data.ElementAt(i));
        }
        public static void setYInterval(this Chart _ch, double _interval)
        {
            ChartArea a = _ch.ChartAreas.FindByName(name);
            if (a == null) _ch.ChartAreas.Add(name);
            a.AxisY.Interval = _interval;
        }
        public static void addGraph(this Chart _ch, string _name, Color _color, IEnumerable<double> _x, IEnumerable<double> _y)
        {
            ChartArea a = _ch.ChartAreas.FindByName(name);
            if (a == null) _ch.ChartAreas.Add(name);
            Series ser = _ch.Series.FindByName(_name);
            if (ser == null) ser = _ch.Series.Add(_name);
            ser.Points.Clear();
            ser.ChartType = SeriesChartType.Line;
            ser.Color = _color;
            for (int i = 0; i < _x.Count(); i++)
                ser.Points.AddXY(_x.ElementAt(i), _y.ElementAt(i));
        }
        public static void clearGraph(this Chart _ch, string _name)
        {
            Series ser = _ch.Series.FindByName(_name);
            if (ser != null) ser.Points.Clear();
        }
        public static void clearAllGraphs(this Chart _ch)
        {
            foreach (Series ser in _ch.Series) ser.Points.Clear();
        }
    }

        //Для чартов
    static class ChartExtrasChart
    {
        
        public static void putDoubleDataOnChart(this Chart chart, System.Array data,bool _isThickness)
        {
            if (data == null) return;
            chart.Series[0].Points.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                double val = (double)data.GetValue(i);
                int ind = chart.Series[0].Points.AddXY(i, val);
                chart.Series[0].Points[ind].Color = (_isThickness) ? DrawResults.GetThicknessColor(val) : DrawResults.GetThicknessColor(val);
            }
        }

        public static void putIntDataOnChart(this Chart chart, System.Array data, bool _isThickness)
        {
            if (data == null) return;
            chart.Series[0].Points.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                int val = (int)data.GetValue(i);
                int ind = chart.Series[0].Points.AddXY(i, val);
                chart.Series[0].Points[ind].Color = (_isThickness) ? DrawResults.GetThicknessColor(val) : DrawResults.GetDefectColor(val);
            }
        }

        public static void putVLineOnChart(this Chart chart, int serNum, Color color, int x, int _width = 2)
        {
            if (chart.Series.Count < serNum || chart.Series[serNum].ChartType != SeriesChartType.Line) return;
            chart.Series[serNum].Points.Clear();
            chart.Series[serNum].Color = color;
            chart.Series[serNum].BorderWidth = _width;
            chart.Series[serNum].Points.AddXY(x, chart.ChartAreas[0].AxisY.Minimum);
            chart.Series[serNum].Points.AddXY(x, chart.ChartAreas[0].AxisY.Maximum);
        }

        public static void putVLineOnChart(this Chart chart, int serNum, int x, int _width = 2)
        {
            if (chart.Series.Count < serNum || chart.Series[serNum].ChartType != SeriesChartType.Line) return;
            chart.Series[serNum].Points.Clear();
            chart.Series[serNum].BorderWidth = _width;
            chart.Series[serNum].Points.AddXY(x, chart.ChartAreas[0].AxisY.Maximum);
            chart.Series[serNum].Points.AddXY(x, chart.ChartAreas[0].AxisY.Minimum);
        }

        public static void putGLineOnChart(this Chart chart, int serNum, Color color, int x, int len, int y, int _width = 2)
        {
            if (chart.Series.Count < serNum || chart.Series[serNum].ChartType != SeriesChartType.Line) return;
            chart.Series[serNum].Points.Clear();
            chart.Series[serNum].Color = color;
            chart.Series[serNum].BorderWidth = _width;
            chart.Series[serNum].Points.AddXY(x, y);
            chart.Series[serNum].Points.AddXY(x + len, y);
        }

        public static void putGLineOnChart(this Chart chart, int serNum, int x, int len, int y, int _width = 2)
        {
            if (chart.Series.Count < serNum || chart.Series[serNum].ChartType != SeriesChartType.Line) return;
            chart.Series[serNum].Points.Clear();
            chart.Series[serNum].BorderWidth = _width;
            chart.Series[serNum].Points.AddXY(x, y);
            chart.Series[serNum].Points.AddXY(x + len, y);
        }
    }
        //Для раскраски
    static class ChartExtrasColor
    {
        //public static void putColorDecision(this Chart chart, USPCData uspcData, int zone, int sensor)
        //{
        //    if (chart.Series[0].Points == null || chart.Series[0].Points.Count == 0) return;
        //    double[] res = uspcData.evalZone(zone, sensor);
        //    for (int i = 0; i < chart.Series[0].Points.Count; i++)
        //    {
        //        chart.Series[0].Points[i].Color = DrawResults.GetColor(res[i]);
        //    }
        //}
        /*
        public static void putColorDecision(this Chart chart, USPCData uspcData, int sensor)
        {
            if (chart.Series[0].Points == null || chart.Series[0].Points.Count == 0) return;
            for (int i = 0; i < chart.Series[0].Points.Count; i++)
                chart.Series[0].Points[i].Color = DrawResults.GetColor(uspcData.offsets[i]*USPCData.countSensors+uspcData.offsSensor[sensor]);
        }
        */
        /*
        public static void putColorDecision(this Chart chart, double[] minTh, int sensor)
        {
            if (chart.Series[0].Points == null || chart.Series[0].Points.Count == 0) return;
            for (int i = 0; i < chart.Series[0].Points.Count; i++)
                chart.Series[0].Points[i].Color = DrawResults.GetDefectColor(minTh[i]);
        }
        public static void putColorDecision(this Chart chart, USPCData uspcData)
        {
            if (chart.Series[0].Points == null || chart.Series[0].Points.Count == 0) return;
            for (int i = 0; i < chart.Series[0].Points.Count; i++)
                chart.Series[0].Points[i].Color = DrawResults.GetDefectColor(uspcData.minZoneThickness[i]);
        }
        */ 
    }
}
