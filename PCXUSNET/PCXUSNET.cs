using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PROTOCOL;
using Settings;

namespace USPC
{
    class PCXUSNET:IPCXUS
    {
        PCXUSNetworkClient netClient;
        Object obj;

        int error = 0;
        public int Err { get { return error; } }

        public PCXUSNET(string _serverAddr)
        {
            netClient = new PCXUSNetworkClient(_serverAddr);
            obj = new Object();
        }

        public void setParamValueDouble(double _val, string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            string strValDotted = string.Format("{0}",_val).Replace(',','.');
            netClient.callNetworkFunction(string.Format("{0},{1},{2},{3},{4}", "writedouble", _paramName, _board, _test), out obj);
        }



        public double getParamValueDouble(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            double ret = 0;
            string cmd =string.Format("{0},{1},{2},{3}","readdouble",_paramName,_board,_test);
            error = netClient.callNetworkFunction(cmd,out obj);
            if (error == 0)
            {
                ret = (double)obj;
            }
            else
            {
            }
            return ret;
        }

        public string getParamValueString(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            return "string";
        }

        public static double GetVal(int _board, int _channel, string _paramName)
        {
            PCXUSNetworkClient client = new PCXUSNetworkClient(AppSettings.s.serverAddr);
            Object retval = new Object();
            string s = string.Format("{0},{1},{2},{3}", "readdouble", _paramName, _board, _channel);
            int ret = client.callNetworkFunction(s, out retval);
            if (ret == 0)
                return (double)retval;
            else
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: callNetworkFunction(\"{2}\") returns {3}", "FRTestAscaNet", System.Reflection.MethodBase.GetCurrentMethod().Name, s, ret);
                return 0;
            }
        }


        public bool open(Int32 _mode)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1}", "open", _mode), out obj);
            if (error == 0)
            {
                Program.boardState = BoardState.Opened;
                return true;
            }
            else
            {
                Program.boardState = BoardState.NotOpened;
                return false;
            }
        }


        public bool close()
        {
            error = netClient.callNetworkFunction(string.Format("{0}", "close"), out obj);
            if (error == 0)
            {
                Program.boardState = BoardState.NotOpened;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool load(string _fName, int _board = -1, int _test = -1)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "load",_fName,_board,_test), out obj);
            return (error == 0);
        }

        public bool save(string _fName, int _board = -1, int _test = -1)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "save", _fName, _board, _test), out obj);
            return (error == 0);
        }

        public bool config(Int32 _board, Int32 _bufferSize, Int32 _interruptFluidity)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "config", _board,_bufferSize,_interruptFluidity), out obj);
            return (error == 0);
        }

        public bool status(Int32 _board, ref Int32 _status, ref Int32 _NumberOfScansAcquired, ref Int32 _NumberOfScansRead, ref Int32 _BufferSize, ref Int32 _scanSize)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1}", "status", _board), out obj);
            if (error == 0)
            {
                if (obj != null)
                {
                    AcqSatus status = (AcqSatus)obj;
                    _status = status.status;
                    _NumberOfScansAcquired = status.NumberOfScansAcquired;
                    _NumberOfScansRead = status.NumberOfScansRead;
                    _BufferSize = status.bufferSize;
                    _scanSize = status.scanSize;
                    return true;
                }
            }
            return false;
        }

        public bool start(Int32 _board)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1}", "start", _board), out obj);
            return true;
        }

        public bool stop(Int32 _board)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1}", "stop", _board), out obj);
            return true;
        }
        public bool clear(Int32 _board)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1}", "clear", _board), out obj);
            return true;
        }

        public Int32 read(Int32 _board, ref AcqAscan[] _data, int _timeout = 200)
        {
            int numberScans = netClient.callNetworkFunction(string.Format("{0},{1}", "read", _board), out obj);
            AcqAscan[] tempAscans = (AcqAscan[])obj;
            _data = tempAscans;
            error = 0;
            return numberScans;
        }

        public bool readAscan(int _board, int _test,ref Ascan ascan,int _timeout)
        {
            error = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "ascan", _board, _test, _timeout), out obj);
            if(obj != null) ascan = (Ascan)obj;
            return (error == 0);
        }

    }
}
