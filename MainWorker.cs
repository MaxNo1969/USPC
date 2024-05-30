using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Diagnostics;
using PCIE1730;
using Settings;
using System.Threading;

namespace USPC
{
    class MainWorker:BackgroundWorker
    {
        private FRMain frMain = null;
        UspcNetDataReader[] dataReaders = null;
        ZoneBackGroundWorker zoneAdder = null;
        bool speedCalced = false;
        public MainWorker(FRMain _frMain)
        {
            frMain = _frMain;

            dataReaders = new UspcNetDataReader[2];
            for (int i = 0; i < 2; i++)
                dataReaders[i] = new UspcNetDataReader(i);
            zoneAdder = new ZoneBackGroundWorker();
            speedCalced = false;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Cancelled);
            //Снимем все сигналы, кроме слаботочки
            clearAllOutSignals();
            frMain.setStartStopMenu(true);
            setSb("Info", "Для начала работы нажмите F5");
        }
        #region вывод информации в главное окно программы
        private void setPb(int _percent)
        {
            if (frMain.InvokeRequired)
            {
                Action action = () => frMain.setPb(_percent);
                frMain.Invoke(action);
            }
            else
                frMain.setPb(_percent);
        }

        private void setSb(string _item, string _text)
        {
            if (frMain.InvokeRequired)
            {
                Action action = () => frMain.setSb(_item, _text);
                frMain.Invoke(action);
            }
            else
                frMain.setSb(_item, _text);
        }

