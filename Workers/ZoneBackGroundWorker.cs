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
    public delegate void OnZoneAdded();
    class ZoneThread
    {
        private const int waitStrobeTime = 30 * 1000;

        private ThreadStart threadStart;
        private Thread thread;
        public OnZoneAdded zoneAdded = null;
        int[] currentOffsets = new int[Program.numBoards];

        private bool terminate = false;

        public ZoneThread()
        {
            threadStart = new ThreadStart(AddNewZone);
            thread = new Thread(threadStart);
        }
        public void start()
        {
            thread.Start();
        }
        public void stop()
        {
            terminate = true;
            thread.Join();
        }
        private void AddNewZone()
        {
            while (!terminate)
            {
                try
                {
                    string s = Program.sl["СТРОБ"].Wait(true, waitStrobeTime);
                    if (s != "Не дождались")
                    {
                        for (int board = 0; board < Program.numBoards; board++)
                        {
                            currentOffsets[board] = Program.data[board].currentOffsetFrames;
                        }
                        log.add(LogRecord.LogReason.info, "{0}: {1}: {2} CurrentOffsets = {3} {4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "СТРОБ", currentOffsets[0], currentOffsets[1]);
                        Program.result.addZone(currentOffsets);
                        if (zoneAdded != null) zoneAdded();
                        //int progres = Program.result.zones * AppSettings.s.zoneSize * 100 / AppSettings.s.tubeLength;
                        Program.sl.set(Program.sl["СТРБРЕЗ"], true);
                        Thread.Sleep(100);
                        Program.sl.set(Program.sl["СТРБРЕЗ"], false);
                    }
                    else
                    {
                        log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не дождались сигнала \"СТРОБ\"");
                        return;
                    }
                }
                catch (ThreadInterruptedException ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    terminate = true;
                }
                catch (ThreadAbortException ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    Thread.ResetAbort();
                    terminate = true;
                }
            }
        }

        //private void AddNewZone()
        //{
        //    while (!terminate)
        //    {
        //        if (Program.sl["СТРОБ"].Val)
        //        {
        //            for (int board = 0; board < Program.numBoards; board++)
        //            {
        //                currentOffsets[board] = Program.data[board].currentOffsetFrames;
        //            }
        //            log.add(LogRecord.LogReason.info, "{0}: {1}: {2} CurrentOffsets = {3} {4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "СТРОБ", currentOffsets[0], currentOffsets[1]);
        //            Program.result.addZone(currentOffsets);
        //            if (zoneAdded != null) zoneAdded();
        //            //int progres = Program.result.zones * AppSettings.s.zoneSize * 100 / AppSettings.s.tubeLength;
        //            Program.sl.set(Program.sl["СТРБРЕЗ"], true);
        //            Thread.Sleep(100);
        //            Program.sl.set(Program.sl["СТРБРЕЗ"], false);
        //        }
        //        Thread.Sleep(10);
        //    }
        //}
    }

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
            //RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
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
            //bool isRunning = false;
            while (!CancellationPending)
            {
                //if (Program.sl["СТРОБ"].Val && !isRunning)
                //{
                //    isRunning = true;
                //    for (int board = 0; board < Program.numBoards; board++)
                //    {
                //        currentOffsets[board] = Program.data[board].currentOffsetFrames;
                //    }
                //    log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "СТРОБ");
                //    Program.result.addZone(currentOffsets);
                //    //int zoneTime = (int)((double)AppSettings.s.zoneSize/(double)AppSettings.s.speed);
                //    //log.add(LogRecord.LogReason.info, "{0}: {1}: ZoneTime = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, zoneTime);
                //    ReportProgress(Program.data[0].currentOffsetFrames * 100 / USPCData.countFrames);
                //    Program.sl.set(Program.sl["СТРБРЕЗ"], true);
                //    Thread.Sleep(100);
                //    Program.sl.set(Program.sl["СТРБРЕЗ"], false);
                //    isRunning = false;
                //}
                //Thread.Sleep(100);
                string s = Program.sl["СТРОБ"].Wait(true, waitStrobeTime);
                if (s != "Не дождались")
                {
                    for (int board = 0; board < Program.numBoards; board++)
                    {
                        currentOffsets[board] = Program.data[board].currentOffsetFrames;
                    }
                    log.add(LogRecord.LogReason.info, "{0}: {1}: {2} CurrentOffsets = {3} {4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "СТРОБ",currentOffsets[0],currentOffsets[1]);
                    Program.result.addZone(currentOffsets);
                    int progres = Program.result.zones * AppSettings.s.zoneSize * 100 / AppSettings.s.tubeLength;
                    ReportProgress(progres);
                    Program.sl.set(Program.sl["СТРБРЕЗ"], true);
                    Thread.Sleep(100);
                    Program.sl.set(Program.sl["СТРБРЕЗ"], false);
                }
                else
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не дождались сигнала \"СТРОБ\"");
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
