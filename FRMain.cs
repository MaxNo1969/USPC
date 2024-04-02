using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;
using System.Threading;
using FPS;
using System.Diagnostics;


namespace USPC
{
    public partial class FRMain : Form
    {
        public PCXUS pcxus = null;
        
        //! Форма для отображения протокола
        private FRProt pr;
        
        public FRMain()
        {
            Thread.CurrentThread.Name = "MainWindow";
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Normal;

            pcxus = new PCXUS();
        }

        private void FRMain_Load(object sender, EventArgs e)
        {
            //восстановление размеров главного окна        
            FormPosSaver.load(this);
            // Окно протокола создаем сразу оно будет существовать 
            // всё время работы программы
            //Настраиваем протокол
            // Окно протокола создаем сразу оно будет существовать 
            // всё время работы программы
            pr = new FRProt(this)
            {
                MdiParent = this,
                Dock = DockStyle.Bottom,
                saveMethod = FRProt.SaveMethod._tofile,
            };
            //Тут можно вставить обработчик закрытия формы
            pr.onHide += new FRProt.OnHideForm(() => { miWindowsProt.Checked = false; });
            pr.Visible = FormPosSaver.visible(pr);
            miWindowsProt.Checked = pr.Visible;
            FormPosSaver.load(pr);

            miOpenUSPC.Enabled = true;
            miLoadUSPC.Enabled = false;
            //miBoardInfo.Enabled = false;
        }

        private void miShowProt_Click(object sender, EventArgs e)
        {
            miWindowsProt.Checked = !miWindowsProt.Checked;
            pr.Visible = miWindowsProt.Checked;
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            if (pcxus != null) pcxus.close();
            Close();
        }

        private void FRMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(pr);
            FormPosSaver.save(this);
        }

        private void miOpenUSPC_Click(object sender, EventArgs e)
        {
            if (!pcxus.open(2))
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не удалось открыть плату USPC");
                return;
            }
            miLoadUSPC.Enabled = true;
            miBoardInfo.Enabled = true;
        }

        private void miLoadUSPC_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Файлы конфигураций (*.us)|*.us|All files (*.*)|*.*";
                ofd.FilterIndex = 0;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    if (!pcxus.load(fileName)) return;
                }
            }
        }

        Dictionary<string, List<int>> data = null;
        
        private void miLoadFile_Click(object sender, EventArgs e)
        {
            data = CSVHelper.readCsv();
        }

        private void miSaveFile_Click(object sender, EventArgs e)
        {
            if (data != null) CSVHelper.writeCsv(data);
        }

        private void miBoardTest_Click(object sender, EventArgs e)
        {
            FRTestAcq frm = new FRTestAcq(this);
            frm.Show();
        }

        private void miBoardInfo_Click(object sender, EventArgs e)
        {
            FRUspcInfo frm = new FRUspcInfo(this);
            frm.Show();
        }

        private void timerUpdUI_Tick(object sender, EventArgs e)
        {
            long usedMem = GC.GetTotalMemory(false);
            sb.Items["heap"].Text = string.Format("{0,6}M", usedMem / (1024 * 1024));
        }

        private void miTestUSPCAscan_Click(object sender, EventArgs e)
        {
            TestUSPCGetAscan frm = new TestUSPCGetAscan(this);
            frm.Show();
        }

        private void tCPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRTestTcp frm = new FRTestTcp(this);
            frm.Show();
        }
    }
}
