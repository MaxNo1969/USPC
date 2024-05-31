﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;
using Data;

namespace USPC
{
    public partial class FRTestAcqNet : Form
    {
        public FRMain frMain;
        //public PCXUS pcxus;
        public AcqAscan[] data = new AcqAscan[1024*100];
        UspcNetDataReaderForTest dataReader = null;
        void StartStopToggle(bool _start)
        {
            btnStart.Enabled = _start;
            btnStop.Enabled = !_start;
        }
        public FRTestAcqNet(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            Owner = _frMain;
            dataReader = new UspcNetDataReaderForTest(this);
            StartStopToggle(true);

        }

        public void updateGraph(int _numberOfScans,AcqAscan[] _data)
        {
            try
            {
                AcqChart.Series["Gate1Amp"].Points.Clear();
                AcqChart.Series["Gate2Amp"].Points.Clear();
                
                //AcqChart.ChartAreas["Default"].AxisY.Minimum = 0.0;
                //AcqChart.ChartAreas["Default"].AxisY.Maximum = 200.0;
                //AcqChart.ChartAreas["Default"].AxisY.Interval = 10.0;

                AcqChart.ChartAreas["Default"].AxisX.Minimum = 0;
                AcqChart.ChartAreas["Default"].AxisX.Maximum = _numberOfScans;

                for (int iPoint = 0; iPoint < _numberOfScans; iPoint++)
                {
                    {
                        //uint tof = _data[iPoint].G1Tof & AcqAscan.TOF_MASK;
                        //double val = 2.5e-6 * tof * USPCData.scopeVelocity;
                        double val = _data[iPoint].G1Amp;
                        AcqChart.Series["Gate1Amp"].Points.AddXY(iPoint, val);
                        lblGate1MaxTof.Text = val.ToString();
                    }
                    {
                        //uint tof = _data[iPoint].G2Tof & AcqAscan.TOF_MASK;
                        //double val = 2.5e-6 * tof * USPCData.scopeVelocity;
                        double val = _data[iPoint].G1Amp;
                        AcqChart.Series["Gate2Amp"].Points.AddXY(iPoint, val);
                        lblGate2MaxTof.Text = val.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartStopToggle(false);
            if (dataReader != null) dataReader.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StartStopToggle(true);
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
