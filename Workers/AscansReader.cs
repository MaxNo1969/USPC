using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using Data;
using System.Reflection;
using System.Threading;
using Settings;

namespace USPC.Workers
{
    class AscansReader:BackgroundWorker
    {
        public AscansReader()
            : base()
        {
            //Натраиваем параметры воркера
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: e.Cancelled = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Cancelled);
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string  str = (string)e.UserState; 
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, str);
        }

        int timeout = 200;
        Ascan ascan = new Ascan();

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            Random r = new Random();
            Result result = Program.result;
            //result.AddZone();
            while (!CancellationPending)
            {
                if (result.values[result.zone] == null) return;
                //if (result.zone == 0) return;
                for (int board = 0; board < Program.numBoards; board++)
                {
                    for (int channel = 0; channel < USPCData.countSensors; channel++)
                    {
                        double def = 0;
                        double thick = 0;
                        if (Program.pcxus.readAscan(board, channel, ref ascan, timeout))
                        {
                            def = ascan.G1Amp;
                            uint tof = ascan.G1TofWt * 5;
                            thick = USPCData.TofToMm(tof);
                        }
                        else
                        {
                            def = Result.deadZone;
                            thick = Result.deadZone;
                        }

                        //log.add(LogRecord.LogReason.debug, "board={0} channel={1} def={2} thick={3}",board,channel,def,thick);
                        if (board == 0)
                        {
                            if (channel < 4)
                            {
                                result.values[result.zone][channel].Add(thick);
                            }
                            else
                            {
                                //log.add(LogRecord.LogReason.error, "{0}: {1}: Попытка чтения канала {2} с  платы {3}", GetType().Name, MethodBase.GetCurrentMethod().Name, channel, board);
                            }
                        }
                        else
                        {
                            if (channel < 4)
                            {
                                result.values[result.zone][channel + 4].Add(def);
                            }
                            else if (channel < 8)
                            {
                                result.values[result.zone][channel + 4].Add(def);
                            }
                            else
                            {
                                //log.add(LogRecord.LogReason.error, "{0}: {1}: Попытка чтения канала {2} с  платы {3}", GetType().Name, MethodBase.GetCurrentMethod().Name, channel, board);
                            }
                        }
                    }
                }
                //Thread.Sleep(50);
            }
        }
    }
}
