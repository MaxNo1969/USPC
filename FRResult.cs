using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FPS;

namespace USPC
{
    public partial class FRResult : Form
    {
        public FRResult(Form _parent)
        {
            InitializeComponent();
            DoubleBuffered = false;
            Owner = _parent;
            MdiParent = _parent;
            FormPosSaver.load(this);
            FormClosing += new FormClosingEventHandler((object sender, FormClosingEventArgs e) => FormPosSaver.save(this));
            CrossView.lblName.Text = "Поперечный контроль";
            LinearView.lblName.Text = "Продольный контроль";
            ThickView.lblName.Text = "Котроль толщины";
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
