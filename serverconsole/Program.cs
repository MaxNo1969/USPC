using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;
using USPC;

namespace serverconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PCXUSNetworkServer server = null;

            log.onLogChanged+= new log.OnLogChanged(()=>Console.WriteLine(log.get().text));
            log.add(LogRecord.LogReason.info, "Program started...");
            //создаём объект для платы
            pcxusemul pcxus = new pcxusemul();
            //Запускаем сервер
            server = new PCXUSNetworkServer(pcxus);
            server.start();
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "Program", System.Reflection.MethodBase.GetCurrentMethod().Name, "server started");
            while(!Console.KeyAvailable);          
        }
    }
}
