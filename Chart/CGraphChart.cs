using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Windows.Forms;

namespace CHART
{
    public delegate double CalcThickness(double x);
    public partial class CGraphChart : Chart
    {
        //! Чтобы можно было посылать массивы любого типа
        //! в нашем случае sbyte или double
        //! Масив данных отображаемых на графике
        System.Array data;

        //! Цвет для графика
        Color color = Color.Black;
        
        //! Количество курсоров
        int cursorsCount = 1;
        
        //! Курсор
        ChartCursorLine cur;
        Color curColor = Color.Black;
        public OnCursorMove onMove;

        //! Добавочный курсор по Ctrl
        ChartCursorLine cur1;
        Color cur1Color = Color.Black;
        public OnCursorMove onMove1;

        //! Функция для расчета толщины
        public CalcThickness calcThickness = null;

        //! Пока что добавим всё-таки StatusStrip прямо сюда
        StatusStrip status = null;

        string[] statusStrings = 
        {
            "Сэмпл:{0}",
            "Амплитуда:{0,5:F2}",
            "Сэмпл:{0}",
            "Амплитуда:{0,5:F2}",
            "Толщина:{0,5:F2}"
        };

        public int x 
        { 
            get 
            { 
                return cur.x; 
            }
            private set
            {
                cur.x=value;
            }
        }
        public double y { get { return cur.y; } }

        public int x1 
        { 
            get 
            { 
                return cur1.x; 
            }
            private set
            {
                cur1.x = value;
            }
        }
        public double y1 { get { return cur1.y; } }

        public CGraphChart()
        {
            InitializeComponent();
            onMove += _onMove;
            onMove1 += _onMove;
        }

        public CGraphChart(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            onMove += _onMove;
            onMove1 += _onMove;
        }

        #region Установка различных параметров
        //Вызывать желательно в этом порядке
        //Данные для отображения на графике
        public void SetData(System.Array _data)
        {
            data = _data;
        }
        //! Настройка осей и координатной сетки
        //! @ToDO Надо бы сделать на автомате
        public void SetAxis(double _intervalX, double _minY, double _maxY, double _intervalY)
        {
            ChartAreas.Clear();
            var a = new ChartArea();
            a.AxisX.Minimum = 0;
            a.AxisX.Maximum = data.Length;
            a.AxisX.Interval = _intervalX;
            a.AxisY.Minimum = _minY;
            a.AxisY.Maximum = _maxY;
            a.AxisY.Interval = _intervalY;
            a.AxisY.LabelStyle.Format = "{F2}";
            ChartAreas.Add(a);
        }
        //! @brief Сколько курсоров будем выводить на график
        public bool SetCursorCount(int _cnt = 1)
        {
            if (_cnt < 1 || _cnt > 2) return false;
            cursorsCount = _cnt;
            return true;
        }
        //! @brief Настройка цветов
        public void SetColors(Color _graphColor, Color _curColor)
        {
            color = _graphColor;
            curColor = _curColor;
        }
        public void SetColors(Color _graphColor, Color _curColor, Color _cur1Color)
        {
            color = _graphColor;
            curColor = _curColor;
            cur1Color = _cur1Color;
        }
        //! @brief Настройка статусбара
        private void InitStatusBar()
        {
            //! Добавляем статусбар
            if (status == null) status = new StatusStrip();
            status.Parent = this;
            status.SizingGrip = false;
        }

        //! @brief Натройка чарта для графика
        public void InitChart(string _title)
        {
            InitStatusBar();
            //! Пошла настройка самого чарта
            Dock = DockStyle.Fill;
            Titles.Add(_title);
            Series.Clear();
            //В эту серию поместим сам график
            var ser = new Series
            {
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                Color = color,
                ChartType = SeriesChartType.Line
            };
            Series.Add(ser);

            cur = new ChartCursorLine(this, curColor, 0, 0, data.Length, onMove, false);
            if (cursorsCount > 1)
                cur1 = new ChartCursorLine(this, cur1Color, 0, 0, data.Length, onMove1, true);
            else
                cur1 = null;
        }
        void SetCalcFunc(CalcThickness _calcFunc)
        {
            calcThickness = _calcFunc;
        }
        #endregion
        //! @brief Обработчик OnCursorMove - заполняе статус бар
        void _onMove(int x)
        {
            status.Items.Clear();
            ToolStripStatusLabel lbl;
            //! Метка для x по первому курсору
            lbl = new ToolStripStatusLabel();
            lbl.ForeColor = curColor;
            lbl.Text = string.Format(statusStrings[0], cur.x);
            status.Items.Add(lbl);
            //! Метка для y по первому курсору
            lbl = new ToolStripStatusLabel();
            lbl.ForeColor = curColor;
            lbl.Text = string.Format(statusStrings[1], cur.y);
            status.Items.Add(lbl);

            if (cursorsCount > 1)
            {
                //! Метка для x по второму курсору
                lbl = new ToolStripStatusLabel();
                lbl.ForeColor = cur1Color;
                lbl.Text = string.Format(statusStrings[2], cur1.x);
                status.Items.Add(lbl);
                //! Метка для y по второму курсору
                lbl = new ToolStripStatusLabel();
                lbl.ForeColor = cur1Color;
                lbl.Text = string.Format(statusStrings[3], cur1.y);
                status.Items.Add(lbl);
            }
            //! Метка для толщины
            if(calcThickness != null)
            {
                double val = cursorsCount > 1 ? Math.Abs(x - x1) : x;
                double th = calcThickness(val);
                status.Items.Add(new ToolStripStatusLabel(string.Format(statusStrings[4],th)));
            }
        }
        public void putDataOnChart()
        {
            if (data != null && data.Length > 0) this.putDataOnChart(data);
        }
    }
}

