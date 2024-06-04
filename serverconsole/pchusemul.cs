using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using PROTOCOL;
using System.Diagnostics;
using System.IO;

namespace USPC
{
    public class PCXUSEMUL:IPCXUS
    {
      
        //public const int MAX_ROW = 100;
        //public const int MAX_STRING = 256;

        private Dictionary<string, double> boardParams;
        private ACQ_STATUS boardStatus = ACQ_STATUS.ACQ_NO_CONFIGURED;
        private Int32 hPCXUS = 0;
        private int error = 0;

        AcqAscan[] scans = null;
        int bufferSize = 0;
        int numberOfScans = 0;
        
        //Серийные номера для плат
        int[] sn = {5555,6666};
        
        //работает после открытия платы
        public int numBoards
        {
            get
            {
                return 2;
            }
        }

        public int Err
        {
            get
            {
                return error;
            }
        }
        
        //работает после открытия платы
        public int serialNumber(int _board = 0)
        {
            return sn[_board];
        }
        public PCXUSEMUL()
        {
            boardParams = new Dictionary<string, double>();
            using (StreamReader reader = new StreamReader("default.us", false))
            {
                string s;
                //Читаем заголовок
                for (int i = 0; i < 22; i++)
                {
                    s = reader.ReadLine();
                }
                while ((s = reader.ReadLine()) != null)
                {
                   string[] splitted = s.Split(new char[] { '=' });
                   try
                   {
                       double val = Convert.ToDouble(splitted[1].Replace('.',','));
                       boardParams.Add(splitted[0], val);
                   }
                   catch (Exception ex)
                   {
                       log.add(LogRecord.LogReason.error, "{0}: {1}: Ошибка: {2}", "pcxusemul", ".ctor", ex.Message);
                   }
                }
                reader.Close();
                log.add(LogRecord.LogReason.info, "{0}: {1}: Считано {2} параметров", "pcxusemul", ".ctor", boardParams.Count);
            }

        }

        ~PCXUSEMUL()
        {
            boardParams.Clear();
            close();
        }

        private bool checkHandle()
        {
            if (hPCXUS == 0)
            {
                log.add(LogRecord.LogReason.error, "board not open: hPCXUS = {0}", hPCXUS);
                error = (int)ErrorCode.PCXUS_INVALID_HANDLE_ERROR;
                return false;
            }
            else
                return true;
        }

        public void setParamValueDouble(double _val,string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            try
            {
                boardParams[_paramName] = _val;
            }
            catch (KeyNotFoundException)
            {
                boardParams.Add(_paramName, _val);
            }
            return;
        }



