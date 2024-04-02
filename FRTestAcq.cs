using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;

namespace USPC
{
    public partial class FRTestAcq : Form
    {
        double[] x;
        double[] g1wt;
        double[] g2wt;

        int size = 2100;
        
        FRMain frMain;
        public PCXUS pcxus;
        public PCXUS.AcqAscan[] data = null;
        UspcDataReader dataReader = null;
        
        public FRTestAcq(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            Owner = _frMain;
            MdiParent = _frMain;
            pcxus = frMain.pcxus;
            dataReader = new UspcDataReader(this);

            x = new double[size];
            g1wt = new double[size];
            g2wt = new double[size];
            for (int i = 0; i < size; i++)
            {
                x[i] = (double)i;
                g1wt[i] = 0;
                g2wt[i] = 0;
            }
        }

        public void updateGraph(PCXUS.AcqAscan[] _data)
        {
            for (int i = 0; i < _data.Count(); i++)
            {
                x[i] = (double)i;
                g1wt[i] = (double)(_data[i].G1Amp);
                g2wt[i] = (double)(_data[i].G2Amp);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (dataReader != null) dataReader.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (dataReader != null)dataReader.CancelAsync();
        }

        private void FRTestAcq_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataReader != null) dataReader.CancelAsync();
        }

    }
}
