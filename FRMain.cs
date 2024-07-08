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
using USPC.Workers;

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

        BackgroundWorker MainWorkWorker = new BackgroundWorker();

        public FRMain()
        {
            Thread.CurrentThread.Name = "MainWindow";
            InitializeComponent();
            WindowState = FormWindowState.Normal;

            MainWorkWorker.WorkerReportsProgress = true;
            MainWorkWorker.WorkerSupportsCancellation = true;
            MainWorkWorker.DoWork += new DoWorkEventHandler(MainWorkWorker_DoWork);
            MainWorkWorker.ProgressChanged += new ProgressChangedEventHandler(MainWorkWorker_ProgressChanged);
            MainWorkWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MainWorkWorker_RunWorkerCompleted);

            timerUpdUI.Start();
        }

        void MainWorkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if(worker != null)worker.CancelAsync();
        }

        void MainWorkWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        void MainWorkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "MainWorkWorker";
            try
            {
                while (!MainWorkWorker.CancellationPending)
                {
                    StartTubeWorker();
                    while (worker.IsBusy)
                    {
                        Thread.Sleep(1000);
                        Application.DoEvents();
                    }
                    Program.tubesCount++;
                    lblTubesCount.Text = Program.tubesCount.ToString();
                    Program.sl["РЕЗУЛЬТАТ"].Val = Program.result.GetTubeResult();
                    Program.sl["СТРБРЕЗ"].Val = true;
                    Thread.Sleep(500);
                    Program.sl["РЕЗУЛЬТАТ"].Val = false;
                    Program.sl["СТРБРЕЗ"].Val = false;
                    if (cbInterrupt.Checked)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                e.Cancel = true;
                return;
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                //worker.CancelAsync();
                MainWorkWorker.CancelAsync();
            }
        }

        #region Протокол и сигналы
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
                ShowInTaskbar=true,
            };
            //Тут можно вставить обработчик закрытия формы
            pr.onHide += new FRProt.OnHideForm(() => { miWindowsProt.Checked = false; });
            pr.Visible = FormPosSaver.visible(pr);
            miWindowsProt.Checked = pr.Visible;
            pr.FormClosing += new FormClosingEventHandler(protocolClosingEventHandler);
        }
        void protocolClosingEventHandler(object _o, FormClosingEventArgs _ea)
        {
            FormPosSaver.save(pr);
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
                ShowInTaskbar = true,
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

            cbInterrupt.Checked = AppSettings.s.bInterrupt;

            CrossView.lblName.Text = "Поперечный контроль";
            CrossView.ch1.Tag = 8;
            CrossView.ch2.Tag = 9;
            CrossView.ch3.Tag = 10;
            CrossView.ch4.Tag = 11;
            LinearView.lblName.Text = "Продольный контроль";
            LinearView.ch1.Tag = 4;
            LinearView.ch2.Tag = 5;
            LinearView.ch3.Tag = 6;
            LinearView.ch4.Tag = 7;
            ThickView.lblName.Text = "Котроль толщины";
            ThickView.ch1.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch2.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch3.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch4.ChartAreas["Default"].AxisY.Maximum = 12.0;
            ThickView.ch1.Tag = 0;
            ThickView.ch2.Tag = 1;
            ThickView.ch3.Tag = 2;
            ThickView.ch4.Tag = 3;
            
            setSb("Info", "Для начала работы нажмите F5");
        }

        public void setStartStopMenu(bool _start)
        {
            if (InvokeRequired)
            {
                miStart.Text = (_start) ? "Старт" : "Стоп";
                Action action = () => menu.Refresh();
                Invoke(action);
                btnStart.Text = (_start) ? "СТАРТ" : "СТОП";
                action = () =>tb.Refresh();
                Invoke(action);
            }
            else
            {
                miStart.Text = (_start) ? "Старт" : "Стоп";
                menu.Refresh();
                btnStart.Text = (_start) ? "СТАРТ" : "СТОП";
                tb.Refresh();
            }
        }

        public void StartTubeWorker()
        {
            if (worker == null)
            {
                worker = new TubeWorker();
                worker.zbWorker.ProgressChanged += new ProgressChangedEventHandler(zbWorker_ProgressChanged);
            }
            Program.result.Clear();
            Action action = () =>  ClearCharts();
            Program.frMain.Invoke(action);
            worker.RunWorkerAsync();
            setSb("Info", "Работа");
            setStartStopMenu(false);

        }

        /// <summary>
        /// Запуск/остановка рабочего потока
        /// (В workThread вызывается из другого потока)
        /// </summary> 
        public void startStop()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, miStart.Text);
            if (miStart.Text == "Старт")
            {
                //Program.prepareBoardsForWork(false);
                MainWorkWorker.RunWorkerAsync();
                setStartStopMenu(false);
            }
            else
            {
                //При остановке снимаем сигнал "РАБОТА"
                worker.zbWorker.CancelAsync();
                worker.CancelAsync();
                MainWorkWorker.CancelAsync();
                setSb("Info", "Нажмите F5 для начала работы");
                setStartStopMenu(true);
            }
        }

        public void zbWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            Action action = () => { Program.frMain.PutDataOnCharts(); Program.frMain.setPb(Program.result.zone * AppSettings.s.zoneSize * 100 / AppSettings.s.tubeLength); };
            Program.frMain.Invoke(action);
        }
        #region Вывод данных в основное окно
        private void PutDataOnCharts()
        {
            double[] values01 = new double[Program.countZones];
            double[] values02 = new double[Program.countZones];
            double[] values03 = new double[Program.countZones];
            double[] values04 = new double[Program.countZones];
            double[] values05 = new double[Program.countZones];
            double[] values06 = new double[Program.countZones];
            double[] values07 = new double[Program.countZones];
            double[] values08 = new double[Program.countZones];
            double[] values09 = new double[Program.countZones];
            double[] values10 = new double[Program.countZones];
            double[] values11 = new double[Program.countZones];
            double[] values12 = new double[Program.countZones];
            for (int i = 0; i < Program.countZones; i++)
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
        #endregion

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
            FormPosSaver.save(this);
        }

        private void miOpenUSPC_Click(object sender, EventArgs e)
        {
            if (Program.boardState == BoardState.Opened)
            {
                if (MessageBox.Show("Плата уже открыта!Переоткрыть?\nВсе настройки будут сброшены.", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) != DialogResult.Yes)return;
            }
            Program.prepareBoardsForWork(false);
        }

        private void miLoadUSPC_Click(object sender, EventArgs e)
        {
            if (Program.boardState != BoardState.Opened)
            {
                MessageBox.Show("Плата не открыта", "Внимание!", MessageBoxButtons.OK);
                return;
            }
            if (!Program.pcxus.load(Program.typeSize.currentTypeSize.configName))
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
            sb.Items["dataSize"].Text = Program.result.GetDataSize().ToString();
            if (Program.boardState == BoardState.Opened)
                sb.Items["boardStateLabel"].BackColor = Color.Green;
            else if(Program.boardState == BoardState.NotOpened)
                sb.Items["boardStateLabel"].BackColor = SystemColors.Control;
            else
                sb.Items["boardStateLabel"].BackColor = Color.Red;
        }

        private void tCPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRTestTcp frm = new FRTestTcp(this);
            frm.Show();
        }

        private void miTestAscanFromNet_Click(object sender, EventArgs e)
        {
            FRTestAscan frm = new FRTestAscan(this);
            frm.Show();
        }

        private void miWindowsSignals_Click(object sender, EventArgs e)
        {
            miWindowsSignals.Checked = !miWindowsSignals.Checked;
            fSignals.Visible = miWindowsSignals.Checked;
        }

        public void openSettings()
        {
            miSettings.Enabled = false;
            FRSettings frm = new FRSettings(this);
            frm.FormClosed += new FormClosedEventHandler((object _o, FormClosedEventArgs _e) => { /*btnSettings.Enabled = true;*/ miSettings.Enabled = true; });
            frm.Show();
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            openSettings();
        }

        /// <summary>
        /// Обновление статусбара
        /// </summary>
        /// <param name="_sbItem">Имя поля в статусбаре</param>
        /// <param name="_sbText">Выводимая строка</param>
        /// Items: info,tubePos,dataSize,duration,heap
        public void setSb(string _sbItem, string _sbText)
        {
            if (InvokeRequired)
            {
                Action action = () => sb.Items[_sbItem].Text = _sbText;
                Program.frMain.Invoke(action);
            }
            else
                sb.Items[_sbItem].Text = _sbText;  
        }
        /// <summary>
        /// Обновление прогресбара
        /// </summary>
        /// <param name="_percent">Просент</param>
        public void setPb(int _percent)
        {
            if (_percent < 100)
            {
                if (InvokeRequired)
                {
                    Action action = () => pb.Value = _percent;
                    Program.frMain.Invoke(action);
                }
                else
                {
                    pb.Value = _percent;
                }
            }
        }

        private void miStart_Click(object sender, EventArgs e)
        {
            startStop();
        }

        private void miTestUSPCAscan_Click(object sender, EventArgs e)
        {
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
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        FRWaitLongProcess frm = new FRWaitLongProcess(this);
                        frm.Show();
                        frm.setMes(string.Format("Загрузка файла {0}", ofd.FileName));
                        frm.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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
                    using (StreamWriter fs = new StreamWriter(sfd.FileName, false))
                    {
                        FRWaitLongProcess frm = new FRWaitLongProcess(this);                        
                        frm.Show();
                        Program.result.save(fs);
                        frm.setMes(string.Format("Сохранение файла {0}", sfd.FileName));
                        frm.Close();
                    }
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
            //pb.Visible = false;
        }

        void w_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            WorkerArgs args = (WorkerArgs)e.Argument;
            switch (args.action)
            {
                case "Загрузка":
                    break;
                case "Сохранение":
                    break;
                case "Генерация":
                    break;
                case "Пересчет":
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

        private void cbInterrupt_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            AppSettings.s.bInterrupt = cb.Checked;
            AppSettings.s.changed = true;
            Settings.Settings.save(AppSettings.s);
        }

        private void menuSaveScanData_ItemClick(object sender, EventArgs e)
        {
            try
            {
                string FileName;
                FileName = "1.csv";
                using (StreamWriter writer = new StreamWriter(FileName))
                {
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }
        }

        private void miEmulToolStripMenuItem_Click(object sender, EventArgs e)
        {
            miEmul.Enabled = false;
            FREmul frEmul = new FREmul(this);
            frEmul.FormClosed += new FormClosedEventHandler((object _o, FormClosedEventArgs _e) => { /*btnSettings.Enabled = true;*/ miEmul.Enabled = true; });
            frEmul.Show();
        }
    }
}
