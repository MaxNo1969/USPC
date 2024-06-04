using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Threading;
using Settings;
using Data;

namespace USPC
{
    class ZoneBackGroundWorker:BackgroundWorker
    {
         
        public ZoneBackGroundWorker()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: e.ProgressPercentage = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            while (true)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Program.result.AddNewZone();
                int zoneTime = (int)((double)AppSettings.s.zoneSize/(double)AppSettings.s.speed);
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2} Now: {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, zoneTime,DateTime.Now.ToLongTimeString());
                ReportProgress(Program.data[0].currentOffsetFrames * 100 / USPCData.countFrames);
                Thread.Sleep(zoneTime*1000);
            }
        }

    }
}
