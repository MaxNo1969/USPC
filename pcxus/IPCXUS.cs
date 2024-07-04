using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USPC
{
    public interface IPCXUS
    {
        void setParamValueDouble(double _val, string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us);
        double getParamValueDouble(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us);
        string getParamValueString(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us);
        bool open(Int32 _mode);
        bool load(string _fName, int _board = -1, int _test = -1);
        bool close();
        bool save(string _fName, int _board = -1, int _test = -1);
        bool config(Int32 _board, Int32 _bufferSize, Int32 _interruptFluidity);
        bool status(Int32 _board, ref Int32 _status, ref Int32 _NumberOfScansAcquired, ref Int32 _NumberOfScansRead, ref Int32 _BufferSize, ref Int32 _scanSize);
        bool start(Int32 _board);
        bool stop(Int32 _board);
        bool clear(Int32 _board);
        Int32 read(Int32 _board, ref AcqAscan[] _data, int _timeout = 200);
        bool readAscan(int _board, int _test, ref Ascan ascan, int _timeout);
        int Err { get; }
    }
}
