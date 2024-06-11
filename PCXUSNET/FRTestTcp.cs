using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;

namespace USPC
{
    public partial class FRTestTcp : Form
    {
        List<string> resp = null;
        public FRTestTcp(FRMain _frMain)
        {
            InitializeComponent();
            Owner = _frMain;
            AcceptButton = btnSend;
            resp = new List<string>();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string serverAddress = Program.serverAddr;
            PCXUSNetworkClient client = new PCXUSNetworkClient(serverAddress);
            Object retval = new Object();
            int res = client.callNetworkFunction(edCommand.Text,out retval);
            string[] cmdAndArgs = edCommand.Text.Split(new char[] {','});
            double doubleVal = 0;
            string stringVal = "";
            if (cmdAndArgs[0] == "readdouble")
            {
                doubleVal = (double)retval;
                edResponce.Text += string.Format("{0} : {1}: val = {2}", edCommand.Text, res,doubleVal);
            }
            else if (cmdAndArgs[0] == "readstring")
            {
                stringVal = (string)retval;
                edResponce.Text += string.Format("{0} : {1}: val = {2}", edCommand.Text, res, stringVal);
            }
            else
            {

                edResponce.Text += string.Format("{0} : {1}", edCommand.Text, res);
            }
            edResponce.Text += System.Environment.NewLine;
            edCommand.Text = string.Empty;

        }
    }
}
