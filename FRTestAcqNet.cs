using System;
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
        UspcNetDataReader[] dataReader = new UspcNetDataReader[Program.numBoards];
        void StartStopToggle(bool _start)
        {
            btnStart.Enabled = _start;
            btnStop.Enabled = !_start;
        }
        public FRTestAcqNet(Form _fr)
        {
            InitializeComponent();
            Owner = _fr;
            //FRWaitLongProcess waitWindow = new FRWaitLongProcess(this);
            //waitWindow.Show();
            //waitWindow.setMes("Открываем платы USPC...");
            //Program.pcxus.open(2);
            //waitWindow.setMes("Загружаем файл конфигурации...");
            //Program.pcxus.load(Program.typeSize.currentTypeSize.configName);
            //waitWindow.Close();
            FRWaitLongProcess waitWindow = new FRWaitLongProcess(this);
            waitWindow.Show();
            waitWindow.setMes("Открываем платы USPC...");
            Program.prepareBoardsForWork(true);
            waitWindow.Close();
            for (int i = 0; i < Program.numBoards; i++)
            {
                dataReader[i] = new UspcNetDataReader(i);
                dataReader[i].ProgressChanged += new ProgressChangedEventHandler(dataReader_ProgressChanged);
            }
            StartStopToggle(true);

        }

        void dataReader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int countFrames = e.ProgressPercentage;
                AcqAscan[] buffer = (AcqAscan[])e.UserState;
                updateGraph(countFrames, buffer);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }

        }
        

        public void updateGraph(int _numberOfScans,AcqAscan[] _data)
        {
            if (_numberOfScans == 0 || _data == null) return;
            try
            {
                AcqChart.Series["Gate1Amp"].Points.Clear();
                AcqChart.ChartAreas["Default"].AxisX.Minimum = 0;
                AcqChart.ChartAreas["Default"].AxisX.Maximum = _numberOfScans;

                for (int iPoint = 0; iPoint < _numberOfScans; iPoint++)
                {
                    {
                        double val = _data[iPoint].G1Amp;
                        AcqChart.Series["Gate1Amp"].Points.AddXY(iPoint, val);
                        lblGate1MaxTof.Text = val.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartStopToggle(false);
            for (int i = 0; i < Program.numBoards; i++)dataReader[i].RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StartStopToggle(true);
            for (int i = 0; i < Program.numBoards; i++)dataReader[i].CancelAsync();
        }

        private void FRTestAcq_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < Program.numBoards;i++ )
                if (dataReader[i] != null && dataReader[i].IsBusy) dataReader[i].CancelAsync();
        }

        private void FRTestAcq_Resize(object sender, EventArgs e)
        {
            AcqChart.SetBounds(0, 35, ClientSize.Width, ClientSize.Height-35);
        }
    }
}
