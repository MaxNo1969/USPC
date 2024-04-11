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
        FRMain frMain;
        public PCXUS pcxus;
        public AcqAscan[] data = null;
        UspcDataReader dataReader = null;
        AscanInfo info;

        public FRTestAcq(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            Owner = _frMain;
            MdiParent = _frMain;
            pcxus = frMain.pcxus;
            int board = 0;
            int channel = 0;
            info = pcxus.GetAscanInfo(board, channel);
            dataReader = new UspcDataReader(this);
            //dataReader.dataAcquired += updateGraph;
        }

        public void updateGraph(int _numberOfScans,AcqAscan[] _data)
        {
            try
            {
                // Clear previous data
                AcqChart.Series["Gate1TOF"].Points.Clear();
                AcqChart.Series["Gate2TOF"].Points.Clear();
                AcqChart.Series["GateIFTOF"].Points.Clear();


                AcqChart.ChartAreas["Default"].AxisY.Maximum = 100.0;
                AcqChart.ChartAreas["Default"].AxisY.Interval = 10.0;

                AcqChart.ChartAreas["Default"].AxisX.Minimum = 0;
                AcqChart.ChartAreas["Default"].AxisX.Maximum = _numberOfScans;

                for (int iPoint = 0; iPoint < _numberOfScans; iPoint++)
                {
                    AcqChart.Series["Gate1TOF"].Points.AddXY(iPoint, _data[iPoint].G1QofC);
                    AcqChart.Series["Gate2TOF"].Points.AddXY(iPoint, _data[iPoint].G2QofC);
                    AcqChart.Series["GateIFTOF"].Points.AddXY(iPoint, _data[iPoint].GIFCouplingAlarm);
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
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

        private void FRTestAcq_Resize(object sender, EventArgs e)
        {
            AcqChart.SetBounds(0, 35, ClientSize.Width, ClientSize.Height-35);
        }
    }
}
