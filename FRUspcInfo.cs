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
            string srv = frMain.strNetServer;
            List<string> info = new List<string>();
            if(frMain.boardState == BoardState.Opened)
            {
                int res;
                PCXUSNetworkClient client = new PCXUSNetworkClient(srv);
                Object obj = new object();
                res = client.callNetworkFunction("readdouble,board_serial_number", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("MUX S/N = {0}", (double)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"board_serial_number\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readdouble,mux_rcpp_serial_number", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("MUX S/N = {0}", (double)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"mux_rcpp_serial_number\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readstring,hardware_version", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("HW Version = {0}", (string)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"hardware_version\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readstring,dll_version", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("DLL Version = {0}", (string)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"dll_version\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readstring,driver_version", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("Driver Version = {0}", (string)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"driver_version\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readstring,package_version", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("Package Version = {0}", (string)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"package_version\":{0:X8}", res));
                }

                res = client.callNetworkFunction("readstring,current_file_name", out obj);
                if (res == 0)
                {
                    info.Add(string.Format("Configuration file: {0}", (string)obj));
                }
                else
                {
                    info.Add(string.Format("Ошибка при чтении параметра \"current_file_name\":{0:X8}", res));
                }
            }
            else
            {
                info.Add("Плата USPC не проинициализирована!");
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
