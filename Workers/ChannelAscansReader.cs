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
using USPC.Data;

namespace USPC.Workers
{

    class ChannelAscansReader : BackgroundWorker
    {
        int board;
        int channel;
        int timeout;
        public ChannelAscansReader(int _board,int _channel,int _timeout):base()
        {
            board = _board;
            channel = _channel;
            timeout = _timeout;
            //Натраиваем параметры воркера
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2} board={3} channel={4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started", board, channel);
            Result result = Program.result;
            while (!CancellationPending)
            {
                //Получим номер канала в result
                int resultChannel = board * 4 + channel;
                Ascan ascan = new Ascan();
                if (Program.pcxus.readAscan(board, channel, ref ascan, timeout))
                {
                    double amp = ascan.G1Amp;
                    uint tof = ascan.G1TofWt * 5;
                    double thick = ThickConverter.TofToMm(tof);
                    double val = (board == 0) ? thick : amp;
                    if (result.values[result.zone] != null && result.values[result.zone][resultChannel] != null)
                    {
                        result.values[result.zone][resultChannel].Add(val);
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
                    Program.result.values[result.zone][resultChannel].Add(Result.notMeasured);
                    log.add(LogRecord.LogReason.warning, "{0}: {1}: Не удалось прочитать ascan board={2} channel={3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, board, channel);
                }
            }
        }
    }
}
