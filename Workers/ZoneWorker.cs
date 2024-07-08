using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Threading;
using Settings;
using Data;
using PCI1730;

namespace USPC
{
    class ZoneWorker:BackgroundWorker
    {
        private const int waitStrobeTime = 30*1000;
         
        public ZoneWorker()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            //ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}: e.ProgressPercentage = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.ProgressPercentage);
        }

        int strobCounter = 0;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Result result = Program.result;
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "ZoneWorker";
            while (!CancellationPending)
            {
                if (Program.sl["СТРОБ"].Val)
                {
                    //if (strobCounter > 0)
                    {
                        if (result.zone >= 0) result.CalcZone(result.zone);
                        result.AddZone();
                    }
                    ReportProgress(0, null);
                    strobCounter++;
                    Thread.Sleep(AppSettings.s.StrobResetTimeout);
                }
            }
        }
    }
}
