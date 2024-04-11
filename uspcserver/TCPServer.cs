using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using PROTOCOL;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace USPC
{
    public delegate void StreamWork(Stream _stream);
    class TCPServer
    {
        IPAddress address = IPAddress.Any;
        public static int port = 63001;
        TcpListener server = null;
        bool isRunning = false;
        AsyncCallback acceptCallback = null;
        public StreamWork streamWork = null;
        const int CMD_SIZE = 512;

        public PCXUS pcxus = null;

        //ToDo: ИП вроде не нужен - пускай слушает всё
        public TCPServer()
        {
            address = IPAddress.Any;
            isRunning = false;
            //streamWork += new StreamWork(defaultStreamWorkHandler);
        }
        public void start()
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            acceptCallback = new AsyncCallback(OnAcceptTCPClient);
            try
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Start listening");
                server = new TcpListener(address, port);
                server.Start();
                isRunning = true;
                server.BeginAcceptTcpClient(acceptCallback, null);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Error:", ex.Message);
            }
        }
        public void stop()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (server != null)
            {
                isRunning = false;
                server.Stop();
            }
        }
        ~TCPServer()
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        //ToDo: Пока чтение и запись в Stream сделаны синхронно (может так и оставлю)
        void OnAcceptTCPClient(IAsyncResult res)
        {
            if (!isRunning) return;
            TcpClient client = server.EndAcceptTcpClient(res);
            NetworkStream stream = client.GetStream();
            // Обмен данными
            try
            {
                if (streamWork != null) streamWork(stream);
            }
            catch(Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Error:", ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
                server.BeginAcceptTcpClient(acceptCallback,null);            
            }
        }
        //Делегат по умолчанию для обработки входящих соединений - просто возвращает назад полученные данные        
        private void defaultStreamWorkHandler(Stream _stream)
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (_stream.CanRead)
            {
                byte[] buffer = new byte[CMD_SIZE];
                int bytesReaded = _stream.Read(buffer, 0, CMD_SIZE);
                if (_stream.CanWrite)
                {
                    _stream.Write(buffer, 0, bytesReaded);
                }
            }
        }
    }
}
