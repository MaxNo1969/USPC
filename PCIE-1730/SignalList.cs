
using PROTOCOL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using USPC;

namespace PCIE1730
{
    /// <summary>
    /// Делегат при установке сигнала
    /// </summary>
    public delegate void onSet();
    /// <summary>
    /// Делегат при ожидании сигнала
    /// </summary>
    /// <param name="_value">Значение</param>
    /// <param name="_signal">Сигнал</param>
    /// <param name="_timeout">Таймаут</param>
    /// <returns></returns>
    public delegate string onWait(bool _value, Signal _signal, TimeSpan _timeout);
    /// <summary>
    /// Список сигналов 
    /// </summary>
    public class SignalList : IDisposable
    {
        private bool disposed = false;
        /// <summary>
        /// Произведена ли очистка
        /// </summary>
        /// <returns></returns>
        public bool Disposed()
        {
            return (disposed);
        }
        /// <summary>
        /// Очистка занятых ресурсов
        /// </summary>
        public void Dispose()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Отключаемся от PCIE1730");
            terminate = true;
            if(th.IsAlive)th.Join(100);
            a1730.Dispose();
            disposed = true;
        }

        private readonly ThreadStart ts;
        private Thread th;
        private volatile bool terminate = false;

        /// <summary>
        /// Плата цифрового ввода/вывода (PCIE-1730)
        /// </summary>
        protected PCIE_1730 a1730;

        private List<Signal> M = new List<Signal>();
        private List<Latch> L = new List<Latch>();
        private readonly object SignalsLock;
        /// <summary>
        /// Список входных сигналов
        /// </summary>
        protected List<SignalIn> MIn;
        /// <summary>
        /// Список выходных сигналов
        /// </summary>
        protected List<SignalOut> MOut;


        /// <summary>
        /// Установить сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <param name="_val">Значение</param>
        public void set(SignalIn _sg, bool _val)
        {
            a1730.SetBit(_sg.Position, _val,true);
        }

        /// <summary>
        /// Прочитать сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <returns>Значение</returns>
        public bool get(SignalIn _sg)
        {
            return a1730.GetBit(_sg.Position,true);
        }

        /// <summary>
        /// Установить сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <param name="_val">Значение</param>
        public void set(SignalOut _sg, bool _val)
        {
            a1730.SetBit(_sg.Position, _val, false);
        }

