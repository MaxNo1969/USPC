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
        TCPServer server = null;
        public FRTestTcp(FRMain _frMain)
        {
            InitializeComponent();
            Owner = _frMain;
            MdiParent = _frMain;
            AcceptButton = btnSend;
            resp = new List<string>();
            server = new TCPServer();
            server.pcxus = _frMain.pcxus;
            server.start();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string serverAddress = null;
            try
            {
                serverAddress = Program.cmdLineArgs["Server"];
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.warning, "FRTestTcp: btnTest_Click: Error: {0}",ex.Message);
                log.add(LogRecord.LogReason.warning, "Parameter \"Server\" not assigned. Use \"127.0.0.1\"");
                serverAddress = "127.0.0.1";
            }
            TcpCommand cmd = new TcpCommand(serverAddress);
            byte[] responce = cmd.sendCommand(edCommand.Text.Trim().ToUpper());
            edResponce.Text += string.Format("{0}", edCommand.Text.Trim().ToUpper());
            edCommand.Text = string.Empty;
            if (responce != null)
            {
                string s = Encoding.UTF8.GetString(responce);
                resp.Add(s);
                edResponce.Text += string.Format(" -> {0}",s);
            }
            else
            {
                log.add(LogRecord.LogReason.error, "FRTestTcp: btnTestClick: responce = null");
                edResponce.Text += string.Format("(<-)null");
            }
            edResponce.Text += System.Environment.NewLine;
        }

        private void FRTestTcp_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.stop();
        }
    }
}
