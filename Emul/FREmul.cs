using PROTOCOL;
using PCI1730;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FPS;
using USPC;
using USPC.PCI_1730;
using Settings;

namespace EMUL
{
    /// <summary>
    /// Форма для эмуляции установки. Включает WorkerThread для эмуляции движения трубы в установке
    /// </summary>
    public partial class FREmul : Form
    {
        /// <summary>
        /// Сигналы для эмуляции PCIE1730
        /// </summary>
        DefSignals sl;
        /// <summary>
        /// Worker - эмуляция движения трубы через установку
        /// </summary>
        BackgroundWorker worker;

        System.Threading.Timer timerStrob;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_lcard">Карта АЦП(для эмулятора виртуальная)</param>
        /// <param name="_tube">Модель трубы</param>
        public FREmul(Form _fr)
        {
            InitializeComponent();
            Owner=_fr;
            ShowInTaskbar = false;

            // Настройка ProgressBar-а
            pbTube.Minimum = 0;
            pbTube.Maximum = 100;
            sl = Program.sl;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            for (int i = 0; i < sl.Count; i++)
            {
                Signal s = sl[i];
                if(s.input)
                    inputSignals.Items.Add(s, s.Val);
                else
                    outputSignals.Items.Add(s, s.Val);
            }
            //Подготавливаем BackgroundWorker
            worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            //worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.DoWork += new DoWorkEventHandler(TubeEmulWorker);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            timer.Start();
        }

        void timerZoneCallback(object _state)
        {
            if (worker == null || !worker.IsBusy) return;
            sl.set(sl["СТРОБ"], true);
            while(!sl.get(sl["СТРБРЕЗ"]))Application.DoEvents();
            worker.ReportProgress(101, string.Format("СТРОБ!"));
            sl.set(sl["СТРОБ"], false);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //Снимаем все сигналы 
            //sl.ClearAllOutputSignals();
            //sl.ClearAllInputSignals();
            pbTube.Value = 0;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                //lb.Items.Add(e.UserState as string);
            }
            else if (e.ProgressPercentage <= 100)
            {
                pbTube.Value=e.ProgressPercentage;
            }
            //Добавляем в лист-бокс и обновляем метку
            else if(e.ProgressPercentage==101)
            {
                lblInfo.Text = e.UserState as string;
                int currentItemNumber = lb.Items.Add(e.UserState as string);
                lb.SelectedIndex = currentItemNumber;
                
            }
            //Просто обновляем метку
            else if (e.ProgressPercentage == 102)
            {
                lblInfo.Text = e.UserState as string;
            }
        }

        /// <summary>
        /// Задержка в цикле ожидания сигнала
        /// </summary>
        const int signalWaitCycleTime = 100;
        const int updateCountersPeriod = 1000;
        const int WaitReadyTime = 2000;
        const int WaitWorkTime = 2000;
        int MoveTubeTime = 30000;

        DateTime TubeStartTime;
        DateTime dtStartWait;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Visible)
            {
                for(int i = 0;i<inputSignals.Items.Count;i++)
                {
                    Signal s = (Signal)inputSignals.Items[i];
                    inputSignals.SetItemChecked(i, s.Val);
                }
                for (int i = 0; i < outputSignals.Items.Count; i++)
                {
                    Signal s = (Signal)outputSignals.Items[i];
                    outputSignals.SetItemChecked(i, s.Val);
                }
            }
            if (worker.IsBusy)
                lblTimer.Text = string.Format(@"{0:hh\:mm\:ss}", DateTime.Now - TubeStartTime);
            else
                lblTimer.Text = "";
        }

        private void signals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox clb = (CheckedListBox)sender;
            if (clb.Name == "inputSignals")
            {
                Signal s = (Signal)clb.Items[e.Index];
                //s.Val = (e.NewValue == CheckState.Checked);
                Program.sl.set(s, (e.NewValue == CheckState.Checked));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lb.Items.Clear();
            worker.RunWorkerAsync();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            if(worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }

        private void FREmul_Load(object sender, EventArgs e)
        {
            FormPosSaver.load(this);
        }
        private void FREmul_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
            FormPosSaver.save(this);
        }
        //FRTubeModel frtubemodel;
        private void btnTubeModel_Click(object sender, EventArgs e)
        {
        }

        void TubeEmulWorker(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "started");
            int percent = 0;
            int timeToBase = (int)((double)AppSettings.s.distanceToBase / (double)AppSettings.s.speed);
            worker.ReportProgress(101, string.Format("distanceToBase = {0}, speed = {1}, timeToBase = {2}", AppSettings.s.distanceToBase, AppSettings.s.speed, timeToBase));
            MoveTubeTime = (int)((double)AppSettings.s.tubeLength / (double)AppSettings.s.speed);
            worker.ReportProgress(101, string.Format("MoveTubeTime = {0}", MoveTubeTime));
            int strobTime = (int)((double)AppSettings.s.zoneSize / (double)AppSettings.s.speed);
            worker.ReportProgress(101, string.Format("strobTime = {0}", strobTime));

            while (!worker.CancellationPending)
            {
                worker.ReportProgress(101, "Снимаем сигнал \"РЕЗУЛЬТАТ\"");
                sl["РЕЗУЛЬТАТ"].Val = false;
                worker.ReportProgress(101, "Включаем сигнал \"ЦИКЛ\"");
                dtStartWait = DateTime.Now;
                sl.set(sl["ЦИКЛ"], true);
                //Ожидаем сигнал "РАБОТА"
                worker.ReportProgress(101, "Ожидаем сигнал \"РАБОТА\"(модуль готов к работе)...");
                while (!sl.get("РАБОТА"))
                {
                    //Проверяем кнопку СТОП
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if ((DateTime.Now - dtStartWait).Milliseconds > WaitWorkTime)
                    {
                        worker.ReportProgress(101, "Не дождались сигнала \"РАБОТА\"...");
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(signalWaitCycleTime);
                }
                worker.ReportProgress(101, "Выставляем сигнал \"КОНТРОЛЬ\"...");
                sl.set(sl["КОНТРОЛЬ"], true);
                bool iBaseSet = false;
                TubeStartTime = DateTime.Now;
                timerStrob = new System.Threading.Timer(timerZoneCallback, null, 0, strobTime);
                while (!worker.CancellationPending)
                {
                    double msEplaced = (DateTime.Now - TubeStartTime).TotalMilliseconds;
                    percent = (int)(msEplaced * 100.0 / (double)MoveTubeTime);
                    if (percent > 100)
                    {
                        timerStrob.Dispose();
                        worker.ReportProgress(101, "Снимаем сигнал \"КОНТРОЛЬ\"");
                        sl.set(sl["КОНТРОЛЬ"], false);
                        while (!sl.get("РЕЗУЛЬТАТ")) Application.DoEvents();
                        worker.ReportProgress(101, "Получили результат...");
                        sl.set(sl["БАЗА"], false);
                        worker.ReportProgress(101, "Снимаем сигнал \"БАЗА\"...");
                        worker.ReportProgress(101, "Закончено...");
                        break;
                    }
                    else
                    {
                        worker.ReportProgress(percent);
                    }
                    if ((DateTime.Now - TubeStartTime).TotalMilliseconds >= timeToBase && !iBaseSet)
                    {
                        worker.ReportProgress(101, "Доехали до базы...");
                        sl.set(sl["БАЗА"], true);
                        iBaseSet = true;
                    }
                    Thread.Sleep(500);
                }
                worker.ReportProgress(101, "Снимаем сигнал \"ЦИКЛ\"");
                sl.set(sl["ЦИКЛ"], false);
            }
        }
    }
}
