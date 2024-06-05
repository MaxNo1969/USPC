using PROTOCOL;
using PCI1730;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FPS;

namespace EMUL
{
    /// <summary>
    /// Форма для эмуляции установки. Включает WorkerThread для эмуляции движения трубы в установке
    /// </summary>
    public partial class FREmul : Form
    {
        /// <summary>
        /// Модель трубы  
        /// </summary>

        /// <summary>
        /// Поток для эмуляции движения трубы
        /// </summary>

        ///// <summary>
        ///// Поток для записи данных из виртуальной трубы в буфер эмулятора АЦП
        ///// </summary>
        //WriteDataThread wdt; 
        /// <summary>
        /// Сигналы для эмуляции PCIE1730
        /// </summary>
        SLLUZK sl;
        /// <summary>
        /// Worker - эмуляция движения трубы через установку
        /// </summary>
        BackgroundWorker worker;

        TimeSpan timeTubeMove = new TimeSpan(0,0,30);

        DateTime tubeStartTime;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_lcard">Карта АЦП(для эмулятора виртуальная)</param>
        /// <param name="_tube">Модель трубы</param>
        public FREmul()
        {
            InitializeComponent();
            // Настройка ProgressBar-а
            pbTube.Minimum = 0;
            pbTube.Maximum = 100;
            sl = SL.getInst();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            for (int i = 0; i < sl.CountIn; i++)
            {
                SignalIn s = sl.GetSignalIn(i);
                inputSignals.Items.Add(string.Format("{0} {1}", s.Position, s.Name), s.Val);
            }
            for (int i = 0; i < sl.CountOut; i++)
            {
                SignalOut s = sl.GetSignalOut(i);
                outputSignals.Items.Add(string.Format("{0} {1}", s.Position, s.Name), s.Val);
            }
            timer.Start();
            //Подготавливаем BackgroundWorker
            worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //Снимаем все сигналы 
            SL.getInst().ClearAllInputSignals();
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
                //lblPos.Text=string.Format("{0,5:f2} м",tm.l2px(tm.Position)/1000.0);
                //Проверяем открыта ли модель трубы
                if (btnTubeModel.Enabled == false)
                {
                    //frtubemodel.ucTube.winStart = tm.curPosX;
                    //frtubemodel.ucTube.Invalidate();
                }
            }
            //Добавляем в лист-бокс и обновляем метку
            else if(e.ProgressPercentage==101)
            {
                lblInfo.Text = e.UserState as string;
                lb.Items.Add(e.UserState as string);
            }
            //Просто обновляем метку
            else if (e.ProgressPercentage == 102)
            {
                lblInfo.Text = e.UserState as string;
            }
        }

        static long startWait;
        /// <summary>
        /// Задержка в цикле ожидания сигнала
        /// </summary>
        const int signalWaitCycleTime = 100;
        const int updateCountersPeriod = 1000;
        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            worker.ReportProgress(101, "Включаем сигнал \"Цепи управления\"");
            SL.getInst().set(SL.getInst().iCC, true);
            worker.ReportProgress(101, "Включаем сигнал \"Цикл\"");
            SL.getInst().set(SL.getInst().iCYC, true);
            //Тут сделаем цикл по трубам
            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Ждем выставления сигнала "Перекладка"
                worker.ReportProgress(101, "Ждем выставления сигнала \"ПЕРЕКЛАДКА\"...");
                while (SL.getInst().oPEREKL.Val == false)
                {
                    //Проверяем кнопку СТОП
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(signalWaitCycleTime);
                }
                worker.ReportProgress(101, "Выставляем сигнал \"ГОТОВНОСТЬ\"...");
                SL.getInst().set(SL.getInst().iREADY, true);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Ожидаем сигнал "Работа"
                worker.ReportProgress(101, "Ожидаем сигнал \"РАБОТА\"(модуль готов к работе)...");
                while (SL.getInst().oWRK.Val == false)
                {
                    //Проверяем кнопку СТОП
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(signalWaitCycleTime);
                }
                //Начинаем  движение трубы
                worker.ReportProgress(101, "Начинаем движение трубы...");
                //Снимаем сигнал "Готовность" (начинается движение трубы)
                worker.ReportProgress(101, "Снимаем сигнал \"ГОТОВНОСТЬ\"...");
                SL.getInst().set(SL.getInst().iREADY, false);
                tubeStartTime = DateTime.Now;
                //Ждем пока труба доедет до входа в модуль (~30 сек.)
                startWait = sw.ElapsedMilliseconds;
                while (true)
                {
                    long currentMilliseconds = sw.ElapsedMilliseconds - startWait;
                    if (currentMilliseconds > 3000) break;                        ;
                    //Проверяем кнопку СТОП
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(signalWaitCycleTime);
                }
                //Труба на входе в модуль
                worker.ReportProgress(101, "Труба на входе в модуль...");
                worker.ReportProgress(101, "Выставляем сигнал \"КОНТРОЛЬ\"(труба на входе)...");
                SL.getInst().set(SL.getInst().iCNTR, true);
                //Ждем пока труба проедет до конца
                startWait = sw.ElapsedMilliseconds;
                bool iBaseSet = false;
                while (true)
                {
                    long currentMilliseconds = sw.ElapsedMilliseconds - startWait;
                    if (currentMilliseconds > timeTubeMove.TotalMilliseconds) break;
                    //Проверяем кнопку СТОП
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (sw.ElapsedMilliseconds - startWait >= 3000 && !iBaseSet)
                    {
                        worker.ReportProgress(101, "Доехали до базы...");
                        SL.getInst().set(SL.getInst().iBASE, true);
                        iBaseSet = true;
                    }
                    worker.ReportProgress((int)(currentMilliseconds * 100 / timeTubeMove.TotalMilliseconds));
                    Thread.Sleep(signalWaitCycleTime);
                }
                //worker.ReportProgress(tm.Position * 100 / tm.Width, 0);
                worker.ReportProgress(101, "Труба вышла из ЛУЗК...");
                worker.ReportProgress(101, "Снимаем сигнал \"КОНТРОЛЬ3\"...");
                SL.getInst().set(SL.getInst().iCNTR, false);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Visible)
            {
                for (int i = 0; i < outputSignals.Items.Count; i++)
                {
                    SignalOut s = sl.GetSignalOut(i);
                    outputSignals.SetItemChecked(i, s.Val);
                }
                for (int i = 0; i < inputSignals.Items.Count; i++)
                {
                    SignalIn s = sl.GetSignalIn(i);
                    inputSignals.SetItemChecked(i, s.Val);
                }
            }
            if (worker.IsBusy)
                lblTimer.Text = string.Format(@"{0:hh\:mm\:ss}", DateTime.Now - tubeStartTime);
            else
                lblTimer.Text = "";
        }

        private void signals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox clb = (CheckedListBox)sender;
            if (clb.Name == "inputSignals")
            {
                SL.getInst().set(sl.GetSignalIn(e.Index), e.NewValue == CheckState.Checked);
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
    }
}
