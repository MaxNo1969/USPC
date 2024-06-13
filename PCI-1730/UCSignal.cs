using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PCI1730
{
    /// <summary>
    /// UserControl для отображения одного обрабатываемого сигнала с PCIE-1720
    /// </summary>
    public partial class UCSignal : UserControl
    {
        private Signal s=null;
        bool input=false;
        Color color_false;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_sIn">Входящий сигнал</param>
        public UCSignal(Signal _s,bool _isInput)
        {
            InitializeComponent();
            s=_s;
            input = _isInput;
        }

        private void UCSignal_Load(object sender, EventArgs e)
        {
            if(input)
            {
                label1.Text = string.Format("{0} {1}", s.position, s.name);
                TT.SetToolTip(this,s.hint);
                TT.SetToolTip(label1,s.hint);
            }
            else
            {
                label1.Text = string.Format("{0} {1}", s.position, s.name);
                TT.SetToolTip(this,s.hint);
                TT.SetToolTip(label1,s.hint);
            }
            label1.Top = (ClientSize.Height - label1.Height) / 2;
            if (label1.Top < 0)
                label1.Top = 0;
            label1.Left = 2;
            color_false = BackColor;
            TT.Active = false;
            TT.Active = true;
        }
        public void Exec()
        {
            if (input)
            {
                if (s.Val)
                    BackColor = Color.Green;
                else
                    BackColor = color_false;
            }
            else
            {
                if (s.Val)
                    BackColor = Color.Red;
                else
                    BackColor = color_false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (!input)
            {
                //bool b = sOut.Val;
                //if (b == true) sOut.Val = false;
                //if (b == false) sOut.Val = true;
                s.Val = !s.Val;
                Exec();                
            }
            TT.Active = false;
            TT.Active = true;
        }
    }
}
