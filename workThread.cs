using PROTOCOL;
using PCIE1730;
using System;
using System.Diagnostics;
using System.Threading;

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
                string s;
                curState = WrkStates.startWorkCycle;
                while (isRunning)
                {
                    //Проверяем сигналы ICC и  CYCLE3 - они должны быть выставлены воё время работы
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
                            s = string.Format("{0}: {1}: Ошибка: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, errStr);
                            log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, errStr);
                            isRunning = false;
                            frm.startStop();
                            break;
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
                                //Здесь подготовка модуля к работе
                                {
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
                            //Снялcя сигнал "ГОТОВНОСТЬ3" - труба начала движение
                            if (SL.getInst().iREADY.Val == false)
                            {
                                //Включить сбор данных с модуля контроля. Ожидать появления сигнала КОНТРОЛЬ. 
                                {
                                    //Запускаем поток чтения данных
                                }
                                waitControlStarted = sw.Elapsed;
                                curState = WrkStates.waitCntrl;
                            }
                            break;
                        //При появлении сигнала КОНТРОЛЬ начать анализировать сигнал СТРОБ. 
                        //Если сигнал КОНТРОЛЬ не появился за определенное время (10 секунд) – 
                        //аварийное завершение режима с выводом со-ответствующего сообщения.
                        case WrkStates.waitCntrl:
                            if ((sw.Elapsed - waitControlStarted).Seconds > 10)
                            {
                                errStr = "Не дождались трубы на входе в модуль";
                                curState = WrkStates.error;
                                break;
                            }
                            if(SL.getInst().iCNTR.Val==true)
                            {
                                curState = WrkStates.work;
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
                            break;
                        case WrkStates.endWork:
                            //По окончании сбора, обработки и передачи результата снять сигнал КОНТРОЛЬ. 
                            curState = WrkStates.startWorkCycle;
                            //По идее надо наверное выходить из цикла работы для следующей трубы уже запускать новый цикл
                            isRunning = false;
                            frm.startStop();
                            break;
                        default:
                            break;

                    }
                    //Thread.Sleep(100);
                }
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Вышли");
                //Если не установлено прерывание на просмотр и нет ошибки запускаем рабочий цикл по новой
                frm.setSb("Info", "Аварийное завершение.");
                if (!frm.bStopForView && curState!= WrkStates.error)
                    frm.startStop();
            }
            catch (Exception e)
            {
                curState = WrkStates.error;
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                frm.setSb("Info", string.Format("Аварийное завершение: {0}",e.Message));
                SL.getInst().ClearAllSignals();
                isRunning = false;
                if (!frm.bStopForView && curState != WrkStates.error)
                    frm.startStop();
            }
        }
        public void start()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (!isRunning)
            {
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
