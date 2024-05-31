using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using Data;

namespace USPC
{
    public delegate void OnDataAcquired(int _NumberOfScans, AcqAscan[] _data);
    class UspcNetDataReader : BackgroundWorker
    {
        string serverAddr;
        USPCData data = null;
        PCXUSNetworkClient client = null;
        Object retval = null;
        int Board;

        public OnDataAcquired dataAcquired = null;
        public UspcNetDataReader(int _board)
        {
            serverAddr = Program.serverAddr;
            data = Program.data[_board];
            Board = _board;
            client = new PCXUSNetworkClient(serverAddr);
            retval = new object();

            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
            client.callNetworkFunction(string.Format("stop,{0}", Board), out retval);
            client.callNetworkFunction(string.Format("clear,{0}", Board), out retval);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            //Подготавливаем к захвату
            if (client.callNetworkFunction(string.Format("config,{0}", Board), out retval) != 0)
            {
                return;
            }
            //Получим информацию о статусе
            if (client.callNetworkFunction(string.Format("status,{0}", Board), out retval) != 0)
            {
                return;
            }
            AcqSatus packet = (AcqSatus)retval;
            log.add(LogRecord.LogReason.info, "Board: {0}, ACQ_STATUS: {1}, BufferSize(in numbers od scans): {2}, ScanSize(in number of DWORD): {3}", Board, ((ACQ_STATUS)packet.status).ToString(), packet.bufferSize, packet.scanSize);
            if (client.callNetworkFunction(string.Format("start,{0}", Board), out retval) != 0)
            {
                return;
            }
            //Смещаем указатель буфера в начало
            Program.data[Board].Start();

            while (true)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Получим информацию о статусе
                int err = client.callNetworkFunction(string.Format("status,{0}", Board), out retval);
                if (err != 0)
                {
                    log.add(LogRecord.LogReason.error, "PCXUS_ACQ_GET_STATUS return {0:8X}", err);
                    e.Cancel = true;
                    return;
                }
                try
                {
                    Int32 NumberOfScans = client.callNetworkFunction("read", out retval);
                    AcqAscan[] buffer = (AcqAscan[])retval;
                    if (dataAcquired != null) dataAcquired(NumberOfScans, buffer);
                    //Array.Copy(buffer, 0, data.ascanBuffer, data.currentOffsetFrames,NumberOfScans);
                    //data.labels.Add(new BufferStamp(DateTime.Now, data.currentOffsetFrames));
                    //data.currentOffsetFrames += NumberOfScans;
                    ReportProgress(NumberOfScans,(object)buffer);
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error:{2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                    e.Cancel = true;
                    return;
                }
                //Thread.Sleep(50);
            }
        }
    }
}