        public double getParamValueDouble(string _paramName,int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us )
        {
            try
            {
                log.add(LogRecord.LogReason.info, string.Format("{0}: {1}: {2}={3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _paramName, boardParams[_paramName]));
                return boardParams[_paramName];
            }
            catch (KeyNotFoundException ex)
            {
                log.add(LogRecord.LogReason.error,string.Format("{0}: {1}: Error: {2}",GetType().Name,System.Reflection.MethodBase.GetCurrentMethod().Name,ex.Message));
                return 0;
            }
        }

        public string getParamValueString(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            return "string";
        }



        public bool open(Int32 _mode)
        {
            if (hPCXUS != 0)
            {
                error = (int)ErrorCode.PCXUS_ALREADY_OPEN;
                log.add(LogRecord.LogReason.warning, "{0}: {1}: Предупреждение: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Плата уже открыта");
                return false;
            }
            hPCXUS = 1;
            return true;
        }
        
        
        public bool close()
        {
            if (!checkHandle()) return false;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            hPCXUS = 0;
            return true;
        }

        public bool load(string _fName, int _board = -1, int _test = -1)
        {
            if (!checkHandle()) return false;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool save(string _fName, int _board = -1, int _test = -1)
        {
            if (!checkHandle()) return false;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool config(Int32 _board, Int32 _bufferSize)
        {
            if(!checkHandle())return false;
            bufferSize = _bufferSize;
            scans = new AcqAscan[bufferSize];
            boardStatus = ACQ_STATUS.ACQ_WAITING_START;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool status(Int32 _board, ref Int32 _status, ref Int32 _NumberOfScansAcquired, ref Int32 _NumberOfScansRead, ref Int32 _BufferSize, ref Int32 _scanSize)
        {
            _status = (Int32)boardStatus;
            _scanSize = 8;
            _BufferSize = bufferSize;
            _NumberOfScansAcquired = numberOfScans;
            _NumberOfScansRead = numberOfScans;
            if (!checkHandle()) return false;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool start(Int32 _board)
        {
            if (!checkHandle()) return false;
            boardStatus = ACQ_STATUS.ACQ_RUNNING;
            scanCounter = 0;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;            
        }

        public bool stop(Int32 _board)
        {
            if (!checkHandle()) return false;
            boardStatus = ACQ_STATUS.ACQ_FINISHED_WITHOUT_SCANSBACKLOG;
            scanCounter = 0;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }
        public bool clear(Int32 _board)
        {
            if (!checkHandle()) return false;
            scans = null;
            scanCounter = 0;
            bufferSize = 0;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }
        uint scanCounter = 0;
        public Int32 read(Int32 _board, byte[] _data, int _timeout = 200)
        {
            if (!checkHandle()) return 0;
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return 0;
        }
        public Int32 read(Int32 _board, AcqAscan[] _data, int _timeout = 200)
        {
            if (!checkHandle()) return 0;
            if (boardStatus == ACQ_STATUS.ACQ_RUNNING && scans != null)
            {
                Random r = new Random();
                numberOfScans = 300 + r.Next(50);
                for (int i = 0; i < numberOfScans; i++)
                {
                    _data[i].Channel = 0;
                    _data[i].ScanCounter = scanCounter;
                    _data[i].PulserCounter = scanCounter;
                    scanCounter++;
                    _data[i].G1Amp = (byte)(90 + r.Next(10));
                    _data[i].G2Amp = (byte)(90 + r.Next(10));
                    double Distance = 3.53;
                    uint wt = (uint)(Distance * 1000.0 / 5.0);
                    _data[i].G1WTmsb = (byte)(wt & 0x00ff);
                    _data[i].G1WTlsb = (byte)(wt >> 16);
                    Distance = 5.1;
                    wt = (uint)(Distance * 1000.0 / 5.0);
                    _data[i].G2WTmsb = (byte)(wt & 0x00ff);
                    _data[i].G2WTlsb = (byte)(wt >> 16);
                }
            }
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name,numberOfScans);
            return numberOfScans;
        }

        public bool readAscan(ref Ascan ascan,int _timeout = 100, int _board = 0, int _test = 0)
        {
            Random r =new Random();
            double Distance = 0.23;
            ascan.G1TofWt = (uint)(Distance * 1000 / 5.0);
            Distance = 5.25;
            ascan.G2TofWt = (uint)(Distance * 1000 / 5.0);
            Distance = 3.11;
            ascan.GIFTof = (uint)(Distance * 1000 / 5.0);
            ascan.G1Begin = 200;
            ascan.G2Begin = 500;
            ascan.G1End = 500;
            ascan.G2End = 800;
            ascan.G1Level = 60;
            ascan.G2Level = 80;
            ascan.G1Amp = (byte)(90 + r.Next(10));
            ascan.G2Amp = (byte)(90 + r.Next(10));
            ascan.DataSize = (ushort)(200+r.Next(20));
            ascan.Points = new byte[ascan.DataSize];
            for (int i = 0; i < ascan.DataSize; i++)
            {
                ascan.Points[i] = (byte)(50 + r.Next(50));
            }
            log.add(LogRecord.LogReason.info, "{0}: {1}: Board={2},Test={3} DataSize={4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _board,_test, ascan.DataSize);
            return true;
        }

        public AscanInfo GetAscanInfo(int _board, int _channel)
        {
            AscanInfo info = new AscanInfo();

            // Part 1.  This part gets parameters to display Ascan according to: 
            //          - the scope video mode
            //          - the scope zero calibration
            //          To display gates according to:
            //          - the wave alternance selection (phase)   
            
            info.Video = (AscanInfo.VideoMode)getParamValueDouble("scope_video",_board,_channel);
            info.ZeroVideo = getParamValueDouble("scope_zero", _board, _channel);
            info.GIFPhase = (AscanInfo.PhaseType)getParamValueDouble("gateIF_phase",_board,_channel);
            info.G1Phase = (AscanInfo.PhaseType)getParamValueDouble("gate1_phase", _board, _channel);
            info.G2Phase = (AscanInfo.PhaseType)getParamValueDouble("gate2_phase", _board, _channel);

            // Part 2.  This part gets parameters to convert Ascan data coming from acquisition to Ascan structure ready to display 

            info.gate1_trigger = getParamValueDouble("gate1_trigger", _board, _channel);
            info.gate1_position = getParamValueDouble("gate1_position", _board, _channel);
            info.gate1_width = getParamValueDouble("gate1_width", _board, _channel);
            info.gate1_level = getParamValueDouble("gate1_level", _board, _channel);
            info.gate1_level_alarm = getParamValueDouble("gate1_nb_alarm_level", _board, _channel);

            info.gate2_trigger = getParamValueDouble("gate2_trigger", _board, _channel);
            info.gate2_position = getParamValueDouble("gate2_position", _board, _channel);
            info.gate2_width = getParamValueDouble("gate2_width", _board, _channel);
            info.gate2_level = getParamValueDouble("gate2_level", _board, _channel);
            info.gate2_level_alarm = getParamValueDouble("gate2_nb_alarm_level", _board, _channel);

            info.gateIF_trigger = getParamValueDouble("gateIF_trigger", _board, _channel);
            info.gateIF_position = getParamValueDouble("gateIF_position", _board, _channel);
            info.gateIF_width = getParamValueDouble("gateIF_width", _board, _channel);
            info.gateIF_level = getParamValueDouble("gateIF_level", _board, _channel);

            info.scope_trigger = getParamValueDouble("scope_trigger", _board, _channel);
            info.scope_offset = getParamValueDouble("scope_offset", _board, _channel);
            info.scope_range = getParamValueDouble("scope_range", _board, _channel);
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            return info;
        }
    }
}
