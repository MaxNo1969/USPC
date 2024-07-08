﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PROTOCOL;
using System.Diagnostics;
using System.Threading;
using FPS;
using PCI1730;
using Data;
using USPC.PCI_1730;
using Settings;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Remoting.Messaging;
using USPC.Workers;

namespace USPC
{
    public enum BoardState { NotOpened=0, Opened, Error };
    static class Program
    {
        public static Dictionary<string, string> cmdLineArgs = null;

        public static BoardState boardState = BoardState.NotOpened;
        public const int numBoards = 2;
        public static readonly int[] channelsOnBoard = { 4, 8 };
        public const int numChannes = 12;
        public static PCXUSNET pcxus = null;
        public static void prepareBoardsForWork(bool _foAcquition)
        {
            Program.pcxus.close();
            Program.pcxus.open(2);
            Program.pcxus.load(Program.typeSize.currentTypeSize.configName);
            if (_foAcquition)
            {
                for (int board = 0; board < Program.numBoards; board++)
                {
                    Program.pcxus.config(board, AppSettings.s.BufferSize, AppSettings.s.InterruptFluidity);
                    AcqSatus acqStatus = new AcqSatus();
                    Program.pcxus.status(board, ref acqStatus.status, ref acqStatus.NumberOfScansAcquired, ref acqStatus.NumberOfScansRead, ref acqStatus.bufferSize, ref acqStatus.scanSize);
                    log.add(LogRecord.LogReason.info, "Board: {0}, ACQ_STATUS: {1}, BufferSize(in numbers od scans): {2}, ScanSize(in number of DWORD): {3}", board, ((ACQ_STATUS)acqStatus.status).ToString(), acqStatus.bufferSize, acqStatus.scanSize);
                    Program.pcxus.start(board);
                }
            }
        }

        public static TypeSize typeSize = new TypeSize();
        public static Result result = new Result();
        public static void saveResult(string _fileName)
        {
        }
        public static FRMain frMain = null;
        public static int medianFilterWidth = 5;

        public static DefSignals sl = null;
        public static double scopeVelocity = 6400.0;

        public static int tubesCount = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Пытаемся поймать все исключения
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Add handler to handle the exception raised by main threads
            Application.ThreadException += new ThreadExceptionEventHandler(HandleError);
            // Add handler to handle the exception raised by additional threads
            AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            int ret = 0;
            //В первую очередь запускаем логирование
            log.add(LogRecord.LogReason.info,@"Program: Начало выполнения программы.");
            try
            {
                FormPosSaver.deser();
                pcxus = new PCXUSNET(AppSettings.s.serverAddr);
                sl = new DefSignals();
                frMain = new FRMain();
                if(ThreadPool.SetMinThreads(1000, 100))
                    Application.Run(frMain);
            }
            catch (Exception e)
            {
                ShowExceptionDetails(e);
                ret = -1;
            }
            finally
            {
                log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, "Зашли в finally");
                if (boardState == BoardState.Opened || boardState == BoardState.Error)pcxus.close();
                //Снимаем все выходные сигналы и останавливаем PCIE1730
                sl.ClearAllOutputSignals();
                sl.Dispose();
                FormPosSaver.ser();
            }
            return ret;
        }
        static void HandleError(object sender, ThreadExceptionEventArgs e)
        {
            //Логирование исключения, завершение или продолжение работы
            //MessageBox.Show(e.Exception.Message, "Аварийный выход", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowExceptionDetails(e.Exception);
            Application.Exit();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionDetails(e.ExceptionObject as Exception);
            Application.Exit();
        }
        static void ShowExceptionDetails(Exception Ex)
        {
            // Do logging of exception details
            log.add(LogRecord.LogReason.error,"{0}: {1}:", "ThreadException", System.Reflection.MethodBase.GetCurrentMethod().Name);
            log.add(LogRecord.LogReason.error, "Type:{0} \nMessage:{1}\nTargetSite:{2}\nCall Stack:{3}", Ex.GetType().Name, Ex.Message, Ex.TargetSite.Name, Ex.StackTrace);
            MessageBox.Show(string.Format("Type:{0} \nMessage:{1}\nTargetSite:{2}\nCall Stack:{3} ",
                Ex.GetType().Name, Ex.Message, Ex.TargetSite.Name, Ex.StackTrace), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static Dictionary<string, string> getCmdStr(string[] args)
        {
            if ((args == null) || args.Length < 1)
            {
                return null;                
            }
            Dictionary<string, string> cmdStr = new Dictionary<string, string>();
            foreach (string s in args)
            {
                string[] ss = s.Split(new char[] { ':' });
                if (ss[0][0] != '/')
                {
                    throw new ArgumentException("Ошибка аргументов командной строки");
                }
                if (ss.Length > 1)
                    cmdStr[ss[0].Substring(1)] = ss[1];
                else
                    cmdStr[ss[0].Substring(1)] = "true";
            }
            return cmdStr;
        }

    }
}
