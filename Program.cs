﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PROTOCOL;
using System.Diagnostics;
using System.Threading;
using FPS;
using PCI1730;
using Data;

namespace USPC
{
    public enum BoardState { NotOpened=0, Opened, Error };
    static class Program
    {
        public static Dictionary<string, string> cmdLineArgs = null;

        public static BoardState boardState = BoardState.NotOpened;
        public static int numBoards = 2;
        public static string serverAddr;
        public static PCXUSNET pcxus = null;
        public static int bufferSize = 1024 * 100;
        public static USPCData[] data = new USPCData[numBoards];
        public static TubeResult result = new TubeResult();
        public static TypeSize typeSize = new TypeSize();
        public static FRMain frMain = null;
        public static int medianFilterWidth = 5;

        public static SignalList sl = null;

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
                cmdLineArgs = getCmdStr(args);
                if (cmdLineArgs != null)
                {
                    #region Логирование
                    {
                        string msg = string.Empty;
                        foreach (KeyValuePair<string, string> kv in cmdLineArgs)
                        {
                            msg += string.Format(@"{0}={1}; ", kv.Key, kv.Value);
                            msg = msg.Trim();
                        }
                        log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, msg);
                    }
                    #endregion
                    serverAddr = Program.cmdLineArgs["Server"];
                }
                else
                {
                    serverAddr = "localhost";
                }
                pcxus = new PCXUSNET(serverAddr);
            }
            catch (ArgumentException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowExceptionDetails(ex);
                return -1;
            }
            catch (KeyNotFoundException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowExceptionDetails(ex);
                return -1;
            }

            try
            {
                FormPosSaver.deser();
                sl = new SignalList();
                sl["РЕЗЕРВ1"].Val = true;
                for (int i = 0; i < numBoards;i++ )
                {
                    data[i] = new USPCData();
                }
                frMain = new FRMain();
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
                //if (boardState == BoardState.Opened || boardState == BoardState.Error)
                pcxus.close();
                //Снимаем все выходные сигналы и останавливаем PCIE1730
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
