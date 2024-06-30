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
using PCI1730;
using EMUL;
using Settings;
using Data;
using CHART;
using System.Threading.Tasks;
using System.IO;

namespace USPC
{
    public partial class FRMain : Form
    {
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
        TubeWorker worker=null;

        /// <summary>
        ///Добавление новых зон
        /// </summary>
        //ZoneBackGroundWorker zoneAdder = null;

        
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
            fSignals = new FRSignals(Program.sl)
            {
                Owner = _fr,
                ShowInTaskbar = false,
            };
            fSignals.onHide += new FRSignals.OnHideForm(() => { miWindowsSignals.Checked = false; });
            fSignals.Visible = FormPosSaver.visible(fSignals);
            miWindowsSignals.Checked = fSignals.Visible;
        }
        #endregion Протокол и сигналы

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
            ThickView.ch1.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch2.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch3.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch4.ChartAreas["Default"].AxisY.Maximum = 12.0;
            
            setSb("Info", "Для начала работы нажмите F5");
        }

        public void ClearCharts()
        {
            UC4SensorView.ClearChart(CrossView.ch1);
            UC4SensorView.ClearChart(CrossView.ch2);
            UC4SensorView.ClearChart(CrossView.ch3);
            UC4SensorView.ClearChart(CrossView.ch4);

            UC4SensorView.ClearChart(LinearView.ch1);
            UC4SensorView.ClearChart(LinearView.ch2);
            UC4SensorView.ClearChart(LinearView.ch3);
            UC4SensorView.ClearChart(LinearView.ch4);

            UC4SensorView.ClearChart(ThickView.ch1);
            UC4SensorView.ClearChart(ThickView.ch2);
            UC4SensorView.ClearChart(ThickView.ch3);
            UC4SensorView.ClearChart(ThickView.ch4);
        }

        public void setStartStopMenu(bool _start)
        {
            miStart.Text = (_start) ? "Старт" : "Стоп";
            menu.Refresh();
            btnStart.Text = (_start) ? "СТАРТ" : "СТОП";
            tb.Refresh();
        }

        //BackgroundWorker testWorker = null;
        /// <summary>
        /// Запуск/остановка рабочего потока
        /// (В workThread вызывается из другого потока)
        /// </summary> 
        public void startStop()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, miStart.Text);
            if (miStart.Text == "Старт")
            {
                startWorkTime = DateTime.UtcNow;
                //Thread.Sleep(200);
                if (worker == null)
                {
                    worker = new TubeWorker();
                    worker.zbWorker.ProgressChanged += new ProgressChangedEventHandler(zbWorker_ProgressChanged);
                    //worker.zoneThread.zoneAdded += new OnZoneAdded(zoneAdded);
                }
                for (int board = 0; board < Program.numBoards; board++)
                    Program.data[board].Start();
                Program.result.Clear();
                ClearCharts();
                worker.RunWorkerAsync();
                setSb("Info", "Работа");
                setStartStopMenu(false);
            }
            else
            {
                //Приостановке снимаем сигнал "РАБОТА"
                Program.sl["РАБОТА"].Val = false;
                if (worker != null && worker.IsBusy)
                {
                    worker.CancelAsync();
                    worker = null;
                }
                setSb("Info", "Нажмите F5 для начала работы");
                setStartStopMenu(true);
            }
        }

        void zbWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            Action action = () => { Program.frMain.PutDataOnCharts(); Program.frMain.setPb(Program.result.zones * AppSettings.s.zoneSize * 100 / AppSettings.s.tubeLength); };
            Program.frMain.Invoke(action);
        }

        private void PutDataOnCharts()
        {
            double[] values01 = new double[USPCData.countZones];
            double[] values02 = new double[USPCData.countZones];
            double[] values03 = new double[USPCData.countZones];
            double[] values04 = new double[USPCData.countZones];
            double[] values05 = new double[USPCData.countZones];
            double[] values06 = new double[USPCData.countZones];
            double[] values07 = new double[USPCData.countZones];
            double[] values08 = new double[USPCData.countZones];
            double[] values09 = new double[USPCData.countZones];
            double[] values10 = new double[USPCData.countZones];
            double[] values11 = new double[USPCData.countZones];
            double[] values12 = new double[USPCData.countZones];
            for (int i = 0; i < USPCData.countZones; i++)
            {
                values01[i] = Program.result.zoneSensorResults[i][0];
                values02[i] = Program.result.zoneSensorResults[i][1];
                values03[i] = Program.result.zoneSensorResults[i][2];
                values04[i] = Program.result.zoneSensorResults[i][3];
                values05[i] = Program.result.zoneSensorResults[i][4];
                values06[i] = Program.result.zoneSensorResults[i][5];
                values07[i] = Program.result.zoneSensorResults[i][6];
                values08[i] = Program.result.zoneSensorResults[i][7];
                values09[i] = Program.result.zoneSensorResults[i][8];
                values10[i] = Program.result.zoneSensorResults[i][9];
                values11[i] = Program.result.zoneSensorResults[i][10];
                values12[i] = Program.result.zoneSensorResults[i][11];
            }
            UC4SensorView.PutDefDataOnChart(CrossView.ch1, values09);
            UC4SensorView.PutDefDataOnChart(CrossView.ch2, values10);
            UC4SensorView.PutDefDataOnChart(CrossView.ch3, values11);
            UC4SensorView.PutDefDataOnChart(CrossView.ch4, values12);
            UC4SensorView.PutDefDataOnChart(LinearView.ch1, values05);
            UC4SensorView.PutDefDataOnChart(LinearView.ch2, values06);
            UC4SensorView.PutDefDataOnChart(LinearView.ch3, values07);
            UC4SensorView.PutDefDataOnChart(LinearView.ch4, values08);
            UC4SensorView.PutThickDataOnChart(ThickView.ch1, values01);
            UC4SensorView.PutThickDataOnChart(ThickView.ch2, values02);
            UC4SensorView.PutThickDataOnChart(ThickView.ch3, values03);
            UC4SensorView.PutThickDataOnChart(ThickView.ch4, values04);
        }

        void testWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        void testWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            if (Program.boardState == BoardState.Opened)
            {
                if (MessageBox.Show("Плата уже открыта!Переоткрыть?\nВсе настройки будут сброшены.", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Program.pcxus.close();
                    Program.boardState = BoardState.NotOpened;
                    Program.pcxus.open(2);
                    Program.boardState = BoardState.Opened;
                }
                return;
            }
            else
            {
                if (!Program.pcxus.open(2))
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, (ErrorCode)Program.pcxus.Err);
                }
            }
        }

        private void miLoadUSPC_Click(object sender, EventArgs e)
        {
            if (Program.boardState != BoardState.Opened)
            {
                MessageBox.Show("Плата не открыта", "Внимание!", MessageBoxButtons.OK);
                return;
            }
            if (!Program.pcxus.load("default.us"))
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, (ErrorCode)Program.pcxus.Err);
                Program.boardState = BoardState.Error;
            }
        }
        private void miCloseUSPC_Click(object sender, EventArgs e)
        {

            if (Program.boardState != BoardState.Opened)
            {
                MessageBox.Show("Плата не открыта", "Внимание!", MessageBoxButtons.OK);
                return;
            }
            if (!Program.pcxus.close())
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, (ErrorCode)Program.pcxus.Err);
                Program.boardState = BoardState.Error;
            }
            else
            {
                Program.boardState = BoardState.NotOpened;
            }
        }

        private void miBoardInfo_Click(object sender, EventArgs e)
        {
            if (Program.boardState != BoardState.Opened)
            {
                MessageBox.Show("Плата не открыта", "Внимание!", MessageBoxButtons.OK);
                return;
            }
            FRUspcInfo frm = new FRUspcInfo(this);
            frm.Show();
        }

        private void timerUpdUI_Tick(object sender, EventArgs e)
        {
            long usedMem = GC.GetTotalMemory(false);
            sb.Items["heap"].Text = string.Format("{0,6}M", usedMem / (1024 * 1024));
            sb.Items["speed"].Text = string.Format("{0,7:F5}", AppSettings.s.speed);
            sb.Items["dataSize"].Text = Program.data[0].currentOffsetFrames.ToString();
            //NotOpened, Opened, loaded, error
            Color color;
            switch(Program.boardState)
            {
                case BoardState.NotOpened:
                    color = SystemColors.Control;
                    break;
                case BoardState.Opened:
                    color = Color.Green;
                    break;
                case BoardState.Error:
                    color = Color.Red;
                    break;
                default:
                    color = SystemColors.Control;
                    break;

            }
            if (Program.boardState == BoardState.NotOpened)
                color = SystemColors.Control;
            else
            {
                if (Program.pcxus.Err != (int)ErrorCode.PCXUS_NO_ERROR) color = Color.Red;
                else
                    color = Color.Green;
            }
            sb.Items["boardStateLabel"].BackColor = color;

        }

        private void tCPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRTestTcp frm = new FRTestTcp(this);
            frm.Show();
        }

        private void miTestAscanFromNet_Click(object sender, EventArgs e)
        {
            FRTestAscaNet frm = new FRTestAscaNet(this);
            frm.Show();
        }

        private void miWindowsSignals_Click(object sender, EventArgs e)
        {
            miWindowsSignals.Checked = !miWindowsSignals.Checked;
            fSignals.Visible = miWindowsSignals.Checked;
        }

        public void openSettings()
        {
            FRSettings frm = new FRSettings(this);
            frm.FormClosed += new FormClosedEventHandler((object _o, FormClosedEventArgs _e) => { /*btnSettings.Enabled = true;*/ miSettings.Enabled = true; });
            frm.Show();
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            FRSettings frm = new FRSettings(this);
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
            if(_percent<100)pb.Value = _percent;
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            startStop();
        }

        private void эмуляцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            miEmul.Enabled = false;
            FREmul frm = new FREmul(this);
            frm.FormClosed += new FormClosedEventHandler((object ob, FormClosedEventArgs ea) => { miEmul.Enabled = true; });
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
            //sfd.Filter = "Файлы данных (*.bintube)|*.bintube|Все файлы (*.*)|*.*";
            sfd.Filter = "Файлы CSV (*.csv)|*.scv|Все файлы (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //try
                //{
                //    BackgroundWorker w = new BackgroundWorker();
                //    w.WorkerReportsProgress = true;
                //    w.WorkerSupportsCancellation = true;
                //    w.DoWork += new DoWorkEventHandler(w_DoWork);
                //    w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
                //    w.ProgressChanged += new ProgressChangedEventHandler(w_ProgressChanged);
                //    pb.Visible = true;
                //    w.RunWorkerAsync(new WorkerArgs("Сохранение", sfd.FileName));
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                try
                {
                    sfd.FileName = "1.csv";
                    using (StreamWriter writer = new StreamWriter(sfd.FileName))
                    {
                        string s;
                        
                        for(int i = 0;i<Program.data[0].currentOffsetFrames;i++)
                        {
                            AcqAscan scan = Program.data[0].ascanBuffer[i];
                            uint tof = (scan.G1Tof & AcqAscan.TOF_MASK) * 5;
                            double thick = USPCData.TofToMm(tof);
                            s = string.Format("{0};{1};{2};{3}", scan.ScanCounter, scan.Channel, tof, thick);
                            writer.WriteLine(s);
                        }
                    }
                    sfd.FileName = "2.csv";
                    using (StreamWriter writer = new StreamWriter(sfd.FileName))
                    {
                        string s;

                        for (int i = 0; i < Program.data[1].currentOffsetFrames; i++)
                        {
                            AcqAscan scan = Program.data[1].ascanBuffer[i];
                            s = string.Format("{0};{1};{2}", scan.Channel, scan.ScanCounter, scan.G1Amp);
                            writer.WriteLine(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return;
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            startStop();
        }
    }
}
