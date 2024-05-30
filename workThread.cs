using PROTOCOL;
using PCIE1730;
using System;
using System.Diagnostics;
using System.Threading;
using Settings;

namespace USPC
{
    /// <summary>
    /// Основной поток работы программы
    /// </summary>
    class workThread : IDisposable
    {
        public bool isRunning { get; private set; }
        private Thread thread;
        /// <summary>
        /// Блокировка
        /// </summary>
        object block = new object();
        FRMain frm;
        /// <summary>
        /// Конструктор
        /// </summary>
        public workThread(FRMain _frm)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            frm = _frm;
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
        private void threadFunc(object _params)
        {
            try
            {
                //начинаем отсчет времени
                Stopwatch sw = new Stopwatch();
                sw.Start();
                //Выставляем сигнал "Перекладка" - установка готова к следующему рабочему циклу
                //SL.getInst().oPEREKL.Val = true;
                //В эту строку запишем сообщение об ошибке
                string errStr = string.Empty;
                curState = WrkStates.startWorkCycle;
                controlIsSet = TimeSpan.Zero;
                while (isRunning)
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
                        frm.setSb("Info",msg);
                        prevState = curState;
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
                                //Если перекладка не снята - снимаем её
                                SL.getInst().oPEREKL.Val = false;
                                //Снимаем все выходные сигналв, кроме ПИТАНИЕ БМ
                                SL.getInst().oWRK.Val = false;
                                SL.getInst().oPEREKL.Val = false;
                                SL.getInst().oRES1.Val = false;
                                SL.getInst().oRES2.Val = false;
                                //Резерв не трогаем
                                //SL.getInst().oREZ.Val=false;

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
                                log.add(LogRecord.LogReason.debug,"controlIsSet = {0}",controlIsSet.Milliseconds);
                                curState = WrkStates.work;
                                //Включить сбор данных с модуля контроля. Ожидать пропадания сигнала КОНТРОЛЬ. 
                                {
                                    //Запускаем поток чтения данных
                                }
                            }
                            break;
                        case WrkStates.work:
                            //Пропал сигнал контроль
                            if (SL.getInst().iCNTR.Val == false)
                            {
                                SL.getInst().oWRK.Val = false;
                                curState = WrkStates.endWork;
                                break;
                            }
                            //Труба доехала до базы
                            if (SL.getInst().iBASE.Val == true && controlIsSet != TimeSpan.Zero)
                            {
                                TimeSpan timeToBase = sw.Elapsed - controlIsSet;
                                controlIsSet = TimeSpan.Zero;
                                log.add(LogRecord.LogReason.debug,"timeToBase = {0}",timeToBase.Milliseconds);
                                //Получаем значение скорости трубы
                                //AppSettings.s.speed = AppSettings.s.distanceToBase / timeToBase.Milliseconds;
                                //frm.setSb("speed",string.Format("{0} м/с", AppSettings.s.speed));
                            }
                            break;
                        case WrkStates.endWork:
                            //По окончании сбора, обработки и передачи результата. 
                            SL.getInst().oRES1.Val = true;
                            SL.getInst().oRES2.Val = true;
                            SL.getInst().oPEREKL.Val = true;
                            Thread.Sleep(100);
                            curState = WrkStates.startWorkCycle;
                            break;
                        default:
                            break;

                    }
                    //Thread.Sleep(100);
                }
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Вышли");
                if (frm.InvokeRequired)
                {
                    Action action = () => frm.setSb("Info", "Вышли из рабочего цикла.");
                    frm.Invoke(action);
                }
                else
                    frm.setSb("Info", "Вышли из рабочего цикла.");
            }
            catch (Exception e)
            {
                curState = WrkStates.error;
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                if (frm.InvokeRequired)
                {
                    Action action = () => frm.setSb("Info", string.Format("Аварийное завершение: {0}", e.Message));
                    frm.Invoke(action);
                }
                else
                    frm.setSb("Info", string.Format("Аварийное завершение: {0}", e.Message));
                SL.getInst().ClearAllSignals();
                isRunning = false;
                stop();
                frm.setStartStopMenu(true);
            }
        }

        private void prepareBoardsForWork()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,"Подготовка к работе");
        }
        public void start()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (!isRunning)
            {
                if (frm.InvokeRequired)
                {
                    Action action = () => frm.setSb("Info", "Готов к работе");
                    frm.Invoke(action);
                }
                else
                    frm.setSb("Info", "Готов к работе");
                isRunning = true;
                thread = new Thread(threadFunc)
                {
                    Name = "workThread",
                    IsBackground = true,
                };
                thread.Start();
            }
            else
            {
                return;
            }
        }
        public void stop()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (isRunning)
            {
                isRunning = false;
                thread.Join();
                thread = null;
            }
            else
            {
                return;
            }
        }
        
        void IDisposable.Dispose()
        {
            stop();
        }
    }
}
