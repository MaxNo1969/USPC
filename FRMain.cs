﻿using System;
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
using Settings;
using Data;
using CHART;


namespace USPC
{
    public partial class FRMain : Form
    {
        //public PCXUS pcxus = null;
        //public BoardState boardState = BoardState.NotOpened;

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
        ///рабочий воркер
        /// </summary>
        MainWorker worker=null;

        /// <summary>
        ///Время начала работы
        /// </summary>
        static DateTime startWorkTime;

        /// <summary>
        /// Признак прерывания на просмотр
        /// </summary>
        public bool bStopForView { get; private set; }

        public FRMain()
        {
            Thread.CurrentThread.Name = "MainWindow";
            InitializeComponent();
            WindowState = FormWindowState.Normal;

            bStopForView = false;

            // Рабочий поток
            //worker = new MainWorker(this);

            timerUpdUI.Start();
        }

        #region Протокол и сигнвлы
        /// <summary>
        /// Настраиваем протокол
        /// Окно протокола создаем сразу оно будет существовать 
        /// всё время работы программы
        /// </summary>
        /// <param name="_fr">Окно - владелец</param>
        private void InitProtocolWindow(Form _fr)
        {
            pr = new FRProt(this)
            {
                saveMethod = FRProt.SaveMethod._tofile,
                Owner=_fr,
                ShowInTaskbar=false,
            };
            //Тут можно вставить обработчик закрытия формы
            pr.onHide += new FRProt.OnHideForm(() => { miWindowsProt.Checked = false; });
            pr.Visible = FormPosSaver.visible(pr);
            miWindowsProt.Checked = pr.Visible;
            FormPosSaver.load(pr);
        }
        /// <summary>
        /// Настраиваем окно сигналов
        /// Окно создаем сразу оно будет существовать 
        /// всё время работы программы
        /// </summary>
        /// <param name="_fr">Окно - владелец</param>
        private void InitSignalsWindow(Form _fr)
        {
            fSignals = new FRSignals(SL.getInst())
            {
                Owner = _fr,
                ShowInTaskbar = false,
            };
            fSignals.onHide += new FRSignals.OnHideForm(() => { miWindowsSignals.Checked = false; });
            fSignals.Visible = FormPosSaver.visible(fSignals);
            miWindowsSignals.Checked = fSignals.Visible;
        }
        #endregion Протокол и сигнвлы

        private void FRMain_Load(object sender, EventArgs e)
        {
            //восстановление размеров главного окна        
            FormPosSaver.load(this);
            //Настраиваем протокол
            InitProtocolWindow(this);
            //Настраиваем окно сигналов
            InitSignalsWindow(this);
            //Типоразмеры
            string[] typeSizeNames = Program.typeSize.allTypesizes();
            foreach (string s in typeSizeNames)
            {
                int ind = cbTypeSize.Items.Add(s);
                if (s == Program.typeSize.name) cbTypeSize.SelectedIndex = ind;
            }

            CrossView.lblName.Text = "Поперечный контроль";
            LinearView.lblName.Text = "Продольный контроль";
            ThickView.lblName.Text = "Котроль толщины";
            
            setSb("Info", "Для начала работы нажмите F5");
        }


        public void setStartStopMenu(bool _start)
        {
            miStart.Text = (_start) ? "Старт" : "Стоп";
        }

