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
using PCIE1730;
using EMUL;


namespace USPC
{
    public partial class FRMain : Form
    {
        public PCXUS pcxus = null;

        PCXUSNetworkServer server = null;

        ///Подчиненные формы
        /// <summary>
        /// Форма для отображения сигналов с 1730
        /// </summary>
        private FRSignals fSignals;
        /// <summary>
        /// Форма для отображения протокола
        /// </summary>
        private FRProt pr;

        /// <summary>
        ///рабочий поток
        /// </summary>
        private workThread wrkTh = null;

        /// <summary>
        ///Флаг работы основного рабочего потока
        /// </summary>
        bool isWorked = false;

        /// <summary>
        ///Время начала работы
        /// </summary>
         static DateTime startWorkTime;

        /// <summary>
        /// Счетчик труб
        /// </summary>
        int tubeCount = 0;

        /// <summary>
        /// Признак прерывания на просмотр
        /// </summary>
        public bool bStopForView { get; private set; }
        
        public FRMain()
        {
            Thread.CurrentThread.Name = "MainWindow";
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Normal;

            bStopForView = true;

            // Рабочий поток
            wrkTh = new workThread(this);
            timerUpdUI.Start();
        }

        private void FRMain_Load(object sender, EventArgs e)
        {
            //восстановление размеров главного окна        
            FormPosSaver.load(this);
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

            //Настраиваем окно сигналов
            // Окно создаем сразу оно будет существовать 
            // всё время работы программы
            if (!Program.cmdLineArgs.ContainsKey("NOSIGNALWINDOW"))
            {
                log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "fSignals form creation");
                fSignals = new FRSignals(SL.getInst())
                {
                    MdiParent = this,
                };
                fSignals.onHide += new FRSignals.OnHideForm(() => { miWindowsSignals.Checked = false; });
                fSignals.Visible = FormPosSaver.visible(fSignals);
                miWindowsSignals.Checked = fSignals.Visible;
            }
            else
                miWindowsSignals.Visible = false;



            string strNetServer = null;
            try
            {
                strNetServer = Program.cmdLineArgs["Server"];
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "command line param \"Server\" not specified");
            }
            if (strNetServer == null)
            {
                //создаём объект для платы
                pcxus = new PCXUS();
                //Запускаем сервер
                server = new PCXUSNetworkServer(pcxus);
                server.start();
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "server started");
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "server not started");
            }
            sb.Items["Info"].Text = "Для начала работы нажмите F5";
        }

        /// <summary>
        /// Запуск/остановка рабочего потока
        /// (В workThread вызывается из другого потока)
        /// </summary>
        public void startStop()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: IsWorked={0}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, isWorked.ToString());
            if (!isWorked)
            {
                startWorkTime = DateTime.UtcNow;
                //SL.getInst().oPEREKL.Val = true;
                Thread.Sleep(100);
                wrkTh.start();
            }
            else
            {
                //Если вышли из потока с ошибкой
                if (wrkTh.curState == workThread.WrkStates.error)
                {
                }
                else
                {
                    lblTubesCount.Text = string.Format("{0}", ++tubeCount);
                }
                wrkTh.stop();
                setSb("Info", "Нажмите F5 для начала работы");
            }
            isWorked = !isWorked;
            //Обрабатываем запуск из другого потока
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    miStart.Text = isWorked ? "Стоп" : "Старт";
                }));
            }
            else
            {
                miStart.Text = isWorked ? "Стоп" : "Старт";
            }
        }



        private void miShowProt_Click(object sender, EventArgs e)
        {
            miWindowsProt.Checked = !miWindowsProt.Checked;
            pr.Visible = miWindowsProt.Checked;
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            if (server != null) server.stop();
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
            pcxus.open(2);
        }

        private void miLoadUSPC_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = "us",
                Filter = "Файлы конфигурации (*.us)|*.us|Все файлы (*.*)|*.*"
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!pcxus.load(ofd.FileName))
                {
                    MessageBox.Show(string.Format("Ошибка загрузки конфигурации \"{0}\"",ofd.FileName));
                }
            }
        }
        private void miCloseUSPC_Click(object sender, EventArgs e)
        {
            pcxus.close();
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

        private void miTestAscanFromNet_Click(object sender, EventArgs e)
        {
            TestGetAscanFromNet frm = new TestGetAscanFromNet(this);
            frm.Show();
        }

        private void miWindowsSignals_Click(object sender, EventArgs e)
        {
            miWindowsSignals.Checked = !miWindowsSignals.Checked;
            fSignals.Visible = miWindowsSignals.Checked;
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            //btnSettings.Enabled = false;
            miSettings.Enabled = false;
            FRSettings frm = new FRSettings();
            frm.FormClosed += new FormClosedEventHandler((object _o, FormClosedEventArgs _e) => { /*btnSettings.Enabled = true;*/ miSettings.Enabled = true; });
            frm.MdiParent = this;
            frm.Show();
        }

        /// <summary>
        /// Обновление статусбара
        /// </summary>
        /// <param name="_sbItem">Имя пля в статусбаре</param>
        /// <param name="_sbText">Выводимая строка</param>
        /// Items: info,tubePos,dataSize,duration,heap
        public void setSb(string _sbItem, string _sbText)
        {
            this.sb.Items[_sbItem].Text = _sbText;
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            startStop();
        }

        private void эмуляцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            miEmul.Enabled = false;
            FREmul frm = new FREmul();
            frm.FormClosed += new FormClosedEventHandler((object ob, FormClosedEventArgs ea) => { miEmul.Enabled = true; });
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
