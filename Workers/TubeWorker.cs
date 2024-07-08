﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Diagnostics;
using PCI1730;
using Settings;
using System.Threading;
using USPC.PCI_1730;
using System.Windows.Forms;
using USPC.Workers;
using Data;
using USPC.Data;

namespace USPC
{
    class TubeWorker:BackgroundWorker
    {
        public AscansReader ascansReader = null;
        public ZoneWorker zbWorker = null;
        DefSignals sl = Program.sl;
        bool speedCalced = false;
        Result result = Program.result;
        public TubeWorker()
        {
            //Натраиваем параметры воркера
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            //Воркер по добавлению зон
            zbWorker = new ZoneWorker();
            //zoneThread = new ZoneThread();
            speedCalced = false;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Cancelled);
            //stopWorkers();
            //Снимем все сигналы, кроме слаботочки
            //clearAllOutSignals();
            //Здесь можно обработать окончание работы с трубой - записать куда-нибудь результат и т.п.
            setSb("Info", "Для начала работы нажмите F5");
        }
        #region вывод информации в главное окно программы
        private void setPb(int _percent)
        {
            Action action = () => Program.frMain.setPb(_percent);
            Program.frMain.Invoke(action);
        }

        private void setSb(string _item, string _text)
        {
            Action action = () => Program.frMain.setSb(_item, _text);
            Program.frMain.Invoke(action);
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
        private void setSb(SetSbParams _params)
        {
            Action action = () => Program.frMain.setSb(_params.item, _params.text);
            Program.frMain.Invoke(action);
        }
        #endregion вывод информации в главное окно программы
        
        enum ReportWhat
        {
            SetSB = 101
        }
        
        //ToDo: Этоткусок можно перенести в FRMain
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.UserState.ToString());
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

        //Снимаем все выходные сигналв, кроме ПИТАНИЕ БМ
        private static void clearAllOutSignals()
        {
            Program.sl.ClearAllOutputSignals();
        }
        //Перечисление для текущуго состояния конечного автомата
        public enum WrkStates
        {
            none, //Не установлено
            startWorkCycle, //Начало рабочего цикла по трубе
            waitTube, //Ожидаем трубу на входе в установку
            work, //Работа - крутим цикл приема данных до снятия сигнала "Контроль"
            endWork, //Работа закончена
            error //Приключилась ошибка
        }
        public WrkStates curState;
        private static WrkStates prevState = WrkStates.none;

        //Время ожидания сигнала "РАБОТА"
        private const int iWrkTimeout = 300;
        //ToDo: Непонятно пока чего ждем
        //Время ожидания готовности чего-то
        private const int iCycleTimeout = 300;
        //Время ожидания сигнала "КОНТРОЛЬ"
        private const int iControlTimeout = 300;

        private static DateTime tubeStarted = DateTime.MinValue;
        private static DateTime waitCycleStarted;
        private static DateTime waitControlStarted;

