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
using System.Threading.Tasks;
using Settings;

namespace USPC
{
    public partial class FRShowAscan : Form
    {
        Ascan ascan;
        AscanInfo info;
        int board = 0;
        int test = 0;
        int meas = 0;

        public double GetVal(string _paramName)
        {
            return PCXUSNET.GetVal(board, test, _paramName);
        }

        public AscanInfo GetAscanInfoNet(int _board, int _channel)
        {
            AscanInfo info = new AscanInfo();
            // Part 1.  This part gets parameters to display Ascan according to: 
            //          - the scope video mode
            //          - the scope zero calibration
            //          To display gates according to:
            //          - the wave alternance selection (phase)   
            //PCXUSNetworkClient client = new PCXUSNetworkClient(Program.serverAddr);
            //Object retval = new Object();
            ///double val;
            info.Video = (AscanInfo.VideoMode)GetVal("scope_video");
            info.ZeroVideo = GetVal("scope_zero");
            info.GIFPhase = (AscanInfo.PhaseType)GetVal("gateIF_phase");
            info.G1Phase = (AscanInfo.PhaseType)GetVal("gate1_phase");
            info.G2Phase = (AscanInfo.PhaseType)GetVal("gate2_phase");
            // Part 2.  This part gets parameters to convert Ascan data coming from acquisition to Ascan structure ready to display 
            info.gate1_trigger = GetVal("gate1_trigger");
            info.gate1_position = GetVal("gate1_position");
            info.gate1_level = GetVal("gate1_level");
            info.gate1_level_alarm = GetVal("gate1_nb_alarm_level");

            info.gate2_trigger = GetVal("gate2_trigger");
            info.gate2_position = GetVal("gate2_position");
            info.gate2_level = GetVal("gate2_level");
            info.gate2_level_alarm = GetVal("gate2_nb_alarm_level");

            info.scope_trigger = GetVal("scope_trigger");
            info.scope_offset = GetVal("scope_offset");
            info.scope_range = GetVal("scope_range");
            return info;
        }


        public FRShowAscan(int _board, int _test, int _meas,Ascan _ascan,FRMain _frMain)
        {
            InitializeComponent();
            board = _board;
            test = _test;
            meas = _meas;
            label6.Text = board.ToString();
            label7.Text = test.ToString();
            label9.Text = meas.ToString();
            ascan = _ascan;
            info = GetAscanInfoNet(board,test);
            gateIF.UpdateGate(Gate.GateNum.GateIF, ascan);
            gate1.UpdateGate(Gate.GateNum.Gate1, ascan);
            gate2.UpdateGate(Gate.GateNum.Gate2, ascan);

            UpdateAscan(ascan, info);

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
            //if ((Ascan.G1InAscan & Ascan.GateInAscan.GateStartInAscan) == Ascan.GateInAscan.GateStartInAscan ||
            //    (Ascan.G1InAscan & Ascan.GateInAscan.GateEndInAscan) == Ascan.GateInAscan.GateEndInAscan)
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
            //if ((Ascan.G2InAscan & Ascan.GateInAscan.GateStartInAscan) == Ascan.GateInAscan.GateStartInAscan ||
            //    (Ascan.G2InAscan & Ascan.GateInAscan.GateEndInAscan) == Ascan.GateInAscan.GateEndInAscan)
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
            //if ((Ascan.GIFInAscan & Ascan.GateIFInAscan.GateStartInAscan) == Ascan.GateIFInAscan.GateStartInAscan ||
            //    (Ascan.GIFInAscan & Ascan.GateIFInAscan.GateEndInAscan) == Ascan.GateIFInAscan.GateEndInAscan)
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
            AscanChart.SetBounds(0,58, ClientSize.Width-150, ClientSize.Height-58);
        }
    }
}
