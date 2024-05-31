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
                    Int32 result = BitConverter.ToInt32(bytesResult,0);
                    if (result > 0 && _func == "read")
                    {
                        IFormatter formatter = new BinaryFormatter();
                        AcqAscan[] scans = (AcqAscan[])formatter.Deserialize(stream);
                        ret = (Object)scans;
                        return result;
                    }
                    if (result != 0)
                    {
                        log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,_func,((ErrorCode)result).ToString());
                        return result;
                    }
                    string[] funcAndArgs = _func.Split(new char[] {','});
                    //Особая обработка если функция должна возвратить что-то ещё кроме ошибки
                    if (funcAndArgs[0] == "readdouble")
                    {
                        Byte[] doubleBytes = new byte[sizeof(double)];
                        numberOfBytesRead = stream.Read(doubleBytes, 0, doubleBytes.Length);
                        double retval = BitConverter.ToDouble(doubleBytes, 0);
                        ret = (Object)retval;
                    }
                    //Особая обработка если функция должна возвратить что-то ещё кроме ошибки
                    if (funcAndArgs[0] == "readstring")
                    {
                        Byte[] stringBytes = new byte[CMD_SIZE];
                        numberOfBytesRead = stream.Read(stringBytes, 0, stringBytes.Length);
                        string retval = Encoding.UTF8.GetString(stringBytes).Trim(new char[] {'\0'});
                        ret = (Object)retval;
                    }
                    //Особая обработка если функция должна возвратить что-то ещё кроме ошибки
                    if (funcAndArgs[0] == "ascan")
                    {
                        IFormatter formatter = new BinaryFormatter();
                        //XmlSerializer formatter = new XmlSerializer(typeof(Ascan));
                        Ascan ascan = (Ascan)formatter.Deserialize(stream);
                        ret = (Object)ascan;
                    }
                    //Особая обработка если функция должна возвратить что-то ещё кроме ошибки
                    if (funcAndArgs[0] == "status")
                    {
                        IFormatter formatter = new BinaryFormatter();
                        AcqSatus status = (AcqSatus)formatter.Deserialize(stream);
                        ret = (Object)status;
                    }
                    return result;
                }
                else
                    return -1;
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
