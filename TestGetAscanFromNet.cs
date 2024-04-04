using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;
using FPS;

namespace USPC
{
    public partial class TestGetAscanFromNet : Form
    {
        FRMain frMain;
        string serverAddress = null;
        AscanInfo info;

        public AscanInfo GetAscanInfoNet(int _board, int _channel)
        {
            AscanInfo info = new AscanInfo();

            // Part 1.  This part gets parameters to display Ascan according to: 
            //          - the scope video mode
            //          - the scope zero calibration
            //          To display gates according to:
            //          - the wave alternance selection (phase)   
            PCXUSNetworkClient client = new PCXUSNetworkClient(serverAddress);
            Object retval = new Object();
            int res = client.callNetworkFunction("readdouble,scope_video",out retval);
            if (client.callNetworkFunction("readdouble,scope_video", out retval) == 0) info.Video = (AscanInfo.VideoMode)((double)retval);
            if (client.callNetworkFunction("readdouble,scope_zero", out retval) == 0) info.ZeroVideo = (double)retval;

            if (client.callNetworkFunction("readdouble,gateIF_phase", out retval) == 0) info.GIFPhase = (AscanInfo.PhaseType)((double)retval);
            if (client.callNetworkFunction("readdouble,gate1_phase", out retval) == 0) info.G1Phase = (AscanInfo.PhaseType)((double)retval);
            if (client.callNetworkFunction("readdouble,gate2_phase", out retval) == 0) info.G2Phase = (AscanInfo.PhaseType)((double)retval);

            // Part 2.  This part gets parameters to convert Ascan data coming from acquisition to Ascan structure ready to display 
            if (client.callNetworkFunction("readdouble,gate1_trigger", out retval) == 0) info.gate1_trigger = (double)retval;
            if (client.callNetworkFunction("readdouble,gate1_position", out retval) == 0) info.gate1_position = (double)retval;
            if (client.callNetworkFunction("readdouble,gate1_width", out retval) == 0) info.gate1_width = (double)retval;
            if (client.callNetworkFunction("readdouble,gate1_level", out retval) == 0) info.gate1_level = (double)retval;
            if (client.callNetworkFunction("readdouble,gate1_nb_alarm_level", out retval) == 0) info.gate1_level_alarm = (double)retval;

            if (client.callNetworkFunction("readdouble,gate2_trigger", out retval) == 0) info.gate2_trigger = (double)retval;
            if (client.callNetworkFunction("readdouble,gate2_position", out retval) == 0) info.gate2_position = (double)retval;
            if (client.callNetworkFunction("readdouble,gate2_width", out retval) == 0) info.gate2_width = (double)retval;
            if (client.callNetworkFunction("readdouble,gate2_level", out retval) == 0) info.gate2_level = (double)retval;
            if (client.callNetworkFunction("readdouble,gate2_nb_alarm_level", out retval) == 0) info.gate2_level_alarm = (double)retval;

            if (client.callNetworkFunction("readdouble,scope_trigger", out retval) == 0) info.scope_trigger = (double)retval;
            if (client.callNetworkFunction("readdouble,scope_offset", out retval) == 0) info.scope_offset = (double)retval;
            if (client.callNetworkFunction("readdouble,scope_range", out retval) == 0) info.scope_range = (double)retval;

            return info;
        }


        public TestGetAscanFromNet(FRMain _frMain)
        {
            InitializeComponent();
            frMain = _frMain;
            MdiParent = frMain;
            try
            {
                serverAddress = Program.cmdLineArgs["Server"];
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.warning, "FRTestTcp: btnTest_Click: Error: {0}", ex.Message);
                log.add(LogRecord.LogReason.warning, "Parameter \"Server\" not assigned. Use \"127.0.0.1\"");
                serverAddress = "127.0.0.1";
            }
            info = GetAscanInfoNet(0, 0);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start")
            {
                btnStartStop.Text = "Stop";
                timer.Enabled = true;
            }
            else
            {
                btnStartStop.Text = "Start";
                timer.Enabled = false;
            }
        }