        BackgroundWorker testWorker = null;
        /// <summary>
        /// Запуск/остановка рабочего потока
        /// (В workThread вызывается из другого потока)
        /// </summary>
        public void startStop()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, miStart.Text);
            if (miStart.Text == "Старт")
            {
                startWorkTime = DateTime.UtcNow;
                SL.getInst().oPEREKL.Val = true;
                Thread.Sleep(100);
                testWorker = new BackgroundWorker()
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };
                testWorker.ProgressChanged += new ProgressChangedEventHandler(testWorker_ProgressChanged);
                testWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(testWorker_RunWorkerCompleted);
                testWorker.DoWork += new DoWorkEventHandler(testWorker_DoWork);
                Program.result.Clear();
                testWorker.RunWorkerAsync();
                setSb("Info", "Работа");
                setStartStopMenu(false);
            }
            else
            {

                if (testWorker != null && testWorker.IsBusy)
                {
                    testWorker.CancelAsync();
                    testWorker = null;
                }
                setSb("Info", "Нажмите F5 для начала работы");
                setStartStopMenu(true);
            }
        }

        void testWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    Program.result.AddNewZone();
                    worker.ReportProgress(0);
                    Thread.Sleep(1000);
                }
            }
        }

        private void PutDataOnCharts()
        {
            double[] values = Program.result.cross.values[0].ToArray();
            UC4SensorView.PutDataOnChart(CrossView.ch1, values);
            values = Program.result.cross.values[1].ToArray();
            UC4SensorView.PutDataOnChart(CrossView.ch2, values);
            values = Program.result.cross.values[2].ToArray();
            UC4SensorView.PutDataOnChart(CrossView.ch3, values);
            values = Program.result.cross.values[3].ToArray();
            UC4SensorView.PutDataOnChart(CrossView.ch4, values);

            values = Program.result.linear.values[0].ToArray();
            UC4SensorView.PutDataOnChart(LinearView.ch1, values);
            values = Program.result.linear.values[1].ToArray();
            UC4SensorView.PutDataOnChart(LinearView.ch2, values);
            values = Program.result.linear.values[2].ToArray();
            UC4SensorView.PutDataOnChart(LinearView.ch3, values);
            values = Program.result.linear.values[3].ToArray();
            UC4SensorView.PutDataOnChart(LinearView.ch4, values);


            values = Program.result.thick.values[0].ToArray();
            UC4SensorView.PutDataOnChart(ThickView.ch1, values);
            values = Program.result.thick.values[1].ToArray();
            UC4SensorView.PutDataOnChart(ThickView.ch2, values);
            values = Program.result.thick.values[2].ToArray();
            UC4SensorView.PutDataOnChart(ThickView.ch3, values);
            values = Program.result.thick.values[3].ToArray();
            UC4SensorView.PutDataOnChart(ThickView.ch4, values);
        }

        void testWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        void testWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            PutDataOnCharts();
        }



        private void miShowProt_Click(object sender, EventArgs e)
        {
            miWindowsProt.Checked = !miWindowsProt.Checked;
            pr.Visible = miWindowsProt.Checked;
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FRMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPosSaver.save(pr);
            FormPosSaver.save(this);
        }

        private void miOpenUSPC_Click(object sender, EventArgs e)
        {
            //pcxus.open(2);
            try
            {
                PCXUSNetworkClient client = new PCXUSNetworkClient(Program.serverAddr);
                Object obj = new object();
                int res = client.callNetworkFunction("open,2", out obj);
                if (res != 0)
                {
                    throw new Exception(string.Format("callNetworkFunction(open) return {0}", ((ErrorCode)res).ToString()));
                }
                else
                {
                    return;
                }
            }
            catch (Exception Ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, Ex.Message);
                return;
            }

        }

        private void miLoadUSPC_Click(object sender, EventArgs e)
        {
            try
            {
                PCXUSNetworkClient client = new PCXUSNetworkClient(Program.serverAddr);
                Object obj = new object();
                int res = client.callNetworkFunction("load,default.us", out obj);
                if (res != 0)
                {
                    throw new Exception(string.Format("callNetworkFunction(load) return {0:X8}", res));
                }
            }
            catch (Exception Ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, Ex.Message);
            }
        }
        private void miCloseUSPC_Click(object sender, EventArgs e)
        {
            try
            {
                PCXUSNetworkClient client = new PCXUSNetworkClient(Program.serverAddr);
                Object obj = new object();
                int res = client.callNetworkFunction("close", out obj);
                if (res != 0)
                {
                    throw new Exception(string.Format("callNetworkFunction(close) return {0:X8}", res));
                }
            }
            catch (Exception Ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, Ex.Message);
            }
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
            sb.Items["speed"].Text = string.Format("{0} mм/mс", AppSettings.s.speed);

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
            //miSettings.Enabled = false;
            FRSettings frm = new FRSettings();
            frm.FormClosed += new FormClosedEventHandler((object _o, FormClosedEventArgs _e) => { /*btnSettings.Enabled = true;*/ miSettings.Enabled = true; });
            frm.Show();
        }

        /// <summary>
        /// Обновление статусбара
        /// </summary>
        /// <param name="_sbItem">Имя поля в статусбаре</param>
        /// <param name="_sbText">Выводимая строка</param>
        /// Items: info,tubePos,dataSize,duration,heap
        public void setSb(string _sbItem, string _sbText)
        {
           sb.Items[_sbItem].Text = _sbText;
        }
        /// <summary>
        /// Обновление прогресбара
        /// </summary>
        /// <param name="_percent">Просент</param>
        public void setPb(int _percent)
        {
            pb.Value = _percent;
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

        private void miTestUSPCAscan_Click(object sender, EventArgs e)
        {
            FRTestAcqNet frm = new FRTestAcqNet(this);
            frm.Show();
        }

        private void testChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRResultView frm = new FRResultView(this);
            frm.Show();
        }
        #region Обработчики меню
        public struct WorkerArgs
        {
            public string action;
            public string fileName;
            public int zones;
            public int sensors;
            public int measCount;
            public int measSize;
            public WorkerArgs(string _action, string _fileName, int _zones = 0, int _sensors = 0, int _measCount = 0, int _measSize = 0)
            {
                action = _action;
                fileName = _fileName;
                zones = _zones;
                sensors = _sensors;
                measCount = _measCount;
                measSize = _measSize;
            }
        }
        //! @brief Загрузка данных из файла
        //! Данные->Загрузить
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы данных (*.bintube)|*.bintube|Все файлы (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BackgroundWorker w = new BackgroundWorker();
                    w.WorkerReportsProgress = true;
                    w.WorkerSupportsCancellation = true;
                    w.DoWork += new DoWorkEventHandler(w_DoWork);
                    w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
                    w.ProgressChanged += new ProgressChangedEventHandler(w_ProgressChanged);
                    pb.Visible = true;
                    w.RunWorkerAsync(new WorkerArgs("Загрузка", ofd.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        //! @brief Генерация данных
        //! Труба->Генерировать
        private void genToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundWorker w = new BackgroundWorker();
            w.WorkerReportsProgress = true;
            w.WorkerSupportsCancellation = true;
            w.DoWork += new DoWorkEventHandler(w_DoWork);
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
            w.ProgressChanged += new ProgressChangedEventHandler(w_ProgressChanged);
            pb.Visible = true;
            w.RunWorkerAsync(new WorkerArgs("Генерация", null, 50, 8, 500, 484));
        }
        //! @brief Сохранение трубы
        //! Данных->Сохранить  
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Файлы данных (*.bintube)|*.bintube|Все файлы (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BackgroundWorker w = new BackgroundWorker();
                    w.WorkerReportsProgress = true;
                    w.WorkerSupportsCancellation = true;
                    w.DoWork += new DoWorkEventHandler(w_DoWork);
                    w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
                    w.ProgressChanged += new ProgressChangedEventHandler(w_ProgressChanged);
                    pb.Visible = true;
                    w.RunWorkerAsync(new WorkerArgs("Сохранение", sfd.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion Обработчики меню

        #region Обработчики для BackgroundWorker-ов загрузки, сохранения, генерации
        void w_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            pb.Value = e.ProgressPercentage;
        }

        void w_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            pb.Visible = false;
        }

        void w_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            WorkerArgs args = (WorkerArgs)e.Argument;
            switch (args.action)
            {
                case "Загрузка":
                    //USPCData.load(args.fileName);
                    break;
                case "Сохранение":
                    //Program.data.save((Object)args.fileName);
                    break;
                case "Генерация":
                    DataGenerator.GenerateThicknessData(16,0,USPCData.countFrames,w);
                    break;
                case "Пересчет":
                    //stick.recalc(w, e);
                    break;
            }
        }
        #endregion

        private void cbTypeSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox cb = (ToolStripComboBox)sender;
            Program.typeSize.select((string)cb.Items[cb.SelectedIndex]);
        }
    }
}
