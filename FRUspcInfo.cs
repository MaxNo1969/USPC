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
    public partial class FRUspcInfo : Form
    {
        public FRUspcInfo(FRMain _frMain)
        {
            InitializeComponent();
            Owner = _frMain;
            string srv = Program.serverAddr;
            List<string> info = new List<string>();
            double board_serial_number = Program.pcxus.getParamValueDouble("board_serial_number");
            if (Program.pcxus.Err == (int)ErrorCode.PCXUS_NO_ERROR)
            {
                info.Add(string.Format("board_serial_number = {0}", board_serial_number));
            }
            else
            {
                info.Add(string.Format("Ошибка при чтении параметра \"board_serial_number\":{0}", (ErrorCode)Program.pcxus.Err));
            }
            double mux_rcpp_serial_number = Program.pcxus.getParamValueDouble("mux_rcpp_serial_number");
            if (Program.pcxus.Err == (int)ErrorCode.PCXUS_NO_ERROR)
            {
                info.Add(string.Format("mux_rcpp_serial_number = {0}", mux_rcpp_serial_number));
            }
            else
            {
                info.Add(string.Format("Ошибка при чтении параметра \"mux_rcpp_serial_number\":{0}", (ErrorCode)Program.pcxus.Err));
            }
            txtInfo.Lines = info.ToArray();
        }

        private void FRUspcInfo_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }

        private void FRUspcInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(this);
        }
    }
}
