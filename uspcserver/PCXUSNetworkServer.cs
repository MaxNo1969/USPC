﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PROTOCOL;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace USPC
{
    class PCXUSNetworkServer
    {
        const int CMD_SIZE = 512;
        const int bufferSize = 1024 * 100;
        static AcqAscan[] data = new AcqAscan[bufferSize];

        TCPServer server = null;
        PCXUS pcxus = null;
        StreamWork onFunctionRequested;

        public PCXUSNetworkServer(PCXUS _pcxus)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            pcxus = _pcxus;
        }

        ~PCXUSNetworkServer()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        public void start()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            server = new TCPServer();
            onFunctionRequested = new StreamWork(completeFunctionRequest);
            server.streamWork += onFunctionRequested;
            server.start();
        }

        public void stop()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            server.streamWork -= onFunctionRequested;
            server.stop();
        }

        private int ConvertToInt(string _str, int _default = 0)
        {
            int ret = _default;
            try
            {
                ret = Convert.ToInt32(_str);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: Error {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _str, ex.Message);
            }
            return ret;
        }

        public void completeFunctionRequest(Stream _stream)
        {
            //log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (_stream.CanRead)
            {
                byte[] buffer = new byte[CMD_SIZE];
                int bytesReaded = _stream.Read(buffer, 0, CMD_SIZE);
                string strReadedFromStream = Encoding.UTF8.GetString(buffer);
                strReadedFromStream = strReadedFromStream.Trim(new char[] {'\0'});
                string[] cmdAndParams = strReadedFromStream.Split(new char[] { ',' });
                cmdAndParams[0] = cmdAndParams[0].ToLower();
                if (cmdAndParams[0] != "ascan")
                {
                    if (cmdAndParams.Length < 1)
                    {
                        log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "empty command");
                        return;
                    }
                    if (cmdAndParams.Length < 2)
                        log.add(LogRecord.LogReason.info, "{0}: {1}: {2}={3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Command", cmdAndParams[0]);
                    else
                        log.add(LogRecord.LogReason.info, "{0}: {1}: {2}={3},{4}={5}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Command", cmdAndParams[0], "Params", string.Join(",", cmdAndParams.Skip(1)));
                }
                UInt32 ret = 0;
                switch (cmdAndParams[0])
                {
                    case "open":
                        {
                            int boot = ((cmdAndParams.Count() > 1))?ConvertToInt(cmdAndParams[1],0):0;
                            ret = (pcxus.open(boot)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    case "close":
                        {
                            ret = (pcxus.close())?0:(UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    case "load":
                        {
                            string configPath = @"c:\uspc\UT_files";
                            string fName = (cmdAndParams.Count() > 1)?cmdAndParams[1]:"default.us";
                            fName = configPath + "\\" + fName;
                            int board = (cmdAndParams.Count() > 2) ? ConvertToInt(cmdAndParams[2], -1) : -1;
                            int test = (cmdAndParams.Count() > 3) ? ConvertToInt(cmdAndParams[2], -1) : -1;
                            ret = (pcxus.load(fName, board, test)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(Int32));
                            _stream.Close();
                            return;
                        }
                        
                    case "save":
                        {
                            string configPath = @"c:\uspc\UT_files";
                            string fName = (cmdAndParams.Count() > 1) ? cmdAndParams[1] : "default.us";
                            fName = configPath + "\\" + fName;
                            int board = (cmdAndParams.Count() > 2) ? ConvertToInt(cmdAndParams[2], -1) : -1;
                            int test = (cmdAndParams.Count() > 3) ? ConvertToInt(cmdAndParams[2], -1) : -1;
                            ret = (pcxus.save(fName, board, test)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(Int32));
                            _stream.Close();
                            return;
                        }
                    case "readdouble":
                        {
                            double value = pcxus.getParamValueDouble(cmdAndParams[1]);
                            ret = (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(Int32));
                            byte[] byteArray = BitConverter.GetBytes(value);
                            _stream.Write(byteArray, 0, byteArray.Length);
                            _stream.Close();
                            return;
                        }
                    case "readstring":
                        {
                            string value = pcxus.getParamValueString(cmdAndParams[1]);
                            ret = (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(Int32));
                            byte[] byteArray = Encoding.UTF8.GetBytes(value);
                            _stream.Write(byteArray, 0, byteArray.Length);
                            _stream.Close();
                            return;
                        }
                    case "ascan":
                        {
                            const int defTimeout = 20;
                            int board = (cmdAndParams.Length > 2) ? ConvertToInt(cmdAndParams[1]) : 0;
                            int test = (cmdAndParams.Length > 3) ? ConvertToInt(cmdAndParams[2]) : 0;
                            int timeout = (cmdAndParams.Length > 4) ? ConvertToInt(cmdAndParams[3],defTimeout) : defTimeout;
                            Ascan ascan = new Ascan();
                            int counter = 0;
                            while (!pcxus.readAscan(ref ascan, timeout,board,test) || counter++ < 5) ;
                            ret = (UInt32)pcxus.Err;
                            byte[] byteArray = BitConverter.GetBytes(ret);
                            _stream.Write(byteArray, 0, byteArray.Length);
                            if (ret == 0)
                            {
                                IFormatter formatter = new BinaryFormatter();
                                //XmlSerializer formatter = new XmlSerializer(typeof(Ascan));
                                formatter.Serialize(_stream, ascan);
                            }
                            _stream.Close();
                            return;
                        }
                    case "config":
                        {
                            int board = (cmdAndParams.Length>2)?ConvertToInt(cmdAndParams[1]):0;
                            //int bufferSize = (cmdAndParams.Length>3)?ConvertToInt(cmdAndParams[2]):bufferSize;
                            ret = (pcxus.config(board,bufferSize)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    case "status":
                        {
                            int board = (cmdAndParams.Length>2)?ConvertToInt(cmdAndParams[1]):0;
                            AcqSatus packet = new AcqSatus();
                            Int32 status = (Int32)ACQ_STATUS.ACQ_NO_CONFIGURED;
                            Int32 NumberOfScansAcqured = 0;
                            Int32 NumberOfScansRead = 0;
                            Int32 bufferSize = 0;
                            Int32 scanSize = 0;
                            ret = (pcxus.status(board,ref status,ref NumberOfScansAcqured,ref NumberOfScansRead,ref bufferSize,ref scanSize)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            packet.status = status;
                            packet.NumberOfScansAcquired = NumberOfScansAcqured;
                            packet.NumberOfScansRead = NumberOfScansRead;
                            packet.bufferSize = bufferSize;
                            packet.scanSize = scanSize;
                            IFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(_stream, packet);
                            _stream.Close();
                            return;
                        }
                    case "start":
                        {
                            int board = (cmdAndParams.Length > 2) ? ConvertToInt(cmdAndParams[1]) : 0;
                            int NumberOfScansToAcquire = (cmdAndParams.Length > 3) ? ConvertToInt(cmdAndParams[2]) : -1;
                            ret = (pcxus.start(board)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    case "read":
                        {
                            int board = (cmdAndParams.Length > 2) ? ConvertToInt(cmdAndParams[1]) : 0;
                            ret = (uint)pcxus.read(board, data);
                            ret = (ret > 0) ? ret : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            AcqAscan[] retarray = new AcqAscan[ret];
                            Array.Copy(data, retarray, ret);
                            log.add(LogRecord.LogReason.debug, "retarray.Length = {0}", retarray.Length);
                            IFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(_stream, retarray);
                            _stream.Close();
                            return;
                        }
                    case "stop":
                        {
                            int board = (cmdAndParams.Length > 2) ? ConvertToInt(cmdAndParams[1]) : 0;
                            ret = (pcxus.stop(board)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    case "clear":
                        {
                            int board = (cmdAndParams.Length > 2) ? ConvertToInt(cmdAndParams[1]) : 0;
                            ret = (pcxus.clear(board)) ? 0 : (UInt32)pcxus.Err;
                            _stream.Write(BitConverter.GetBytes(ret), 0, sizeof(UInt32));
                            _stream.Close();
                            return;
                        }
                    default:
                        {
                            ret = (UInt32)ErrorCode.PCXUS_UNKNOWN_ERROR;
                            byte[] byteArray = BitConverter.GetBytes(ret);
                            _stream.Write(byteArray, 0, byteArray.Length);
                            _stream.Close();
                            return;
                        }
                }
            }
        }
    }
}
