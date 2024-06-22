
using PROTOCOL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using USPC;
using Settings;

namespace PCI1730
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
        protected PCI_1730 a1730;


        /// <summary>
        /// Список сигналов
        /// </summary>
        protected List<Signal> M = new List<Signal>();


        /// <summary>
        /// Получаем сигнал по имени
        /// </summary>
        public Signal this[string _name]
        {
            get
            {
                try
                {
                    return M.Find(x => x.name == _name);
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    return null;
                }
            }
        }

        public Signal this[int _index]
        {
            get
            {
                if (_index < M.Count)
                    return M[_index];
                else
                    return null;
            }
        }

        
        private List<Latch> L = new List<Latch>();
        protected readonly object SignalsLock;

        /// <summary>
        /// Установить сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <param name="_val">Значение</param>
        public void set(Signal _sg, bool _val)
        {
            if(_sg.input)
                a1730.SetBit(_sg.position, _val,true);
            else
                a1730.SetBit(_sg.position, _val, false);
        }

        /// <summary>
        /// Прочитать сигнал
        /// </summary>
        /// <param name="_sg">Сигнал</param>
        /// <returns>Значение</returns>
        public bool get(Signal _sg)
        {
            return a1730.GetBit(_sg.position,true);
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
                    if (p.Val != v)
                    {
                        log.add(LogRecord.LogReason.info,"{0}: {1} : {2} выставляем в {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,p.name,p.Val.ToString());
                    }
                }
                a1730.SetBit(p.position, p.Val,false);
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
        protected string OnWait(bool _val, Signal _signal, TimeSpan _timeout)
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

        
        public int Count{get{return M.Count;} }
        /// <summary>
        /// Конструктор (Напрямую нигде не вызывается)
        /// </summary>
        protected SignalList()
        {
            SignalsLock = new object();
            PCIE1730Settings st1730 = AppSettings.settings.pcie1730Settings;
            if (st1730 == null)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не удалось загрузить настройки PCIE-1730");
            }
            if (AppSettings.s.pcie1730Settings.useEmul)
                a1730 = new PCI_1730_virtual(st1730.ToString(), st1730.portInCnt, st1730.portOutCnt);
            else
            {
                try
                {
                    a1730 = new PCI_1730_real(st1730.ToString(), st1730.portInCnt, st1730.portOutCnt);
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Используем эмулятор.");
                    AppSettings.s.pcie1730Settings.useEmul = true;
                    a1730 = new PCI_1730_virtual(st1730.ToString(), st1730.portInCnt, st1730.portOutCnt);
                }
            }
            ts = new ThreadStart(Run);
            th = new Thread(ts)
            {
                Name = "SignalThread"
            };
            Start();
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
                    p.front = p.Val != v;
                    if (p.front)
                    {
                        p.val_prev = p.Val;
                        p.Val = v;
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
                        if (lp.signal.Val == lp.val)
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
                    if (p.AlarmVal != p.Val)
                    {
                        string msg;
                        if (p.Val)
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
            throw new KeyNotFoundException(string.Format("SignalList: Find: Сигнал {0} не найден", _name));
        }
        public void ClearAllOutputSignals()
        {
            for (int i = 0; i < M.Count; i++)
                if(!M[i].input)M[i].Val = false;
        }
        public void ClearAllInputSignals()
        {
            for (int i = 0; i < M.Count; i++)
                if(M[i].input)M[i].Val = false;
        }
    }
}
