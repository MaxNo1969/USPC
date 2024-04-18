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
    public partial class FRTestAcqNet : Form
    {
        public FRMain frMain;
        //public PCXUS pcxus;
        public string serverAddr;
        public AcqAscan[] data = new AcqAscan[1024*100];
        UspcNetDataReader dataReader = null;

        public FRTestAcqNet(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            Owner = _frMain;
            MdiParent = _frMain;
            //dataReader.dataAcquired += updateGraph;
            try
            {
                serverAddr = Program.cmdLineArgs["Server"];
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.warning, "FRTestTcp: btnTest_Click: Error: {0}", ex.Message);
                log.add(LogRecord.LogReason.warning, "Parameter \"Server\" not assigned. Use \"127.0.0.1\"");
                serverAddr = "127.0.0.1";
            }
            dataReader = new UspcNetDataReader(this);
        }

        public void updateGraph(int _numberOfScans,AcqAscan[] _data)
        {
            try
            {
                // Clear previous data
                //AcqChart.Series["Gate1TOF"].Points.Clear();
                //AcqChart.Series["Gate2TOF"].Points.Clear();
                //AcqChart.Series["GateIFTOF"].Points.Clear();
                
                
                //double[] Gate1Amp = new double[_numberOfScans];
                //double[] Gate2Amp = new double[_numberOfScans];
                //for (int i = 0; i < _numberOfScans; i++)
                //{
                //    Gate1Amp[i] = (double)_data[i].G1Amp;
                //    Gate2Amp[i] = (double)_data[i].G2Amp;
                //}

                //double[] Gate1AmpFiltered = Median.Filter(Gate1Amp, 5);
                //double[] Gate2AmpFiltered = Median.Filter(Gate2Amp, 5);


                AcqChart.Series["Gate1Amp"].Points.Clear();
                AcqChart.Series["Gate2Amp"].Points.Clear();

                AcqChart.ChartAreas["Default"].AxisY.Maximum = 100.0;
                AcqChart.ChartAreas["Default"].AxisY.Interval = 10.0;

                AcqChart.ChartAreas["Default"].AxisX.Minimum = 0;
                AcqChart.ChartAreas["Default"].AxisX.Maximum = _numberOfScans;

                for (int iPoint = 0; iPoint < _numberOfScans; iPoint++)
                {
                    //AcqChart.Series["Gate1TOF"].Points.AddXY(iPoint, _data[iPoint].G1QofC);
                    //AcqChart.Series["Gate2TOF"].Points.AddXY(iPoint, _data[iPoint].G2QofC);
                    //AcqChart.Series["GateIFTOF"].Points.AddXY(iPoint, _data[iPoint].GIFCouplingAlarm);

                    AcqChart.Series["Gate1Amp"].Points.AddXY(iPoint, _data[iPoint].G1Amp);
                    AcqChart.Series["Gate2Amp"].Points.AddXY(iPoint, _data[iPoint].G2Amp);

                    //AcqChart.Series["Gate1Amp"].Points.AddXY(iPoint, Gate1AmpFiltered[iPoint]);
                    //AcqChart.Series["Gate2Amp"].Points.AddXY(iPoint, Gate2AmpFiltered[iPoint]);
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
            //Program.data.saveAsync("data.bin");
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
