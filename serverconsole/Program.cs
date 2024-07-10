using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;
using System.IO;
using serverconsole.Properties;

namespace USPC
{
    class Program
    {
        public static string uspcDir = Settings.Default.UspcDir;
        public static string configName = Settings.Default.ConfigName;
        public static Dictionary<string, string> cmdLineArgs = null;
        public static StreamWriter fileStream = null;
        private static void WriteLog()
        {
            LogRecord logRecord = log.get();
            if (logRecord != null)
            {
                Console.WriteLine(string.Format("{0}\t{1}\t{2}", logRecord.dt, logRecord.reason, logRecord.text));
                if (fileStream != null) fileStream.WriteLine(string.Format("{0}\t{1}\t{2}", logRecord.dt, logRecord.reason, logRecord.text));
            }
        }
        static void Main(string[] args)
        {
            log.onLogChanged += new log.OnLogChanged(WriteLog);
            string logDirectory = "Log";
            DateTime dt = DateTime.Now; 
            string fileName = logDirectory+"\\"+string.Format("{0}-{1:D4}-{2:D2}-{3:D2}-{4:D2}-{5:D2}-{6:D2}}.log","log",dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second);
            try
            {
                if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);
                fileStream = new StreamWriter(fileName);
                fileStream.AutoFlush = true;
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}:{1}: Error", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;    
            }
            Dictionary<string, string> cmdLineArgs = getCmdStr(args);
            IPCXUS pcxus;
            //создаём объект для платы
            if(cmdLineArgs.ContainsKey("EMUL"))
                pcxus = new PCXUSEMUL();
            else
                pcxus = new PCXUS();
            log.add(LogRecord.LogReason.info, "{0}: {1}: Директория с файлами настроек: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, uspcDir);
            log.add(LogRecord.LogReason.info, "{0}: {1}: Файл настроек: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, configName);
            log.add(LogRecord.LogReason.info, "Program started...");
            try
            {
                //Запускаем сервер
                PCXUSNetworkServer server = new PCXUSNetworkServer(pcxus);
                server.start();
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, "server started");
                while (!Console.KeyAvailable) ;
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}:{1}: Error", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                log.onLogChanged = null;
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        static Dictionary<string, string> getCmdStr(string[] args)
        {
            Dictionary<string, string> cmdStr = new Dictionary<string, string>();
            if ((args == null) || args.Length < 1)
            {
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
