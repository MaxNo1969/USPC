using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using System.ComponentModel;
using PROTOCOL;

namespace Data
{
    //! @brief Класс для гненерации различных измерений 
    public static class DataGenerator
    {
        static Random r = new Random();
        public static void GenerateThicknessData(double _thick, int _count = Program.countFrames, BackgroundWorker _w = null, DoWorkEventArgs _e = null)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", "DataGenerator", System.Reflection.MethodBase.GetCurrentMethod().Name, "Начало генерации данных");
            if (Program.data == null) return;
            AcqAscan[] scans = Program.data.ascanBuffer;
            for (int i = 0; i < _count; i += Program.countSensors)
            {
                if (_w != null && _w.CancellationPending)
                {
                    if (_e != null) _e.Cancel = true;
                }
                else
                {
                    
                    int percent = (int)((double)i * 100 / (double)Program.countFrames);
                    if (_w != null) _w.ReportProgress(percent);
                    for (int j = 0; j < Program.countSensors; j++)
                    {
                        scans[i + j].Channel = (byte)j;
                        double val = _thick + (r.NextDouble() - 0.5) / 10.0;
                        uint G1Tof = (uint)(val / (2.5e-6 * Program.scopeVelocity));
                        scans[i + j].G1Tof = G1Tof;
                    }
                }
            }
            for (int i = 0; i < Program.countZones; i++)
            {
                Program.data.commonStatus[i] = 6;
            }
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", "DataGenerator", System.Reflection.MethodBase.GetCurrentMethod().Name, "Генерация данных закончена");
        }
        public static void GeterateDefectsData(int _percent, int _count = Program.countFrames)
        {
            if (Program.data == null) return;
            AcqAscan[] scans = Program.data.ascanBuffer;
            for (int i = 0; i < _count; i += Program.countSensors)
            {
                for (int j = 0; j < Program.countSensors; j++)
                {
                    scans[i + j].Channel = (byte)j;
                    int val = _percent + r.Next(-10,10);
                    scans[i + j].G1Amp = (byte)val;
                }
            }
        }
    }
}
