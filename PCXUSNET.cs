using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            int error = netClient.callNetworkFunction(cmd,out obj);
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



        public bool open(Int32 _mode)
        {
            int err = netClient.callNetworkFunction(string.Format("{0},{1}", "open", _mode), out obj);
            return (err == 0);
        }


        public bool close()
        {
            int err = netClient.callNetworkFunction(string.Format("{0}", "close"), out obj);
            return (err == 0);
        }

        public bool load(string _fName, int _board = -1, int _test = -1)
        {
            int err = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "load",_fName,_board,_test), out obj);
            return (err == 0);
        }

        public bool save(string _fName, int _board = -1, int _test = -1)
        {
            int err = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "save", _fName, _board, _test), out obj);
            return (err == 0);
        }

        public bool config(Int32 _board, Int32 _bufferSize)
        {
            int err = netClient.callNetworkFunction(string.Format("{0},{1},{2},{3}", "config", _bufferSize), out obj);
            return (err == 0);
        }

        public bool status(Int32 _board, ref Int32 _status, ref Int32 _NumberOfScansAcquired, ref Int32 _NumberOfScansRead, ref Int32 _BufferSize, ref Int32 _scanSize)
        {
            return true;
        }

        public bool start(Int32 _board)
        {
            return true;
        }

        public bool stop(Int32 _board)
        {
            return true;
        }
        public bool clear(Int32 _board)
        {
            return true;
        }
        public Int32 read(Int32 _board, byte[] _data, int _timeout = 200)
        {
            return 0;
        }
        public Int32 read(Int32 _board, AcqAscan[] _data, int _timeout = 200)
        {
            return 0;
        }

        public bool readAscan(ref Ascan ascan, int _timeout = 100, int _board = 0, int _test = 0)
        {
            return true;
        }

    }
}
