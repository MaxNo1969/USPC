using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using PROTOCOL;

namespace USPC
{
    class TcpCommand
    {

        public static int port = 63001;
        string address = null;
        int bufferSize = 1024;

        public TcpCommand(string _addr)
        {
            address = _addr;
        }

        public byte[] sendCommand(string _cmd)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                // Инициализация
                client = new TcpClient(address, port);
                if (_cmd != null && _cmd != "")
                {
                    Byte[] data = Encoding.UTF8.GetBytes(_cmd);
                    stream = client.GetStream();
                    // Отправка сообщения
                    stream.Write(data, 0, data.Length);
                    // Получение ответа
                    Byte[] readingData = new Byte[bufferSize];
                    int numberOfBytesRead = 0;
                    do
                    {
                        numberOfBytesRead = stream.Read(readingData, 0, readingData.Length);
                    }
                    while (stream.DataAvailable);
                    return readingData;
                }
                else
                    return Encoding.UTF8.GetBytes("Empty command.");
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "TcpCommand: senDCommand: Error: {0}", ex.Message);
                if (stream != null) stream.Close();
                if (client != null) client.Close();
                return null;
            }
            finally
            {
                if(stream != null)stream.Close();
                if(client != null)client.Close();
            }
        }
    }

    class TCPServer
    {
        IPAddress address = IPAddress.Any;
        public static int port = 63001;
        TcpListener server = null;
        int bufferSize = 1024;
        bool isRunning = false;
        AsyncCallback acceptCallback = null;
        public PCXUS pcxus = null;

        //ToDo: ИП вроде не нужен - пускай слушает всё
        public TCPServer()
        {
            address = IPAddress.Any;
            isRunning = false;
        }
        public void start()
        {
            log.add(LogRecord.LogReason.info, "TCPServer: start()");
            acceptCallback = new AsyncCallback(ProcessAcceptClient);
            try
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "TCPServer", "TCPServer", "Start listening");
                server = new TcpListener(address, port);
                server.Start();
                isRunning = true;
                server.BeginAcceptTcpClient(acceptCallback, null);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", "TCPServer", "TCPServer", ex.Message);
            }
        }
        public void stop()
        {
            log.add(LogRecord.LogReason.info, "TCPServer: stop()");
            if (server != null)
            {
                isRunning = false;
                server.Stop();
            }
        }
        ~TCPServer()
        {
            log.add(LogRecord.LogReason.info, "TCPServer: ~TCPServer()");
        }

        private byte[] serverResponce(string _cmd)
        {
            string[] cmdAndPars = _cmd.Split(new char[] {','});
            byte[] ret = null;
            switch (cmdAndPars[0])
            {
                case "TEST":
                    ret = Encoding.UTF8.GetBytes(cmdAndPars[0]);
                    break;
                case "PCXUS_OPEN":
                    {
                        if (cmdAndPars.Count() != 3)
                        {
                            log.add(LogRecord.LogReason.error, "PCXUS_OPEN: wrong params");
                            ret = Encoding.UTF8.GetBytes("PCXUS_OPEN: wrong params");
                            break;
                        }
                        int hPCXUS = Convert.ToInt32(cmdAndPars[1]);
                        int boot = Convert.ToInt32(cmdAndPars[2]);
                        pcxus.open(boot);
                        ret = Encoding.UTF8.GetBytes("PCXUS_OPEN: board opened");
                    }
                    break;
                case "PCXUS_LOAD":
                    {
                        if (cmdAndPars.Count() != 4)
                        {
                            log.add(LogRecord.LogReason.error, "PCXUS_LOAD: wrong params");
                            ret = Encoding.UTF8.GetBytes("PCXUS_LOAD: wrong params");
                            break;
                        }
                        int board = Convert.ToInt32(cmdAndPars[1]);
                        int test = Convert.ToInt32(cmdAndPars[2]);
                        string fName = cmdAndPars[3];
                        string configPath = @"c:\uspc\UT_files";
                        fName = string.Format(@"{0}\{1}", configPath, fName);
                        pcxus.load(fName);
                        ret = Encoding.UTF8.GetBytes(string.Format("PCXUS_LOAD: Configuration \"{0}\" loaded",fName));
                    }
                    break;
                case "PCXUS_CLOSE":
                    {
                        if (cmdAndPars.Count() != 1)
                        {
                            log.add(LogRecord.LogReason.error, "PCXUS_CLOSE: wrong params");
                            ret = Encoding.UTF8.GetBytes("PCXUS_OPEN: wrong params");
                            break;
                        }
                        pcxus.close();
                        ret = Encoding.UTF8.GetBytes("PCXUS_OPEN: board closed");
                    }
                    break;
                case "PCXUS_ACQ_ASCAN":
                    {
                        if (cmdAndPars.Count() != 4)
                        {
                            log.add(LogRecord.LogReason.error, "PCXUS_ACQ_ASCAN: wrong params");
                            ret = Encoding.UTF8.GetBytes("PCXUS_ACQ_ASCAN: wrong params");
                            break;
                        }
                        int board = Convert.ToInt32(cmdAndPars[1]);
                        int test = Convert.ToInt32(cmdAndPars[2]);
                        int _ascan = Convert.ToInt32(cmdAndPars[3]);
                        int timeout = Convert.ToInt32(cmdAndPars[4]);
                        PCXUS.Ascan ascan = new PCXUS.Ascan();
                        PCXUS.PCXUS_ACQ_ASCAN(board, test, ref ascan, timeout);                        
                        //byte[] bytes =                         
                    }
                    break;

                default:
                    ret = Encoding.UTF8.GetBytes(cmdAndPars[0]);
                    break;

            }
            return ret;
        }

        void ProcessAcceptClient(IAsyncResult res)
        {
            if (!isRunning) return;
            TcpClient client = server.EndAcceptTcpClient(res);
            NetworkStream stream = client.GetStream();
            // Обмен данными
            try
            {
                if (stream.CanRead)
                {
                    byte[] myReadBuffer = new byte[bufferSize];
                    int numberOfBytesRead = 0;
                    StringBuilder cmd = new StringBuilder();
                    do
                    {
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        cmd.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);
                    log.add(LogRecord.LogReason.info, "TCPServer: ProcessAcceptClient: received command: {0}", cmd);
                    Byte[] responseData = serverResponce(cmd.ToString());
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch(Exception ex)
            {
                log.add(LogRecord.LogReason.error,"TCPServer: ProcessAcceptClient: Error: {0}", ex.Message);
            }
            finally
            {
                //log.add(LogRecord.LogReason.info, "TCPServer: ProcessAcceptClient: finally");
                stream.Close();
                client.Close();
                server.BeginAcceptTcpClient(acceptCallback,null);            
            }
        }
    }
}