        /// <summary>
        /// Прочитать сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <returns>Значение</returns>
        public bool get(SignalOut _sg)
        {
            return a1730.GetBit(_sg.Position, false);
        }
        /// <summary>
        /// Записать текущие значения сигналов в плату PCIE1730
        /// </summary>
        protected void WriteSignals()
        {
            byte[] values_out = a1730.ReadOut();
            for (int i = 0; i < M.Count(); i++)
            {
                Signal p = M[i];
                if (p.input)
                    continue;
                if (p.verbal)
                {
                    bool v = a1730.GetBit(p.position,false);
                    if (p.val != v)
                    {
                        log.add(LogRecord.LogReason.info,"{0}: {1} : {2} выставляем в {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,p.name,p.val.ToString());
                    }
                }
                a1730.SetBit(p.position, p.val,false);
            }
            a1730.Write(values_out);
        }
        /// <summary>
        /// Ожидание сигнала
        /// </summary>
        /// <param name="_val">Значение</param>
        /// <param name="_signal">Сигнал</param>
        /// <param name="_timeout">Таймаут ожидания(мс)</param>
        /// <returns></returns>
        private string OnWait(bool _val, Signal _signal, TimeSpan _timeout)
        {
            Latch lp;
            lock (SignalsLock)
            {
                lp = new Latch(_val, _signal);
                L.Add(lp);
            }
            bool Signaled = lp.ev.WaitOne(_timeout);
            lp.ev.Reset();
            string ret;
            lock (SignalsLock)
            {
                if (Signaled)
                    ret = lp.reason;
                else
                    ret = "Не дождались";
                L.Remove(lp);
            }
            return (ret);
        }
        /// <summary>
        /// Конструктор (Напрямую нигде не вызывается)
        /// </summary>
        protected SignalList()
        {
            SignalsLock = new object();
            MIn = new List<SignalIn>();
            MOut = new List<SignalOut>();
            PCIE1730Settings st1730 = AppSettings.settings.pcie1730Settings;
            if (st1730 == null)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не удалось загрузить настройки PCIE-1730");
            }
            try
            {
                if (Program.cmdLineArgs.ContainsKey("NOA1730"))
                    a1730 = new PCIE_1730_virtual(st1730.ToString(), st1730.portInCnt, st1730.portOutCnt);
                else
                    a1730 = new PCIE_1730_real(st1730.ToString(), st1730.portInCnt, st1730.portOutCnt);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            //Читаем сигналы из AppSettings
            int cnt = st1730.sl.Count;
            log.add(LogRecord.LogReason.info,"{0}: {1}: Будем читать {2} сигналов.", GetType().Name,System.Reflection.MethodBase.GetCurrentMethod().Name, cnt);
            for (int i = 0; i < cnt; i++)
            {
                Signal sg = new Signal(st1730.sl[i].name, WriteSignals, OnWait, SignalsLock)
                {
                    sgSet = st1730.sl[i]
                };
                M.Add(sg);
                log.add(LogRecord.LogReason.info, "{0} {1}-{2}(Digital={3},EOn={4},EOff={5},Timeout={6},No_reset={7},Verbal={8})", sg.position, sg.name, sg.hint, sg.digital, sg.eOn, sg.eOn, sg.timeout, sg.no_reset, sg.verbal);
            }

            ts = new ThreadStart(Run);
            th = new Thread(ts)
            {
                Name = "SignalThread"
            };
        }
        /// <summary>
        /// Запуск потока обработки сигналов
        /// </summary>
        protected void Start()
        {
            th.Start();
        }
        private void ReadSignals()
        {
            if (disposed)
                return;
            byte[] vv = a1730.Read();
            DateTime dt = DateTime.Now;
            for (int i = 0; i < M.Count(); i++)
            {
                Signal p = M[i];
                if (!p.input)
                    continue;
                bool v = a1730.GetBit(p.position,true);
                lock (SignalsLock)
                {
                    p.front = p.val != v;
                    if (p.front)
                    {
                        p.val_prev = p.val;
                        p.val = v;
                        p.last_changed = dt;

                        if (p.verbal)
                            log.add(LogRecord.LogReason.info, "{0}: {1}: {2} = {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, p.name, v);
                    }
                }
            }
        }
        private void LatchSignals()
        {
            lock (SignalsLock)
            {
                for (int i = 0; i < L.Count(); i++)
                {
                    Latch lp = L[i];
                    if (!lp.terminated)
                    {
                        if (lp.signal.val == lp.val)
                        {
                            lp.terminated = true;
                            lp.reason = "Ok";
                        }
                        if (lp.terminated)
                            lp.ev.Set();
                    }
                }
            }
        }
        /// <summary>
        /// Виртуальная функция реакции
        /// </summary>
        protected virtual void Reaction()
        {
        }
        /// <summary>
        /// Виртуальная функция сигнал "Тревога"
        /// </summary>
        protected virtual void CheckAlarm()
        {
        }
        private void CheckAlarmL()
        {
            lock (SignalsLock)
            {
                for (int i = 0; i < M.Count(); i++)
                {
                    Signal p = M[i];
                    if (!p.input)
                        continue;
                    if (!p.IsAlarm)
                        continue;
                    if (p.AlarmVal != p.val)
                    {
                        string msg;
                        if (p.val)
                        {
                            if (p.eOn.Length != 0)
                                msg = p.eOn;
                            else
                                msg = "Авария: " + p.name + " true";
                        }
                        else
                        {
                            if (p.eOff.Length != 0)
                                msg = p.eOff;
                            else
                                msg = "Авария: " + p.name + " false";
                        }
                        LatchTerminate0(msg);
                    }
                }
            }
        }
        private void Run()
        {
            while (!terminate)
            {
                ReadSignals();
                LatchSignals();
                CheckAlarmL();
                lock (SignalsLock)
                {
                    Reaction();
                    CheckAlarm();
                }
            }
        }
        /// <summary>
        /// LatchTerminate0
        /// </summary>
        /// <param name="_msg"></param>
        protected void LatchTerminate0(string _msg)
        {
            for (int i = 0; i < L.Count(); i++)
            {
                Latch lp = L[i];
                if (!lp.terminated)
                {
                    lp.terminated = true;
                    lp.reason = _msg;
                    lp.ev.Set();
                }
            }
        }
        /// <summary>
        /// LatchTerminate0
        /// </summary>
        /// <param name="_msg"></param>
        public void LatchTerminate(string _msg)
        {
            lock (SignalsLock)
            {
                for (int i = 0; i < L.Count(); i++)
                {
                    Latch lp = L[i];
                    if (!lp.terminated)
                    {
                        lp.terminated = true;
                        lp.reason = _msg;
                        lp.ev.Set();
                    }
                }
            }
        }
        protected Signal Find(string _name, bool _input)
        {
            for (int i = 0; i < M.Count(); i++)
            {
                Signal p = M[i];
                if (p.name == _name)
                {
                    if (p.input != _input)
                        log.add(LogRecord.LogReason.warning,"{0}: {1}: Сигнал {2}  не {3}".GetType().Name,System.Reflection.MethodBase.GetCurrentMethod().Name,_name, _input ? "входящий" : "исходящий");
                    return (p);
                }
            }
            //MessageBox.Show("SignalList: Find: Сигнал " + _name + " не найден");
            throw new KeyNotFoundException(string.Format("SignalList: Find: Сигнал {0} не найден", _name));
            //return (null);
        }
        public int CountIn { get { return (MIn.Count); } }
        public int CountOut { get { return (MOut.Count); } }
        public SignalIn GetSignalIn(int _index)
        {
            return (MIn[_index]);
        }
        public SignalOut GetSignalOut(int _index)
        {
            return (MOut[_index]);
        }
    }
}
