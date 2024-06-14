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

namespace USPC
{
    class TubeWorker:BackgroundWorker
    {
        UspcNetDataReader[] dataReaders = null;
        DefSignals sl = Program.sl;
        bool speedCalced = false;
        public TubeWorker()
        {
            //Натраиваем параметры воркера
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            //Воркеры сбора данных для каждой платы
            dataReaders = new UspcNetDataReader[2];
            for (int i = 0; i < 2; i++)
                dataReaders[i] = new UspcNetDataReader(i);
            speedCalced = false;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Cancelled);
            //Снимем все сигналы, кроме слаботочки
            clearAllOutSignals();
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
            moduleReady, //Установка готова к работе
            waitCntrl, //Ждем появления сигнала "Контроль"
            work, //Работа - крутим цикл приема данных до снятия сигнала "Контроль"
            endWork, //Работа закончена
            error //Приключилась ошибка
        }
        public WrkStates curState;
        private static WrkStates prevState = WrkStates.none;

        //Время ожидания сигнала "РАБОТА"
        private static int iWrkTimeout = 30;
        //ToDo: Непонятно пока чего ждем
        //Время ожидания готовности чего-то
        private static int iReadyTimeout = 30;
        //Время ожидания сигнала "КОНТРОЛЬ"
        private static int iControlTimeout = 30;

        private static DateTime tubeStarted;
        private static DateTime waitReadyStarted;
        private static DateTime waitControlStarted;
        private static DateTime controlIsSet = DateTime.MinValue;

        #region запуск/остановка сбора данных по всем платам
        void stopWorkers()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            for (int i = 0; i < 2; i++)
            {
                if (dataReaders[i] != null && dataReaders[i].IsBusy)
                {
                    dataReaders[i].CancelAsync();
                }
            }
        }

        void startWorkers()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            for (int i = 0; i < 2; i++)
            {
                if (dataReaders[i] != null && !dataReaders[i].IsBusy)
                {
                    dataReaders[i].RunWorkerAsync();
                }
            }
        }
        #endregion запуск/остановка сбора данных по всем платам


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            try
            {
                //В эту строку запишем сообщение об ошибке
                string errStr = string.Empty;
                curState = WrkStates.startWorkCycle;

                while (true)
                {
                    //Проверяем сигналы ICC и  CYCLE - они должны быть выставлены воё время работы
                    //Вроде не надо - будет исключение
                    /*
                    if (!SL.getInst().checkSignals())
                    {
                        errStr = "Отсутствуют сигналы";
                        curState = WrkStates.error;
                    }
                    */ 
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
                        //Проверяем наличие ошибки - если выставлено, то бросаем исключение
                        case WrkStates.error:
                            //log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, errStr);
                            throw new Exception(errStr);
                        //Начало рабочего цикла
                        case WrkStates.startWorkCycle:
                            curState = WrkStates.waitTube;
                            tubeStarted = DateTime.Now;
                            sl.set("РАБОТА", true);
                            break;
                        case WrkStates.waitTube:
                            //Труба поступила на вход установки
                            if (Program.sl["КОНТРОЛЬ"].Val == true)
                            {
                                clearAllOutSignals();

                                //Здесь подготовка модуля к работе
                                {
                                    //Установить сигнал "Питание БМ"
                                    //SL.getInst().oPBM.Val = true;
                                    prepareBoardsForWork();
                                }
                                //Выставляем сигнал "ГОТОВНОСТЬ"
                                //SL.getInst().oWORK.Val = true;
                                
                                waitReadyStarted = DateTime.Now;
                                curState = WrkStates.moduleReady;
                                break;
                            }
                            else
                            {
                                //Не дождались сигнала "РАБОТА"
                                if((DateTime.Now - tubeStarted ).Seconds > iWrkTimeout)
                                {
                                    errStr = "Не дождались сигнала \"РАБОТА\"";
                                    curState = WrkStates.error;
                                    break;
                                }
                                break;
                            }
                        case WrkStates.moduleReady:
                            //Тут надо проверить таймоут ожидания начала движения трубы
                            if ((waitReadyStarted - DateTime.Now).Seconds > iReadyTimeout)
                            {
                                errStr = "Не дождались начала движения трубы";
                                curState = WrkStates.error;
                                break;
                            }
                            else
                            {
                                curState = WrkStates.waitCntrl;
                                waitControlStarted = DateTime.Now;
                                break;
                            }
                        //При появлении сигнала КОНТРОЛЬ запоминаем время для вычисления скорости
                        //Если сигнал КОНТРОЛЬ не появился за определенное время (10 секунд) – 
                        //аварийное завершение режима с выводом со-ответствующего сообщения.
                        case WrkStates.waitCntrl:
                            if ((DateTime.Now - waitControlStarted).Seconds > iControlTimeout)
                            {
                                errStr = "Не дождались трубы на входе в модуль";
                                curState = WrkStates.error;
                                break;
                            }
                            else
                            {
                                //if (SL.getInst().iCNTR.Val == true)
                                //{
                                    controlIsSet = DateTime.Now;
                                    curState = WrkStates.work;
                                    //Включить сбор данных с модуля контроля. Ожидать пропадания сигнала КОНТРОЛЬ. 
                                    startWorkers();
                                //}
                                break;
                            }
                        case WrkStates.work:
                            //Пропал сигнал контроль
                            /*
                            if (SL.getInst().iCNTR.Val == false)
                            {
                                //Останавливаем сбор
                                stopWorkers();
                                SL.getInst().oWRK.Val = false;
                                curState = WrkStates.endWork;
                                break;
                            }
                            */ 
                            //Труба доехала до базы
                            if (Program.sl["БАЗА"].Val == true && controlIsSet != DateTime.MinValue && !speedCalced)
                            {
                                int millisecondsToBase = (DateTime.Now - controlIsSet).Milliseconds;
                                controlIsSet = DateTime.MinValue;
                                //Получаем значение скорости трубы
                                AppSettings.s.speed = (double)AppSettings.s.distanceToBase / ((double)millisecondsToBase*1000.0);
                                log.add(LogRecord.LogReason.info, "Рассчитаная скорость: {0}", AppSettings.s.speed);
                                setSb("speed", AppSettings.s.speed.ToString());
                                speedCalced = true;
                            }
                            break;
                        case WrkStates.endWork:
                            //По окончании сбора, обработки и передачи результата. 
                            Program.sl["РАБОТА"].Val = false;
                            Program.sl["РРЕЗУЛЬТАТ"].Val = false;
                            stopWorkers();
                            Thread.Sleep(100);
                            speedCalced = false;
                            return;
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
