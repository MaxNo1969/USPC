using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Windows.Forms;

namespace CHART
{
    public delegate void OnCursorMove(int x);
    public class ChartCursorLine
    {
        public OnCursorMove onMove = null;
        //! Чарт на котором находится наш курсор
        //! он всегда будет находится в chart.ChartAreas[0].CursorX
        Chart chart = null;
        //! Минимальное значение переменной связаной с курсором
        int minVal = 0;
        //! Максимальное значение переменной связаной с курсором
        int maxVal = 0;

        bool isCtrlControled;
        int cursorSer;

        private int _x;
        public int x 
        { 
            get
            {
                return _x;
            } 
            set
            {
                _x = value;
                chart.putVLineOnChart(cursorSer, _x);
                //if (onMove != null) onMove(_x);
            } 
        }
        public double y
        {
            get
            {
                return chart.Series[0].Points[_x].YValues[0];
            }
        }

        //Констуктор
        public ChartCursorLine(Chart _chart, Color _color, int _val , int _minVal, int _maxVal, OnCursorMove _onMove, bool _isCtrlControled )
        {
            chart = _chart;
            minVal = _minVal;
            maxVal = _maxVal;
            _x = _val;
            isCtrlControled = _isCtrlControled;
            //Добавим новую серию для этого курсора
            var ser = new Series
            {
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                Color = _color,
                ChartType = SeriesChartType.Line
            };
            chart.Series.Add(ser);
            cursorSer = chart.Series.Count - 1;
            //Добавим к чарту обработчики 
            chart.MouseClick += new MouseEventHandler(chart_MouseClick);
            //Для обработки стрелочек
            chart.PreviewKeyDown += new PreviewKeyDownEventHandler(chart_PreviewKeyDown);
            chart.KeyDown += new KeyEventHandler(chart_KeyDown);
            onMove += _onMove;
            chart.putVLineOnChart(cursorSer, _x);
        }

        void chart_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && !isCtrlControled) return;
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control && isCtrlControled) return;
            int newVal = _x;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    newVal--;
                    break;
                case Keys.Right:
                    newVal++;
                    break;
                default:
                    break;
            }
            if (newVal < 0) newVal = maxVal - 1;
            if (newVal > maxVal-1) newVal = 0;
            if (newVal != _x)
            {
                _x = newVal;
                chart.putVLineOnChart(cursorSer, _x);
                if (onMove != null) onMove(_x);
            }
        }

        void chart_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        void chart_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && !isCtrlControled) return;
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control && isCtrlControled) return;
            Chart c = sender as Chart;
            MouseEventArgs mea = e as MouseEventArgs;
            c.Focus();
            _x = (int)chart.ChartAreas[0].AxisX.PixelPositionToValue(mea.X);
            chart.putVLineOnChart(cursorSer, _x);
            if (onMove != null) onMove(_x);
        }
    }
}
