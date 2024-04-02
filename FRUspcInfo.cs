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
        FRMain frMain;
        public FRUspcInfo(FRMain _frMain)
        {
            InitializeComponent();
            Owner = _frMain;
            MdiParent = _frMain;
            frMain = _frMain;
            PCXUS pcxus = frMain.pcxus;
            List<string> info = new List<string>();
            info.Add(string.Format("Установлено {0} плат USPC", pcxus.numBoards));
            info.Add(string.Format("Board S/N = {0}", pcxus.serialNumber()));
            //Не работает!!!
            //info.Add(string.Format("Board S/N (param) = {0}", (int)pcxus.getParamValueDouble("board_serial")));
            info.Add(string.Format("MUX S/N = {0}", pcxus.getParamValueDouble("mux_rcpp_serial_number")));
            info.Add(string.Format("HW Version = {0}", pcxus.getParamValueString("hardware_version")));
            info.Add(string.Format("DLL Version = {0}", pcxus.getParamValueString("dll_version")));
            info.Add(string.Format("Driver Version = {0}", pcxus.getParamValueString("driver_version")));
            info.Add(string.Format("Package Version = {0}", pcxus.getParamValueString("package_version")));
            info.Add(string.Format("Configuration file: {0}", pcxus.getParamValueString("current_file_name")));
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
