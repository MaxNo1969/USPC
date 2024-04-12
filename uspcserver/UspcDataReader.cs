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
    class UspcDataReader : BackgroundWorker
    {
        PCXUS pcxus = null;
        AcqAscan[] data = null;
        int Board = 0;
        FRTestAcq frTestAcq = null;
        public OnDataAcquired dataAcquired = null;
        public UspcDataReader(FRTestAcq _frTestAcq)
        {
            frTestAcq = _frTestAcq;
            pcxus = frTestAcq.pcxus;
            data = frTestAcq.data;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += new DoWorkEventHandler(worker_DoWork);
            ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: e.Cancelled = {2}",GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,e.Cancelled);
            if (pcxus != null)
            {
                pcxus.stop(Board);
                pcxus.clear(Board);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                frTestAcq.updateGraph((int)e.UserState, data);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Worker started");
            //Подготовим плату к захвату
            Int32 status = 0;
            Int32 NumberOfScansAcquired = 0;
            Int32 NumberOfScansRead = 0;
            Int32 BufferSize = 0;
            Int32 ScanSize = 0;
            //Подготавливаем к захвату
            if (!pcxus.config(Board, 1024*100))
            {
                return;
            }
            //Получим информацию о статусе
            if (!pcxus.status(Board, ref status, ref NumberOfScansAcquired, ref NumberOfScansRead, ref BufferSize, ref ScanSize))
            {
                return;
            }
            log.add(LogRecord.LogReason.info, "ACQ_STATUS: {0}, BufferSize(in numbers od scans): {1}, ScanSize(in number of DWORD): {2}", ((ACQ_STATUS)status).ToString(), BufferSize, ScanSize);
            data = new AcqAscan[BufferSize];
            if (data == null)
            {
                log.add(LogRecord.LogReason.error, "Не удалось выделить {0} байт под буфер для захвата", BufferSize * ScanSize * sizeof(uint));
                return;
            }
            if (!pcxus.start(Board))
            {
                return;
            }

            while (true)
            {
                //log.add(LogRecord.LogReason.info, "{0}: {1}: Worker cycle {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, workerCycleCounter++);
                //Получим информацию о статусе
                if (!pcxus.status(Board, ref status, ref NumberOfScansAcquired, ref NumberOfScansRead, ref BufferSize, ref ScanSize))
                {
                    log.add(LogRecord.LogReason.error, "PCXUS_ACQ_GET_STATUS return {0:8X}", pcxus.Err);
                    e.Cancel = true;
                    return;
                }
                log.add(LogRecord.LogReason.info, "ACQ_STATUS: {0}, NumberOfScansAcquired: {1}, NumberOfScansRead: {2}", ((ACQ_STATUS)status).ToString(), NumberOfScansAcquired, NumberOfScansRead);
                Int32 NumberOfScans = pcxus.read(Board, data,200);
                log.add(LogRecord.LogReason.info, "NumberOfScans: {0}", NumberOfScans);
                //AcqAscan[] buffer = new AcqAscan[NumberOfScans];
                //Array.Copy(data, buffer, NumberOfScans);
                //if (dataAcquired != null) dataAcquired(NumberOfScans, buffer);
                ReportProgress(0,(Object)NumberOfScans);
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //Thread.Sleep(50);
            }
        }
    }
}
