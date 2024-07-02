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
    class ZoneBackGroundWorker:BackgroundWorker
    {
        private const int waitStrobeTime = 30*1000;
         
        public ZoneBackGroundWorker()
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

        int[] currentOffsets = new int[Program.numBoards];
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            while (!CancellationPending)
            {
                if (Program.sl["СТРОБ"].Val)
                {
                    for (int board = 0; board < Program.numBoards; board++)
                    {
                        currentOffsets[board] = Program.data[board].currentOffsetFrames;
                    }
                    if (currentOffsets.Sum() != 0)
                    {
                        Program.result.addZone(currentOffsets);
                        log.add(LogRecord.LogReason.info, "{0}: {1}: CurrentOffsets = {2} {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, currentOffsets[0], currentOffsets[1]);
                        ReportProgress(Program.data[0].currentOffsetFrames * 100 / USPCData.countFrames);
                        Thread.Sleep(AppSettings.s.StrobResetTimeout);
                    }
                }
            }
        }
    }
}
