using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using USPC.Data;
using Data;
using Settings;
using PROTOCOL;

namespace USPC.Workers
{
    class ChannelReaderThread
    {
        int board = 0;
        int channel = 0;
        Thread thread = null;
        ThreadStart threadStart = null;
        bool terminate = false;
        Result result = Program.result;
        int timeout = AppSettings.s.BoardReadTimeout;

        public ChannelReaderThread(int _board, int _channel)
        {
            board = _board;
            channel = _channel;
            threadStart = new ThreadStart(ReadAscans);
            thread = new Thread(threadStart);
            thread.Name = string.Format("ChannelReaderThread-{0}-{1}", board, channel);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.AboveNormal;
        }
        public void start()
        {
            thread.Start();
            //log.add(LogRecord.LogReason.debug, "{0} запущен ThreadState={1}", thread.Name, thread.ThreadState);
        }
        public void stop()
        {
            terminate = true;
            thread.Join();
            //log.add(LogRecord.LogReason.debug, "{0} остановлен ThreadState={1}", thread.Name, thread.ThreadState);
        }
        void ReadAscans()
        {
            while (!terminate)
            {
                //Получим номер канала в result
                int resultChannel = board * 4 + channel;
                Ascan ascan = new Ascan();
                if (Program.pcxus.readAscan(board, channel, ref ascan, timeout))
                {
                    double amp = ascan.G1Amp;
                    uint tof = (ascan.G1TofWt & Ascan.TOF_MASK) * 5;
                    double thick = ThickConverter.TofToMm(tof);
                    double val = (board == 0) ? thick : amp;
                    if (result.values[result.zone] != null && result.values[result.zone][resultChannel] != null)
                    {
                        result.values[result.zone][resultChannel].Add(ascan);
                        //log.add(LogRecord.LogReason.debug, "{0}: {1}: board={2} channel={3} zone={4} count={5} val={6}", 
                        //    GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, 
                        //    board, channel,result.zone,result.values[result.zone][resultChannel].Count,val);
                    }
                    else
                    {
                        log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не добавлена зона или датчик");
                    }
                }
                else
                {
                    Program.result.values[result.zone][resultChannel].Add(result.notMeasuredAscan);
                    log.add(LogRecord.LogReason.warning, "{0}: {1}: Не удалось прочитать ascan board={2} channel={3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, board, channel);
                }
            }
            //log.add(LogRecord.LogReason.debug, "{0}: {1}: {2} Вышли из ReadAscans", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, thread.Name);
            return;
        }
    }
}
