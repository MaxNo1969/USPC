using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Windows.Forms;

namespace CHART
{   
    //Управление курсром на чарте
    public class ChartCursorColumn
    {
        public OnCursorMove onMove = null;
        //! Чарт на котором находится наш курсор
        //! он всегда будет находится в chart.ChartAreas[0].CursorX
        Chart chart = null;
        //! Минимальное значение переменной связаной с курсором
        int minVal = 0;
        //! Максимальное значение переменной связаной с курсором
        int maxVal = 0;

        //Констуктор
        public ChartCursorColumn(Chart _chart, Color _color, int _val , int _minVal, int _maxVal, OnCursorMove _onMove = null)
        {
            chart = _chart;
            minVal = _minVal;
            maxVal = _maxVal;

            //Создадим наш курсор
            ChartArea area = chart.ChartAreas[0];
            area.CursorX = new System.Windows.Forms.DataVisualization.Charting.Cursor()
            {
                LineColor = _color,
                LineWidth = 2,
                Position = _val
            };
            //Добавим к чарту обработчики 
            chart.MouseClick += new System.Windows.Forms.MouseEventHandler(chart_MouseClick);
            //Для обработки стрелочек
            chart.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(chart_PreviewKeyDown);
            chart.KeyDown += new System.Windows.Forms.KeyEventHandler(chart_KeyDown);
            onMove += _onMove;
        }

        void chart_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int val = (int)chart.ChartAreas[0].CursorX.Position;
            int newVal = val;
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
            if (newVal != val)
            {
                chart.ChartAreas[0].CursorX.Position = newVal;
                if (onMove != null) onMove(x);
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
            Chart c = sender as Chart;
            MouseEventArgs mea = e as MouseEventArgs;
            c.Focus();
            c.ChartAreas[0].CursorX.SetCursorPixelPosition(mea.Location, true);
            if (onMove != null) onMove(x);
        }

        public int x
        {
            get { return (int)chart.ChartAreas[0].CursorX.Position; }
        }
        public double y
        {
            get { return chart.Series[0].Points[x].YValues[0]; }
        }
        public void moveTo(int x)
        {
            chart.ChartAreas[0].CursorX.Position = x;
        }
    }
}
