using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PROTOCOL;
using System.Diagnostics;
using System.Threading;
using Data;

namespace USPC
{
    public delegate void OnDataAcquired(int _NumberOfScans, AcqAscan[] _data);
    class UspcNetDataReaderForTest : BackgroundWorker
    {
        //PCXUS pcxus = null;
        string serverAddr;
        AcqAscan[] data = null;
        int Board = 0;
        FRTestAcqNet frTestAcq = null;
        PCXUSNetworkClient client = null;
        Object retval = null;

        
        public OnDataAcquired dataAcquired = null;
        public UspcNetDataReaderForTest(FRTestAcqNet _frTestAcq)
        {
            frTestAcq = _frTestAcq;
            serverAddr = Program.serverAddr;
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
                int countFrames = e.ProgressPercentage;
                frTestAcq.updateGraph(countFrames, data);
                //frTestAcq.frMain.setSb("dataSize", string.Format("{0}", Program.data.currentOffsetFrames));
                AcqAscan[] buffer = new AcqAscan[countFrames];
                Array.Copy(data, buffer, countFrames);
                StructToCsv.writeCsv<AcqAscan>("acqscans.csv", buffer); 
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
            Program.data[Board].Start();

            while (true)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Получим информацию о статусе
                int err = client.callNetworkFunction(string.Format("status,{0}",Board), out retval);
                if (err != 0)
                {
                    log.add(LogRecord.LogReason.error, "PCXUS_ACQ_GET_STATUS return {0:8X}", err);
                    e.Cancel = true;
                    return;
                }
                //AcqSatus status = (AcqSatus)retval;
                //log.add(LogRecord.LogReason.info, "ACQ_STATUS: {0}, NumberOfScansAcquired: {1}, NumberOfScansRead: {2}", ((ACQ_STATUS)status.status).ToString(), status.NumberOfScansAcquired, status.NumberOfScansRead);
                try
                {
                    Int32 NumberOfScans = client.callNetworkFunction("read", out retval);
                    AcqAscan[] buffer = (AcqAscan[])retval;
                    if (dataAcquired != null) dataAcquired(NumberOfScans, buffer);
                    if (Program.data[Board].currentOffsetFrames + NumberOfScans <= USPCData.countFrames)
                    {
                        Array.Copy(buffer, data, NumberOfScans);
                        Array.Copy(buffer, 0, Program.data[Board].ascanBuffer, Program.data[Board].currentOffsetFrames, NumberOfScans);
                        Program.data[Board].OffsetCounter(NumberOfScans);
                        ReportProgress(NumberOfScans);
                    }
                    else
                    {
                        throw new InsufficientMemoryException("Закончилось место в буфере сканирования");
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
