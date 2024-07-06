using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PROTOCOL;

namespace USPC
{
    class ZoneThread
    {
        public delegate void OnZoneAdded();
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
    }
}
