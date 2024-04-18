using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FORMS;

namespace CHART
{
    /// <summary>
    /// Форма для отображения графиков
    /// </summary>
    public partial class FRChart : MNMDIForm
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public FRChart(Form _frm):base(_frm)
        {
            InitializeComponent();
            chart.ChartAreas.Add("Area");
        }
        /// <summary>
        /// Добавляем данные на график
        /// </summary>
        /// <param name="_name">Наименование серии</param>
        /// <param name="_color">Цвет</param>
        /// <param name="_x">Данные x</param>
        /// <param name="_y">Данные y</param>
        public void addGraph(string _name,Color _color,IEnumerable<double> _x, IEnumerable<double> _y)
        {
            chart.addGraph(_name, _color, _x,_y);
        }
        /// <summary>
        /// Добавляем данные на график
        /// </summary>
        /// <param name="_name">Наименование серии</param>
        /// <param name="_color">Цвет</param>
        /// <param name="_data">Данные</param>
        public void addGraph(string _name, Color _color, IEnumerable<double> _data)
        {
            chart.addGraph(_name, _color, _data);
        }
    }
}
