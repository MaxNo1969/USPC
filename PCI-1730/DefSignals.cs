using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI1730;
using PROTOCOL;
using Settings;

namespace USPC.PCI_1730
{
    class DefSignals:SignalList
    {
        public DefSignals():base()
        {
            //Читаем сигналы из AppSettings
            List<SignalSettings> listSignalSettings = AppSettings.s.pcie1730Settings.sl;
            int cnt = listSignalSettings.Count;
            log.add(LogRecord.LogReason.info, "{0}: {1}: Будем читать {2} сигналов.", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, cnt);
            for (int i = 0; i < cnt; i++)
            {
                Signal sg = new Signal(listSignalSettings[i].name, WriteSignals, OnWait, SignalsLock)
                {
                    sgSet = listSignalSettings[i]
                };
                M.Add(sg);
                log.add(LogRecord.LogReason.info, "{0} {1}-{2}(Digital={3},EOn={4},EOff={5},Timeout={6},No_reset={7},Verbal={8})", sg.position, sg.name, sg.hint, sg.digital, sg.eOn, sg.eOn, sg.timeout, sg.no_reset, sg.verbal);
            }
        }

        public bool controlICC = false;
        public bool controlCYCLE = false;

        /// <summary>
        /// Виртуальная функция реакции
        /// </summary>
        protected override void Reaction()
        {
        }
        /// <summary>
        /// Виртуальная функция сигнал "Тревога"
        /// </summary>
        protected override void CheckAlarm()
        {
            if (controlCYCLE && !this["ЦИКЛ"].Val)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Пропал сигнал \"ЦИКЛ\"");
                this.ClearAllOutputSignals();
            }
        }
        public bool get(string _s)
        {
            if (this[_s] != null) return this[_s].Val;
            //else throw new KeyNotFoundException(string.Format("Не найден сигнал \"{0}\"",_s));
            else return false;
        }
        public void set(string _s,bool _val)
        {
            if (this[_s] != null) this[_s].Val = _val;
            //else throw new KeyNotFoundException(string.Format("Не найден сигнал \"{0}\"", _s));
        }

    }
}
