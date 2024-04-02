using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using USPC;
using PROTOCOL;
using FPS;
using System.Runtime.InteropServices;

namespace USPC
{
    public partial class TestUSPCGetAscan : Form
    {
        FRMain frMain;
        public TestUSPCGetAscan(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            MdiParent = frMain;
        }

        static PCXUS.Ascan BytesToAscan(byte[] bytes)
        {
            int size = Marshal.SizeOf(typeof(PCXUS.Ascan));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, ptr, size);
                return (PCXUS.Ascan)Marshal.PtrToStructure(ptr, typeof(PCXUS.Ascan));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        static byte[] AscanToBytes<T>(PCXUS.Ascan ascan)
        {
            int size = Marshal.SizeOf(ascan);
            byte[] arr = new byte[size];
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(ascan, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }
        
        private void PCXUS_ACQ_ASCAN_net(int _board,int  _channel, ref PCXUS.Ascan _ascan, int _timeout)
        {
            string serverAddr = Program.cmdLineArgs["Server"];
            TcpCommand cmd = new TcpCommand(serverAddr);
            byte[] bytes = cmd.sendCommand(string.Format("{0},{1},{2},{3}", "PCXUS_ACQ_ASCAN", _board, _channel, _timeout));
            _ascan = BytesToAscan(bytes);
        }
        
        private void timerAscan_Tick(object sender, EventArgs e)
        {
            // A timer event to get an A-scan
            PCXUS.Ascan Ascan = new PCXUS.Ascan();
            Int32 err = 0;
            int CurrentBoard = 0;
            int CurrentChannel = 0;

            err = PCXUS.PCXUS_ACQ_ASCAN(CurrentBoard, CurrentChannel, ref Ascan, 20);
            if (err != 0)
            {
                return;
            }
            // Clear previous data
            AscanChart.Series["AscanPlot"].Points.Clear();

            AscanChart.ChartAreas["Default"].AxisY.Minimum = 0.0;
            AscanChart.ChartAreas["Default"].AxisY.Maximum = 100.0;
            AscanChart.ChartAreas["Default"].AxisY.Interval = 20.0;

            AscanChart.ChartAreas["Default"].AxisX.Minimum = Ascan.AscanBegin / 1000.0;
            AscanChart.ChartAreas["Default"].AxisX.Maximum = Ascan.AscanBegin / 1000.0 + Ascan.TimeEqu / 1000.0;
            
            // Draw Ascan plot
            if (Ascan.DataSize == 0) return;

            double Step = Ascan.TimeEqu / (Ascan.DataSize - 1)  / 1000.0;
            for (int iPoint = 0; iPoint < Ascan.DataSize; iPoint++)
            {
                AscanChart.Series["AscanPlot"].Points.AddXY(Ascan.AscanBegin / 1000.0 + Step * iPoint, Ascan.Points[iPoint]);
            }
        }

        private void tbbtnStart_Click(object sender, EventArgs e)
        {
            timerAscan.Enabled = true;
        }

        private void tbbtnStop_Click(object sender, EventArgs e)
        {
            timerAscan.Enabled = false;
        }

        private void TestUSPCGetAscan_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }

        private void TestUSPCGetAscan_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(this);
        }
    }
}
