using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace USPC
{
    public partial class FRWaitLongProcess : Form
    {
        public FRWaitLongProcess(Form _fr)
        {
            InitializeComponent();
            Owner = _fr;
        }
        public void setMes(string _mes)
        {
            mes.Text = _mes;
            this.Refresh();
        }
    }
}