        // This function draws an Ascan and gates...
        public void UpdateAscan(Ascan Ascan, AscanInfo Info)
        {
            bool GatePositivePart;
            bool GateNegativePart;

            // Clear previous data
            AscanChart.Series["AscanPlot"].Points.Clear();

            // Set scales axis
            if (Info.Video == AscanInfo.VideoMode.RF)
                AscanChart.ChartAreas["Default"].AxisY.Minimum = -100.0;
            else
                AscanChart.ChartAreas["Default"].AxisY.Minimum = 0.0;

            AscanChart.ChartAreas["Default"].AxisY.Maximum = 100.0;
            AscanChart.ChartAreas["Default"].AxisY.Interval = 20.0;

            AscanChart.ChartAreas["Default"].AxisX.Minimum = Info.ZeroVideo + Ascan.AscanBegin / 1000.0;
            //AscanChart.ChartAreas["Default"].AxisX.Maximum = Info.ZeroVideo + Ascan.AscanBegin / 1000.0 + Ascan.TimeEqu / 1000.0;
            AscanChart.ChartAreas["Default"].AxisX.Maximum = Info.ZeroVideo + Ascan.AscanBegin / 1000.0 + Info.scope_range;


            // Draw Ascan plot
            if (Ascan.DataSize == 0) return;

            //double Step = Ascan.TimeEqu / (Ascan.DataSize - 1)  / 1000.0;
            double Step = Info.scope_range / (Ascan.DataSize - 1);
            for (int iPoint = 0; iPoint < Ascan.DataSize; iPoint++)
            {
                if (Info.Video == AscanInfo.VideoMode.RF)
                    AscanChart.Series["AscanPlot"].Points.AddXY(Info.ZeroVideo + Ascan.AscanBegin / 1000.0 + Step * iPoint, Ascan.Points[iPoint] - 127);
                else
                    AscanChart.Series["AscanPlot"].Points.AddXY(Info.ZeroVideo + Ascan.AscanBegin / 1000.0 + Step * iPoint, Ascan.Points[iPoint]);
            }

            // Draw Gate 1 plot
            AscanChart.Series["Gate1PosPlot"].Points.Clear();
            AscanChart.Series["Gate1NegPlot"].Points.Clear();
            AscanChart.Series["Gate1Pos2Plot"].Points.Clear();
            AscanChart.Series["Gate1Neg2Plot"].Points.Clear();
            if ((Ascan.G1InAscan & Ascan.GateInAscan.GateStartInAscan) == Ascan.GateInAscan.GateStartInAscan ||
                (Ascan.G1InAscan & Ascan.GateInAscan.GateEndInAscan) == Ascan.GateInAscan.GateEndInAscan)
            {
                double Begin = Info.ZeroVideo + Ascan.G1Begin / 1000.0;
                double End = Info.ZeroVideo + Ascan.G1End / 1000.0;
                double Level = Ascan.G1Level;
                double Level2 = Ascan.G1Level * Math.Pow(10.0, (Ascan.G1AlarmFilterLevel & Ascan.G1_FILTER_LEVEL_MASK) / -20.0);

                AscanChart.Series["Gate1PosPlot"].Points.AddXY(Begin, Level);
                AscanChart.Series["Gate1PosPlot"].Points.AddXY(End, Level);
                AscanChart.Series["Gate1NegPlot"].Points.AddXY(Begin, -1 * Level);
                AscanChart.Series["Gate1NegPlot"].Points.AddXY(End, -1 * Level);
                AscanChart.Series["Gate1Pos2Plot"].Points.AddXY(Begin, Level2);
                AscanChart.Series["Gate1Pos2Plot"].Points.AddXY(End, Level2);
                AscanChart.Series["Gate1Neg2Plot"].Points.AddXY(Begin, -1 * Level2);
                AscanChart.Series["Gate1Neg2Plot"].Points.AddXY(End, -1 * Level2);
            }

            switch (Info.G1Phase)
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (Info.Video >= AscanInfo.VideoMode.PositiveWave);
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.FullWave: // Full wave
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.RF: // RF
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                default:
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
            }

            AscanChart.Series["Gate1PosPlot"].Enabled = GatePositivePart;
            AscanChart.Series["Gate1NegPlot"].Enabled = GateNegativePart;
            AscanChart.Series["Gate1Pos2Plot"].Enabled = GatePositivePart;
            AscanChart.Series["Gate1Neg2Plot"].Enabled = GateNegativePart;

