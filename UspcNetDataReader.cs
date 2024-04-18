using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Diagnostics;
using System.Threading;

namespace USPC
{
    public delegate void OnDataAcquired(int _NumberOfScans, AcqAscan[] _data);
    class UspcNetDataReader : BackgroundWorker
    {
        //PCXUS pcxus = null;
        string serverAddr;
        AcqAscan[] data = null;
        int Board = 0;
        bool bErrInsufficientMemory = false;
        FRTestAcqNet frTestAcq = null;
        PCXUSNetworkClient client = null;
        Object retval = null;

        
        public OnDataAcquired dataAcquired = null;
        public UspcNetDataReader(FRTestAcqNet _frTestAcq)
        {
            frTestAcq = _frTestAcq;
            serverAddr = _frTestAcq.serverAddr;
            data = frTestAcq.data;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
           
            client = new PCXUSNetworkClient(serverAddr);
            retval = new Object();

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
            client.callNetworkFunction(string.Format("stop,{0}",Board),out retval);
            client.callNetworkFunction(string.Format("clear,{0}",Board), out retval);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage == 0)
                {
                    int countFrames = (int)e.UserState;
                    frTestAcq.updateGraph(countFrames, data);
                    if (Program.data.currentOffsetFrames + countFrames <= Program.data.ascanBuffer.Length)
                    {
                        Array.Copy(data, 0, Program.data.ascanBuffer, Program.data.currentOffsetFrames, countFrames);
                        Program.data.OffsetCounter(countFrames);
                        frTestAcq.frMain.setSb("dataSize", string.Format("{0}", Program.data.currentOffsetFrames));
                        return;
                    }
                    else
                    {
                        //throw new InsufficientMemoryException("Закончилось место в буфере сканирования");
                        bErrInsufficientMemory = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            //Подготавливаем к захвату
            if (client.callNetworkFunction(string.Format("config,{0}",Board),out retval)!=0)
            {
                return;
            }
            //Получим информацию о статусе
            if (client.callNetworkFunction(string.Format("status,{0}",Board), out retval) != 0)
            {
                return;
            }
            AcqSatus packet = (AcqSatus)retval;
            log.add(LogRecord.LogReason.info, "ACQ_STATUS: {0}, BufferSize(in numbers od scans): {1}, ScanSize(in number of DWORD): {2}", ((ACQ_STATUS)packet.status).ToString(), packet.bufferSize, packet.scanSize);
            if (client.callNetworkFunction(string.Format("start,{0}",Board), out retval) != 0)
            {
                return;
            }
            //Смещаем указатель буфера в начало
            Program.data.Start();

            while (true)
            {
                //log.add(LogRecord.LogReason.info, "{0}: {1}: Worker cycle {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, workerCycleCounter++);
                //Получим информацию о статусе
                int err = client.callNetworkFunction(string.Format("status,{0}",Board), out retval);
                if (err != 0)
                {
                    log.add(LogRecord.LogReason.error, "PCXUS_ACQ_GET_STATUS return {0:8X}", err);
                    e.Cancel = true;
                    return;
                }
                //log.add(LogRecord.LogReason.info, "ACQ_STATUS: {0}, NumberOfScansAcquired: {1}, NumberOfScansRead: {2}", ((ACQ_STATUS)status).ToString(), NumberOfScansAcquired, NumberOfScansRead);
                try
                {
                    if (bErrInsufficientMemory)
                    {
                        throw new InsufficientMemoryException("Закончилось место в буфере сканирования");
                    }
                    Int32 NumberOfScans = client.callNetworkFunction("read", out retval);
                    //log.add(LogRecord.LogReason.info, "NumberOfScans: {0}", NumberOfScans);
                    AcqAscan[] buffer = (AcqAscan[])retval;
                    Array.Copy(buffer, data, NumberOfScans);
                    if (dataAcquired != null) dataAcquired(NumberOfScans, buffer);
                    ReportProgress(0, (Object)NumberOfScans);
                    if (CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
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
