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
    class UspcNetDataReader : BackgroundWorker
    {
        USPCData data = null;
        PCXUSNetworkClient client = null;
        Object retval = null;
        int board;

        public OnDataAcquired dataAcquired = null;
        public UspcNetDataReader(int _board)
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            data = Program.data[_board];
            board = _board;
            client = new PCXUSNetworkClient(AppSettings.s.serverAddr);
            retval = new object();

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
            //client.callNetworkFunction(string.Format("stop,{0}", board), out retval);
            Program.pcxus.stop(board);
            //client.callNetworkFunction(string.Format("clear,{0}", board), out retval);
            Program.pcxus.clear(board);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.debug,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            //Подготавливаем к захвату
            //if (client.callNetworkFunction(string.Format("config,{0}", board), out retval) != 0)
            //{
            //    return;
            //}
            Program.pcxus.config(board, AppSettings.s.BufferSize, AppSettings.s.InterruptFluidity);
            //Получим информацию о статусе
            //if (client.callNetworkFunction(string.Format("status,{0}", board), out retval) != 0)
            //{
            //    return;
            //}
            AcqSatus acqStatus = new AcqSatus();
            Program.pcxus.status(board, ref acqStatus.status, ref acqStatus.NumberOfScansAcquired, ref acqStatus.NumberOfScansRead, ref acqStatus.bufferSize, ref acqStatus.scanSize);
            log.add(LogRecord.LogReason.info, "Board: {0}, ACQ_STATUS: {1}, BufferSize(in numbers od scans): {2}, ScanSize(in number of DWORD): {3}", board, ((ACQ_STATUS)acqStatus.status).ToString(), acqStatus.bufferSize, acqStatus.scanSize);
            //if (client.callNetworkFunction(string.Format("start,{0}", board), out retval) != 0)
            //{
            //    return;
            //}
            Program.pcxus.start(board);
            //Смещаем указатель буфера в начало
            Program.data[board].Start();

            while (true)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Получим информацию о статусе
                //int err = client.callNetworkFunction(string.Format("status,{0}", board), out retval);
                //if (err != 0)
                //{
                //    log.add(LogRecord.LogReason.error, "PCXUS_ACQ_GET_STATUS return {0:8X}", err);
                //    e.Cancel = true;
                //    return;
                //}
                try
                {
                    if (Program.pcxus.status(board, ref acqStatus.status, ref acqStatus.NumberOfScansAcquired, ref acqStatus.NumberOfScansRead, ref acqStatus.bufferSize, ref acqStatus.scanSize))
                    {
                        Int32 NumberOfScans = client.callNetworkFunction(string.Format("read,{0}", board), out retval);
                        if (retval != null)
                        {
                            AcqAscan[] buffer = (AcqAscan[])retval;
                            if (dataAcquired != null) dataAcquired(NumberOfScans, buffer);
                            Array.Copy(buffer, 0, data.ascanBuffer, data.currentOffsetFrames, NumberOfScans);
                            data.labels.Add(new BufferStamp(DateTime.Now, data.currentOffsetFrames));
                            data.currentOffsetFrames += NumberOfScans;
                            ReportProgress(NumberOfScans, (object)buffer);
                        }
                        else
                        {
                            log.add(LogRecord.LogReason.error, "{0}: {1}: Error:{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "read не вернула пакет");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error:{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    //Program.pcxus.stop(board);
                    //Program.pcxus.clear(board);
                    //e.Cancel = true;
                    //return;
                }
                Thread.Sleep(AppSettings.s.BoardReadTimeout);
            }
        }
    }
}
