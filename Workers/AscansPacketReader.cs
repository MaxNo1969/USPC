using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using Data;
using Settings;
using System.Threading;

namespace USPC
{
    public delegate void OnDataAcquired(int _NumberOfScans, AcqAscan[] _data);
    class AscansPacketReader : BackgroundWorker
    {
        public OnDataAcquired dataAcquired = null;
        Result result = Program.result;
        IPCXUS pcxus = Program.pcxus;
        PCXUSNetworkClient client = null;
        object retval = new object();

        public AscansPacketReader()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            client = new PCXUSNetworkClient(AppSettings.s.serverAddr);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: Останавливаем захват",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
            client.callNetworkFunction("stop_read_ascans", out retval);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.add(LogRecord.LogReason.debug,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.ProgressPercentage);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug,"{0}: {1}: ", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            client.callNetworkFunction("start_read_ascans", out retval);
            while (true)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                try
                {
                    int countAscans = client.callNetworkFunction("get_ascans", out retval);
                    Ascan[] ascans = (Ascan[])retval;
                    for (int i = 0; i < countAscans; i++)
                    {
                        Ascan ascan = ascans[i];
                        int board = ascan.G2Amp;
                        int realSensor = ascan.ChannelNumber + board * 4;
                        result.values[result.zone][realSensor].Add(ascan);
                    }
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error:{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    log.add(LogRecord.LogReason.error, "{0}", ex.StackTrace);
                }
                Thread.Sleep(AppSettings.s.BoardReadTimeout);
            }
        }
    }
}
