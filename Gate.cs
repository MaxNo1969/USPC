using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Data;
using USPC.Data;

namespace USPC
{
    public partial class Gate : UserControl
    {
        public Gate()
        {
            InitializeComponent();
        }
        public enum GateNum
        {
            GateIF,
            Gate1,
            Gate2
        };

        public void UpdateGate(GateNum Gate, Ascan ascan)
        {
            double Amplitude = 0;
            double Distance = 0;
            Color AmpFillColor = new Color();
            Color AmpFillGradientColor = new Color();
            Color TOFMaxFillColor = new Color();
            Color TOFMaxFillGradientColor = new Color();
            Color TOFMinFillColor = new Color();
            Color TOFMinFillGradientColor = new Color();

            switch (Gate)
            {
                case GateNum.GateIF:
                    lbAmpValue.Visible = false;
                    lbAmp.Visible = false;
                    Amplitude = 0;
                    uint tof = (ascan.GIFTof & Ascan.TOF_MASK) * 5;
                    Distance = ThickConverter.TofToMm(tof);
                    if ((ascan.GIFFlags & Ascan.GateIFFlags.AmpAlarm) == Ascan.GateIFFlags.AmpAlarm)
                    {
                        AmpFillColor = Color.Red;
                        AmpFillGradientColor = Color.White;
                    }
                    else
                    {
                        AmpFillColor = Color.Black;
                        AmpFillGradientColor = Color.Gray;
                    }
                    if ((ascan.GIFFlags & Ascan.GateIFFlags.ThicknessAlarmMax) == Ascan.GateIFFlags.ThicknessAlarmMax)
                    {
                        TOFMaxFillColor = Color.Red;
                        TOFMaxFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMaxFillColor = Color.Black;
                        TOFMaxFillGradientColor = Color.Gray;
                    }
                    if ((ascan.GIFFlags & Ascan.GateIFFlags.ThicknessAlarmMin) == Ascan.GateIFFlags.ThicknessAlarmMin)
                    {
                        TOFMinFillColor = Color.Red;
                        TOFMinFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMinFillColor = Color.Black;
                        TOFMinFillGradientColor = Color.Gray;
                    }
                    break;
                case GateNum.Gate1:
                    lbAmpValue.Visible = true;
                    lbAmp.Visible = true;
                    Amplitude = Math.Min(100.0, ascan.G1Amp);
                    tof = (ascan.G1TofWt & Ascan.TOF_MASK)*5;
                    Distance = ThickConverter.TofToMm(tof);
                    if ((ascan.G1Flags & Ascan.GateFlags.CouplingAlarm) == Ascan.GateFlags.CouplingAlarm)
                    {
                        AmpFillColor = Color.Red;
                        AmpFillGradientColor = Color.White;
                    }
                    else
                    {
                        AmpFillColor = Color.Black;
                        AmpFillGradientColor = Color.Gray;
                    }
                    if ((ascan.G1Flags & Ascan.GateFlags.ThicknessAlarmMax) == Ascan.GateFlags.ThicknessAlarmMax)
                    {
                        TOFMaxFillColor = Color.Red;
                        TOFMaxFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMaxFillColor = Color.Black;
                        TOFMaxFillGradientColor = Color.Gray;
                    }
                    if ((ascan.G1Flags & Ascan.GateFlags.ThicknessAlarmMin) == Ascan.GateFlags.ThicknessAlarmMin)
                    {
                        TOFMinFillColor = Color.Red;
                        TOFMinFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMinFillColor = Color.Black;
                        TOFMinFillGradientColor = Color.Gray;
                    }
                    break;
                case GateNum.Gate2:
                    lbAmpValue.Visible = true;
                    lbAmp.Visible = true;
                    Amplitude = Math.Min(100.0, ascan.G2Amp);
                    tof = (ascan.G2TofWt & Ascan.TOF_MASK)*5;
                    Distance = ThickConverter.TofToMm(tof);
                    if ((ascan.G2Flags & Ascan.GateFlags.CouplingAlarm) == Ascan.GateFlags.CouplingAlarm)
                    {
                        AmpFillColor = Color.Red;
                        AmpFillGradientColor = Color.White;
                    }
                    else
                    {
                        AmpFillColor = Color.Black;
                        AmpFillGradientColor = Color.Gray;
                    }
                    if ((ascan.G2Flags & Ascan.GateFlags.ThicknessAlarmMax) == Ascan.GateFlags.ThicknessAlarmMax)
                    {
                        TOFMaxFillColor = Color.Red;
                        TOFMaxFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMaxFillColor = Color.Black;
                        TOFMaxFillGradientColor = Color.Gray;
                    }
                    if ((ascan.G2Flags & Ascan.GateFlags.ThicknessAlarmMin) == Ascan.GateFlags.ThicknessAlarmMin)
                    {
                        TOFMinFillColor = Color.Red;
                        TOFMinFillGradientColor = Color.White;
                    }
                    else
                    {
                        TOFMinFillColor = Color.Black;
                        TOFMinFillGradientColor = Color.Gray;
                    }
                    break;
            }
            lbAmpValue.Text = Amplitude.ToString();
            lbDistanceValue.Text = Distance.ToString();
        }
    }
}
