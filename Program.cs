using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PROTOCOL;
using System.Diagnostics;
using System.Threading;
using FPS;
using PCIE1730;
using Data;

namespace USPC
{
    static class Program
    {
        public const int countZones = 300;
        public const int countSensors = 8;
        public const int zoneLength = 50;

        public const int countFrames = 900000;
        //public const int countFrames = 9000;
        public const int countFramesPerChannel = countFrames / countSensors;
        public const int countFramesPerZone = countFrames / countZones;

        public const int scopeVelocity = 6400;
        
        public static Dictionary<string, string> cmdLineArgs = null;
        public static int lengthCaretka = 20;

        public static USPCData data = new USPCData();
        public static int medianFilterWidth = 5;

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
                }
            }
            catch (ArgumentException ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return -1;
            }
            if (cmdLineArgs != null)
            {
                if (cmdLineArgs.ContainsKey(@"/Emul"))
                {
                }
            }
            try
            {
                FormPosSaver.deser();
                Application.Run(new FRMain());
            }
            catch (Exception e)
            {
                ShowExceptionDetails(e);
                ret = -1;
            }
            finally
            {
                Debug.WriteLine("Вошли в program/finally");
                //Снимаем все выходные сигналы и останавливаем PCIE1730
                SL.Destroy();
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
            Dictionary<string, string> cmdStr = new Dictionary<string, string>();
            if ((args == null) || args.Length < 1)
            {
                cmdStr.Add("NONE", "true");
                return cmdStr;
            }
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
