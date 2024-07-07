using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using PROTOCOL;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace USPC
{
    class PCXUSNetworkClient
    {
        public static int port = 63001;
        string address = null;
        const int CMD_SIZE = 256;
        public PCXUSNetworkClient(string _serverAddr)
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name); 
            address = _serverAddr;
        }
        ~PCXUSNetworkClient()
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        public int callNetworkFunction(string _func, out Object ret)
        {
            TcpClient client = null;
            NetworkStream stream = null;
            try
            {
                ret = null;
                // Инициализация
                client = new TcpClient(address, port);
                if (_func != null && _func != "")
                {
                    Byte[] data = Encoding.UTF8.GetBytes(_func);
                    stream = client.GetStream();
                    // Отправка сообщения
                    stream.Write(data, 0, data.Length);
                    // Получение ответа
                    Byte[] bytesResult = new Byte[sizeof(Int32)];
                    int numberOfBytesRead = stream.Read(bytesResult, 0, bytesResult.Length);
                    Int32 result = BitConverter.ToInt32(bytesResult, 0);
                    //Ошибка
                    if (result != 0)
                    {
                        ret = null;
                        return result;
                    }
                    string[] funcAndArgs = _func.Split(new char[] { ',' });
                    switch (funcAndArgs[0])
                    {
                        case "read":
                            {
                                IFormatter formatter = new BinaryFormatter();
                                AcqAscan[] scans = (AcqAscan[])formatter.Deserialize(stream);
                                //log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\", {3} scans readed", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func, scans.Length);
                                ret = (Object)scans;
                                return result;
                            }
                        case "readdouble":
                            {
                                Byte[] doubleBytes = new byte[sizeof(double)];
                                numberOfBytesRead = stream.Read(doubleBytes, 0, doubleBytes.Length);
                                double retval = BitConverter.ToDouble(doubleBytes, 0);
                                log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\", {3} = {4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func, funcAndArgs[0], retval);
                                ret = (Object)retval;
                                return result;
                            }
                        case "readstring":
                            {
                                Byte[] stringBytes = new byte[CMD_SIZE];
                                numberOfBytesRead = stream.Read(stringBytes, 0, stringBytes.Length);
                                string retval = Encoding.UTF8.GetString(stringBytes).Trim(new char[] { '\0' });
                                log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\", {3} = {4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func, funcAndArgs[0], retval);
                                ret = (Object)retval;
                                return result;
                            }
                        case "ascan":
                            {
                                IFormatter formatter = new BinaryFormatter();
                                Ascan ascan = (Ascan)formatter.Deserialize(stream);
                                //log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\", DataSize = {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func, ascan.DataSize);
                                ret = (Object)ascan;
                                return result;
                            }
                        case "status":
                            {
                                IFormatter formatter = new BinaryFormatter();
                                AcqSatus status = (AcqSatus)formatter.Deserialize(stream);
                                //log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\", {3}, ", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func, ((ACQ_STATUS)status.status).ToString());
                                ret = (Object)status;
                                return result;
                            }
                        default:
                            {
                                log.add(LogRecord.LogReason.debug, "{0}: {1}: command = \"{2}\"", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _func);
                                return result;
                            }
                    }
                }
                else
                    throw new ArgumentException("Вызов пустой функции");
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Error:", ex.Message);
                if (stream != null) stream.Close();
                if (client != null) client.Close();
                ret = null;
                return -2;
            }
            finally
            {
                if (stream != null) stream.Close();
                if (client != null) client.Close();
            }
        }
    }
}