        #region запуск/остановка сбора данных по всем платам
        void stopWorkers()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (ascansReader != null)
            {
                ascansReader.stop();
                ascansReader = null;
            }
            zbWorker.CancelAsync();
        }

        void startWorkers()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            zbWorker.RunWorkerAsync();
            ascansReader = new AscansReader(AppSettings.s.BoardReadTimeout);  
            ascansReader.start();
        }
        #endregion запуск/остановка сбора данных по всем платам

        bool controlCycle = false;
        bool boardsPrepared = false;

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "TubeWorker";
            try
            {
                //В эту строку запишем сообщение об ошибке
                string errStr = string.Empty;
                curState = WrkStates.startWorkCycle;
                waitCycleStarted = DateTime.Now;
                while (!CancellationPending)
                {
                    //Проверяем сигналы ICC и  CYCLE - они должны быть выставлены воё время работы
                    if (controlCycle && !Program.sl["ЦИКЛ"].Val)
                    {
                        errStr = "Пропал сигнал \"ЦИКЛ\"";
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
                        //stopWorkers();
                        e.Cancel = true;
                        return;
                    }
                    switch (curState)
                    {
                        //Проверяем наличие ошибки - если выставлено, то бросаем исключение
                        case WrkStates.error:
                            //log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, errStr);
                            throw new Exception(errStr);
                        //Начало рабочего цикла
                        case WrkStates.startWorkCycle:
                            {
                                //Здесь подготовка модуля к работе
                                {
                                    if (!boardsPrepared)
                                    {
                                        //Инициализируем платы и загружаем конфигурацию
                                        Program.prepareBoardsForWork(false);
                                        boardsPrepared = true;
                                    }
                                    //Program.result.addDeadZoneStart();
                                }
                                if (Program.sl["ЦИКЛ"].Val)
                                {
                                    //Выставляем сигнал "РАБОТА"
                                    Program.sl["РАБОТА"].Val = true;
                                    log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Начали контролировать \"ЦИКЛ\"");
                                    controlCycle = true;
                                    waitControlStarted = DateTime.Now;
                                    curState = WrkStates.waitTube;
                                    Program.result.addDeadZoneStart();
                                    break;
                                }
                                else
                                {
                                    if ((DateTime.Now - waitCycleStarted).TotalSeconds > iCycleTimeout)
                                    {
                                        errStr = "Не дождались сигнала \"ЦИКЛ\"";
                                        curState = WrkStates.error;
                                    }
                                    break;
                                }
                            }
                        case WrkStates.waitTube:
                            //Труба поступила на вход установки
                            if (Program.sl["КОНТРОЛЬ"].Val == true)
                            {
                                Program.sl["РЕЗУЛЬТАТ"].Val = false;
                                Program.sl["СТРБРЕЗ"].Val = false;
                                //Program.result.AddZone();
                                //Включить сбор данных с модуля контроля. Ожидать пропадания сигнала КОНТРОЛЬ. 
                                startWorkers();
                                tubeStarted = DateTime.Now;
                                curState = WrkStates.work;
                                break;
                            }
                            else
                            {
                                //Не дождались сигнала "КОНТРОЛЬ"
                                if ((DateTime.Now - waitControlStarted).TotalSeconds > iWrkTimeout)
                                {
                                    errStr = "Не дождались сигнала \"КОНТРОЛЬ\"";
                                    curState = WrkStates.error;
                                    break;
                                }
                                break;
                            }
                        case WrkStates.work:
                            //Пропал сигнал контроль
                            if (Program.sl["КОНТРОЛЬ"].Val == false)
                            {
                                curState = WrkStates.endWork;
                                break;
                            }
                            //Труба доехала до базы
                            if (Program.sl["БАЗА"].Val && tubeStarted != DateTime.MinValue && !speedCalced)
                            {
                                double millisecondsToBase = (DateTime.Now - tubeStarted).TotalMilliseconds;
                                tubeStarted = DateTime.MinValue;
                                //Получаем значение скорости трубы
                                AppSettings.s.speed = (double)AppSettings.s.distanceToBase / millisecondsToBase;
                                log.add(LogRecord.LogReason.info, "Рассчитаная скорость: {0}", AppSettings.s.speed);
                                setSb("speed", AppSettings.s.speed.ToString());
                                speedCalced = true;
                            }
                            break;
                        case WrkStates.endWork:
                            Program.result.addDeadZoneEnd();
                            //Перестаём контролировать цикл
                            controlCycle = false;
                            //По окончании сбора, обработки и передачи результата. 
                            Program.sl["РАБОТА"].Val = false;
                            //stopWorkers();
                            boardsPrepared = false;
                            speedCalced = false;
                            e.Cancel = true;
                            return;
                        default:
                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                //speedCalced = false;
                //sl["РАБОТА"].Val = false;
                //stopWorkers();
                //log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //controlCycle = false;
                //e.Cancel = true;
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }
            finally
            {
                speedCalced = false;
                sl["РАБОТА"].Val = false;
                stopWorkers();
                //log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Finally");
                controlCycle = false;
                e.Cancel = true;
            }
        }

        private void InitializeComponent()
        {

        }
    }
}
