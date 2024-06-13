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

    }
}