            // Draw Gate 2 plot
            AscanChart.Series["Gate2PosPlot"].Points.Clear();
            AscanChart.Series["Gate2NegPlot"].Points.Clear();
            AscanChart.Series["Gate2Pos2Plot"].Points.Clear();
            AscanChart.Series["Gate2Neg2Plot"].Points.Clear();
            if ((Ascan.G2InAscan & Ascan.GateInAscan.GateStartInAscan) == Ascan.GateInAscan.GateStartInAscan ||
                (Ascan.G2InAscan & Ascan.GateInAscan.GateEndInAscan) == Ascan.GateInAscan.GateEndInAscan)
            {
                double Begin = Info.ZeroVideo + Ascan.G2Begin / 1000.0;
                double End = Info.ZeroVideo + Ascan.G2End / 1000.0;
                double Level = Ascan.G2Level;
                double Level2 = Ascan.G2Level * Math.Pow(10.0, ((Ascan.G2AlarmFilterLevel & Ascan.G2_FILTER_LEVEL_MASK) >> 4) / -20.0);

                AscanChart.Series["Gate2PosPlot"].Points.AddXY(Begin, Level);
                AscanChart.Series["Gate2PosPlot"].Points.AddXY(End, Level);
                AscanChart.Series["Gate2NegPlot"].Points.AddXY(Begin, -1 * Level);
                AscanChart.Series["Gate2NegPlot"].Points.AddXY(End, -1 * Level);
                AscanChart.Series["Gate2Pos2Plot"].Points.AddXY(Begin, Level2);
                AscanChart.Series["Gate2Pos2Plot"].Points.AddXY(End, Level2);
                AscanChart.Series["Gate2Neg2Plot"].Points.AddXY(Begin, -1 * Level2);
                AscanChart.Series["Gate2Neg2Plot"].Points.AddXY(End, -1 * Level2);
            }

            switch (Info.G2Phase)
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (Info.Video >= AscanInfo.VideoMode.PositiveWave);
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.FullWave: // Full wave
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.RF: // RF
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                default:
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
            }

            AscanChart.Series["Gate2PosPlot"].Enabled = GatePositivePart;
            AscanChart.Series["Gate2NegPlot"].Enabled = GateNegativePart;
            AscanChart.Series["Gate2Pos2Plot"].Enabled = GatePositivePart;
            AscanChart.Series["Gate2Neg2Plot"].Enabled = GateNegativePart;

            // Draw Gate IF plot
            AscanChart.Series["GateIFPosPlot"].Points.Clear();
            AscanChart.Series["GateIFNegPlot"].Points.Clear();
            if ((Ascan.GIFInAscan & Ascan.GateIFInAscan.GateStartInAscan) == Ascan.GateIFInAscan.GateStartInAscan ||
                (Ascan.GIFInAscan & Ascan.GateIFInAscan.GateEndInAscan) == Ascan.GateIFInAscan.GateEndInAscan)
            {
                double Begin = Info.ZeroVideo + Ascan.GIFBegin / 1000.0;
                double End = Info.ZeroVideo + Ascan.GIFEnd / 1000.0;
                double Level = Ascan.GIFLevel;

                AscanChart.Series["GateIFPosPlot"].Points.AddXY(Begin, Level);
                AscanChart.Series["GateIFPosPlot"].Points.AddXY(End, Level);
                AscanChart.Series["GateIFNegPlot"].Points.AddXY(Begin, -1 * Level);
                AscanChart.Series["GateIFNegPlot"].Points.AddXY(End, -1 * Level);
            }

            switch (Info.GIFPhase)
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (Info.Video >= AscanInfo.VideoMode.PositiveWave);
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.FullWave: // Full wave
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                case AscanInfo.PhaseType.RF: // RF
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
                default:
                    GatePositivePart = true;
                    GateNegativePart = true;
                    break;
            }
            AscanChart.Series["GateIFPosPlot"].Enabled = GatePositivePart;
            AscanChart.Series["GateIFNegPlot"].Enabled = GateNegativePart;

        }

        
        private void timer_Tick(object sender, EventArgs e)
        {
            Object retval = new Object();
            PCXUSNetworkClient client = new PCXUSNetworkClient(serverAddress);
            int res = client.callNetworkFunction("ascan,0,0,100",out retval);
            if (res == 0)
            {
                Ascan ascan = (Ascan)retval;
                // Update gate information
                gateIF.UpdateGate(Gate.GateNum.GateIF, ascan);
                gate1.UpdateGate(Gate.GateNum.Gate1, ascan);
                gate2.UpdateGate(Gate.GateNum.Gate2, ascan);

                UpdateAscan(ascan, info);
            }
        }

        private void TestGetAscanFromNet_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }

        private void TestGetAscanFromNet_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(this);
        }

        private void TestGetAscanFromNet_Resize(object sender, EventArgs e)
        {
            AscanChart.SetBounds(0,0, ClientSize.Width-150, ClientSize.Height);
        }
    }
}
