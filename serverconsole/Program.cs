using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;

namespace USPC
{
    class Program
    {
        public static Dictionary<string, string> cmdLineArgs = null;
        static void Main(string[] args)
        {
            log.onLogChanged += new log.OnLogChanged(() => Console.WriteLine(log.get().text));
            log.add(LogRecord.LogReason.info, "Program started...");
            Dictionary<string, string> cmdLineArgs = getCmdStr(args);
            IPCXUS pcxus;
            //создаём объект для платы
            if(cmdLineArgs.ContainsKey("EMUL"))
                pcxus = new PCXUSEMUL();
            else
                pcxus = new PCXUS();

            //Запускаем сервер
            PCXUSNetworkServer server = new PCXUSNetworkServer(pcxus);
            server.start();
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, "server started");
            while (!Console.KeyAvailable) ;
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
