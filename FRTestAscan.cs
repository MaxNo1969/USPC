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
    public partial class FRTestAscan : Form
    {
        //AscanInfo info;
        int board = 0;
        int test = 0;
        int timeout = 0;
        AscanParams pars = Program.ascanParams;

        public FRTestAscan(FRMain _frMain)
        {
            InitializeComponent();
            cbBoards.SelectedIndex = 0;
            cbTest.SelectedIndex = 0;
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

        double par(string _name)
        {
            return pars.get(board, test, _name);
        }

        // This function draws an Ascan and gates...
        public void UpdateAscan(Ascan Ascan)
        {
            bool GatePositivePart;
            bool GateNegativePart;

            // Clear previous data
            AscanChart.Series["AscanPlot"].Points.Clear();

            // Set scales axis
            AscanChart.ChartAreas["Default"].AxisY.Minimum = 0.0;

            AscanChart.ChartAreas["Default"].AxisY.Maximum = 100.0;
            AscanChart.ChartAreas["Default"].AxisY.Interval = 20.0;

            AscanChart.ChartAreas["Default"].AxisX.Minimum = par("zero_video") + Ascan.AscanBegin / 1000.0;
            //AscanChart.ChartAreas["Default"].AxisX.Maximum = Info.ZeroVideo + Ascan.AscanBegin / 1000.0 + Ascan.TimeEqu / 1000.0;
            AscanChart.ChartAreas["Default"].AxisX.Maximum = par("zero_video") + Ascan.AscanBegin / 1000.0 + par("scope_range");
                      
            // Draw Ascan plot
            if (Ascan.DataSize == 0) return;

            //double Step = Ascan.TimeEqu / (Ascan.DataSize - 1)  / 1000.0;
            double Step = par("scope_range") / (Ascan.DataSize - 1);
            for (int iPoint = 0; iPoint < Ascan.DataSize; iPoint++)
            {
                AscanChart.Series["AscanPlot"].Points.AddXY(par("zero_video") + Ascan.AscanBegin / 1000.0 + Step * iPoint, Ascan.Points[iPoint]);
            }

            // Draw Gate 1 plot
            AscanChart.Series["Gate1PosPlot"].Points.Clear();
            AscanChart.Series["Gate1NegPlot"].Points.Clear();
            AscanChart.Series["Gate1Pos2Plot"].Points.Clear();
            AscanChart.Series["Gate1Neg2Plot"].Points.Clear();
            //if ((Ascan.G1InAscan & Ascan.GateInAscan.GateStartInAscan) == Ascan.GateInAscan.GateStartInAscan ||
            //    (Ascan.G1InAscan & Ascan.GateInAscan.GateEndInAscan) == Ascan.GateInAscan.GateEndInAscan)
            {
                double Begin = par("zero_video") + Ascan.G1Begin / 1000.0;
                double End = par("zero_video") + Ascan.G1End / 1000.0;
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

            switch ((AscanInfo.PhaseType)par("gate1_phase"))
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (par("scope_video") >= (double)AscanInfo.VideoMode.PositiveWave);
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
                double Begin = par("zero_video") + Ascan.G2Begin / 1000.0;
                double End = par("zero_video") + Ascan.G2End / 1000.0;
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

            switch ((AscanInfo.PhaseType)par("gate2_phase"))
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (par("scope_video") >= (double)AscanInfo.VideoMode.PositiveWave);
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
                double Begin = par("zero_video") + Ascan.GIFBegin / 1000.0;
                double End = par("zero_video") + Ascan.GIFEnd / 1000.0;
                double Level = Ascan.GIFLevel;

                AscanChart.Series["GateIFPosPlot"].Points.AddXY(Begin, Level);
                AscanChart.Series["GateIFPosPlot"].Points.AddXY(End, Level);
                AscanChart.Series["GateIFNegPlot"].Points.AddXY(Begin, -1 * Level);
                AscanChart.Series["GateIFNegPlot"].Points.AddXY(End, -1 * Level);
            }

            switch ((AscanInfo.PhaseType)par("gateIF_phase"))
            {
                case AscanInfo.PhaseType.PositiveWave: // Positive wave
                    GatePositivePart = true;
                    GateNegativePart = false;
                    break;
                case AscanInfo.PhaseType.NegativeWave: // Negative wave
                    GatePositivePart = (par("scope_video") >= (double)AscanInfo.VideoMode.PositiveWave);
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
            Ascan ascan = new Ascan();
            if (Program.pcxus.readAscan(board, test, ref ascan, timeout))
            {
                // Update gate information
                gateIF.UpdateGate(Gate.GateNum.GateIF, ascan);
                gate1.UpdateGate(Gate.GateNum.Gate1, ascan);
                gate2.UpdateGate(Gate.GateNum.Gate2, ascan);

                UpdateAscan(ascan);
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
            AscanChart.SetBounds(0,58, ClientSize.Width-150, ClientSize.Height-58);
        }

        private void cbBoards_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex >= 0)
            {
                board = cb.SelectedIndex;
            }

        }

        private void cbTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedIndex >= 0) 
            {
                test = cb.SelectedIndex;
            }
        }

        private void textBoxTimeout_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                timeout = Convert.ToInt32(tb.Text);
            }
            catch (FormatException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}:,{1}: Error:{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                timeout = 100;
                tb.Text = "100";
            }
        }
    }
}