        private void setSb(SetSbParams _params)
        {
            if (frMain.InvokeRequired)
            {
                Action action = () => frMain.setSb(_params.item, _params.text);
                frMain.Invoke(action);
            }
            else
                frMain.setSb(_params.item, _params.text);
        }
        class SetSbParams
        {
            public string item;
            public string text;
            public SetSbParams(string _item, string _text)
            {
                item = _item;
                text = _text;
            }
        }
        #endregion вывод информации в главное окно программы
        enum ReportWhat
        {
            SetSB = 101
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.UserState.ToString());
            if(e.ProgressPercentage <= 100)
            {
                setPb(e.ProgressPercentage);
                return;
            }
            ReportWhat what = (ReportWhat)e.ProgressPercentage;
            switch(what)
            {
                case ReportWhat.SetSB:
                    setSb((SetSbParams)e.UserState);
                    break;
                default:
                    break;

                }
        }

        private static void clearAllOutSignals()
        {
            //Снимаем все выходные сигналв, кроме ПИТАНИЕ БМ
            //SL.getInst().oPBM.Val = false;
            SL.getInst().oWRK.Val = false;
            SL.getInst().oPEREKL.Val = false;
            SL.getInst().oRES1.Val = false;
            SL.getInst().oRES2.Val = false;
            //Резерв не трогаем
            //SL.getInst().oREZ.Val=false;
        }
        //Перечисление для текущуго состояния конечного автомата
        public enum WrkStates
        {
            none, //Не установлено
            startWorkCycle, //Начало рабочего цикла по трубе
            waitNextTube, //Ожидаем трубу на входе в установку
            moduleReady, //Установка готова к работе
            waitCntrl, //Ждем появления сигнала "Контроль"
            work, //Работа - крутим цикл приема данных до снятия сигнала "Контроль"
            endWork, //Работа закончена
            error //Приключилась ошибка
        }
        public WrkStates curState;
        private static WrkStates prevState = WrkStates.none;
        
        private static TimeSpan waitReadyStarted;
        private static TimeSpan waitControlStarted;
        private static TimeSpan controlIsSet;

        void stopWorkers()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            for (int i = 0; i < 2; i++)
            {
                if (dataReaders[i] != null && dataReaders[i].IsBusy)
                {
                    dataReaders[i].CancelAsync();
                }
            }
            if (zoneAdder != null && zoneAdder.IsBusy)
            {
                zoneAdder.CancelAsync();
            }
        }

        void startWorkers()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            for (int i = 0; i < 2; i++)
            {
                if (dataReaders[i] != null && !dataReaders[i].IsBusy)
                {
                    dataReaders[i].RunWorkerAsync();
                }
            }
            if (zoneAdder != null && !zoneAdder.IsBusy)
            {
                zoneAdder.RunWorkerAsync();
            }
        }

        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            try
            {
                //начинаем отсчет времени
                Stopwatch sw = new Stopwatch();
                sw.Start();
                //В эту строку запишем сообщение об ошибке
                string errStr = string.Empty;
                curState = WrkStates.startWorkCycle;
                controlIsSet = TimeSpan.Zero;
                while (true)
                {
                    //Проверяем сигналы ICC и  CYCLE - они должны быть выставлены воё время работы
                    if (!SL.getInst().checkSignals())
                    {
                        errStr = "Отсутствуют сигналы";
                        curState = WrkStates.error;
                    }
                    //Состояние изменилось
                    if (prevState != curState)
                    {
                        string msg = string.Format("{0} -> {1}", Enum.GetName(typeof(WrkStates), prevState), Enum.GetName(typeof(WrkStates), curState));
                        log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, msg);
                        setSb("Info", msg);
                        prevState = curState;
                    }
                    if (CancellationPending)
                    {
                        speedCalced = false;
                        stopWorkers();
                        e.Cancel = true;
                        return;
                    }
                    switch (curState)
                    {
                        //Проверяем наличие ошибки - если выставлено, то закрываем всё и выходим из цикла
                        case WrkStates.error:
                            //log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, errStr);
                            throw new Exception(errStr);
                        //isRunning = false;
                        //frm.setSb("Info", errStr);
                        //break;
                        //Начало рабочего цикла - снимаем сигнал "Перекладка" для сигнализации того, что
                        //установка ждет следующую трубу
                        case WrkStates.startWorkCycle:
                            SL.getInst().oPEREKL.Val = false;
                            curState = WrkStates.waitNextTube;
                            break;
                        case WrkStates.waitNextTube:
                            //Труба поступила на вход установки
                            if (SL.getInst().iREADY.Val == true)
                            {
                                clearAllOutSignals();

                                //Здесь подготовка модуля к работе
                                {
                                    //Установить сигнал "Питание БМ"
                                    SL.getInst().oPBM.Val = true;
                                    prepareBoardsForWork();
                                }
                                //Выставляем сигнал "РАБОТА"
                                SL.getInst().oWRK.Val = true;
                                waitReadyStarted = sw.Elapsed;
                                curState = WrkStates.moduleReady;
                            }
                            break;
                        case WrkStates.moduleReady:
                            //Тут надо проверить таймоут ожидания начала движения трубы
                            if ((sw.Elapsed - waitReadyStarted).Seconds > 30)
                            {
                                errStr = "Не дождались начала движения трубы";
                                curState = WrkStates.error;
                                break;
                            }
                            //Снялcя сигнал "ГОТОВНОСТЬ" - труба начала движение
                            if (SL.getInst().iREADY.Val == false)
                            {
                                waitControlStarted = sw.Elapsed;
                                curState = WrkStates.waitCntrl;
                            }
                            break;
                        //При появлении сигнала КОНТРОЛЬ запоминаем время для вычисления скорости
                        //Если сигнал КОНТРОЛЬ не появился за определенное время (10 секунд) – 
                        //аварийное завершение режима с выводом со-ответствующего сообщения.
                        case WrkStates.waitCntrl:
                            if ((sw.Elapsed - waitControlStarted).Seconds > 10)
                            {
                                errStr = "Не дождались трубы на входе в модуль";
                                curState = WrkStates.error;
                                break;
                            }
                            //
                            if(SL.getInst().iCNTR.Val==true)
                            {
                                controlIsSet = sw.Elapsed;   
                                curState = WrkStates.work;
                                //Включить сбор данных с модуля контроля. Ожидать пропадания сигнала КОНТРОЛЬ. 
                                startWorkers();
                            }
                            break;
                        case WrkStates.work:
                            //Пропал сигнал контроль
                            if (SL.getInst().iCNTR.Val == false)
                            {
                                //Останавливаем сбор
                                stopWorkers();
                                SL.getInst().oWRK.Val = false;
                                curState = WrkStates.endWork;
                                break;
                            }
                            //Труба доехала до базы
                            if (SL.getInst().iBASE.Val == true && controlIsSet != TimeSpan.Zero && !speedCalced)
                            {
                                TimeSpan timeToBase = sw.Elapsed - controlIsSet;
                                controlIsSet = TimeSpan.Zero;
                                //Получаем значение скорости трубы
                                //AppSettings.s.speed = (double)AppSettings.s.distanceToBase / (double)timeToBase.Milliseconds;
                                log.add(LogRecord.LogReason.info, "Рассчитаная скорость: {0}", (double)AppSettings.s.distanceToBase / ((double)timeToBase.Milliseconds*1000.0));
                                speedCalced = true;
                            }
                            break;
                        case WrkStates.endWork:
                            //По окончании сбора, обработки и передачи результата. 
                            SL.getInst().oRES1.Val = true;
                            SL.getInst().oRES2.Val = true;
                            SL.getInst().oPEREKL.Val = true;
                            Thread.Sleep(100);
                            speedCalced = false;
                            curState = WrkStates.startWorkCycle;
                            break;
                        default:
                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                speedCalced = false;
                stopWorkers();
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                e.Cancel = true;
                return;
            }
        }

        private void prepareBoardsForWork()
        {
            //throw new NotImplementedException();
        }

    }
}
