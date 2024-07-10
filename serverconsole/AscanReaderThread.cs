using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using serverconsole.Properties;
using PROTOCOL;

namespace USPC
{
    class AscanReaderThread
    {
        Thread thread = null;
        ThreadStart threadStart = null;
        volatile bool terminate = false;
        IPCXUS pcxus;
        public Queue<Ascan> queue = Program.queue;
        int timeout = Settings.Default.AscanTimeout;
        int threadTimeout = Settings.Default.ThreadTimeout;
        public int ascansCount = 0;
        public AscanReaderThread(IPCXUS _pcxus)
        {
            pcxus = _pcxus;
            threadStart = new ThreadStart(ReadAscans);
            thread = new Thread(threadStart);
            thread.Name = "AscansReaderThread";
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Normal;
        }
        public void start()
        {
            thread.Start();
            log.add(LogRecord.LogReason.debug, "{0} запущен ThreadState={1}", thread.Name, thread.ThreadState);
        }
        public void stop()
        {
            terminate = true;
            thread.Join();
            log.add(LogRecord.LogReason.debug, "{0} остановлен ThreadState={1}", thread.Name, thread.ThreadState);
        }
        void ReadAscans()
        {
            Ascan ascan = new Ascan();
            ascansCount = 0;
            while (!terminate)
            {
                for (int board = 0; board < 2; board++)
                {
                    int countSensor = (board == 0) ? 4 : 8;
                    for (int sensor = 0; sensor < countSensor; sensor++)
                    {
                        if (terminate) return;
                        if (pcxus.readAscan(board, sensor, ref ascan, timeout))
                        {
                            try
                            {
                                //Номер платы запишем в G2Amp
                                ascan.G2Amp = (byte)board;
                                queue.Enqueue(ascan);
                            }
                            catch (OutOfMemoryException ex)
                            {
                                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                                queue.Clear();
                            }

                            ascansCount++;
                            if(ascansCount%1000==0)
                                log.add(LogRecord.LogReason.info, "{0}: {1}: Считано {2} сканов", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ascansCount);
                        }
                        else
                        {
                            log.add(LogRecord.LogReason.warning, "{0}: {1}: Не удалось прочитать ascan board={2} sensor={3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, board, sensor);
                        }
                        Thread.Sleep(threadTimeout);
                    }
                }
            }
            //log.add(LogRecord.LogReason.debug, "{0}: {1}: {2} Вышли из ReadAscans", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, thread.Name);
            return;
        }
    }
}
