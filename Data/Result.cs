using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using PROTOCOL;
using Settings;
using System.Diagnostics;
using System.IO;
using USPC.Data;

namespace Data
{
    public class ListValues
    {
        private List<Ascan> _list = new List<Ascan>();
        public void Add(Ascan _val)
        {
            try
            {
                _list.Add(_val);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        public Ascan this[int _index]
        {
            get
            {
                return _list[_index];
            }
            set
            {
                _list[_index]=value;
            }
        }
        public int  Count
        {
            get
            {
                return _list.Count;
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
    public class ListSensors
    {
        private List<ListValues> _list = new List<ListValues>();
        public void Add(ListValues _val)
        {
            try
            {
                _list.Add(_val);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        public ListValues this[int _index]
        {
            get
            {
                return _list[_index];
            }
            set
            {
                _list[_index] = value;
            }
        }
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }
        public void Clear()
        {
            _list.Clear();
        }
    }

    public class ListZones
    {
        private List<ListSensors> _list = new List<ListSensors>();
        public void Add(ListSensors _val)
        {
            try
            {
                _list.Add(_val);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        public ListSensors this[int _index]
        {
            get
            {
                return _list[_index];
            }
            set
            {
                _list[_index] = value;
            }
        }
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }
        public void Clear()
        {
            _list.Clear();
        }
    }

    class Result
    {
        public int zone {get;private set;}
        List<int> offsets = new List<int>();
        
        public ListZones values = new ListZones();
        //Итоги в разрезе зона/датчик
        public double[][] zoneSensorResults;
        public List<int> zonesLengths = new List<int>();
        //Итоги по зоне
        public bool[] zoneResults;
        public void addDeadZoneStart()
        {
            int numberOfScans = Program.typeSize.currentTypeSize.deadZoneStart;
            ListSensors listSensors = new ListSensors();
            for (int sens = 0; sens < USPCData.countSensors; sens++)
            {
                listSensors.Add(new ListValues());
            }
            for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
            {
                for (int i = 0; i < numberOfScans; i++)
                {
                    listSensors[sensor].Add(notMeasuredAscan);
                }
                zoneSensorResults[zone][sensor] = Result.deadZone;
            }
            //zone++;
            zonesLengths.Add(Program.typeSize.currentTypeSize.deadZoneStart);
            values.Add(listSensors);
            Program.frMain.zbWorker_ProgressChanged(null, null);
        }
        public void addDeadZoneEnd()
        {
            int numberOfScans = Program.typeSize.currentTypeSize.deadZoneEnd; 
            ListSensors listSensors = new ListSensors();
            for (int sens = 0; sens < USPCData.countSensors; sens++)
            {
                listSensors.Add(new ListValues());
            }
            for (int numBoard = 0; numBoard < Program.numBoards; numBoard++)
            {
                for (int channel = 0; channel < USPCData.countSensors; channel++)
                {

                    for (int i = 0; i < numberOfScans; i++)
                        listSensors[channel].Add(deadAscan);
                    zoneSensorResults[zone][channel] = Result.deadZone;
                }
            }
            //zone++;
            zonesLengths.Add(Program.typeSize.currentTypeSize.deadZoneEnd);
            values.Add(listSensors);
            Program.frMain.zbWorker_ProgressChanged(null, null);
        }

        public void CalcZone(int _zone)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: _zone = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _zone);
            for (int board = 0; board < Program.numBoards; board++)
            {
                int sensorsCount = (board == 0) ? 4 : 8;
                for (int sensor = 0; sensor < sensorsCount; sensor++)
                {
                    int sensReal;
                    if(board==0)
                        sensReal = sensor;
                    else 
                        sensReal = sensor+4;
                    for (int meas = 0; meas < values[_zone][sensReal].Count; meas++)
                    {
                        try
                        {
                            double val = (sensReal<4)?ThickConverter.TofToMm(values[_zone][sensReal][meas].G1TofWt):values[_zone][sensReal][meas].G1Amp;
                            if (zoneSensorResults[_zone][sensReal] == notMeasured)
                                    zoneSensorResults[_zone][sensReal] = val;
                            else if (board == 0)
                            {
                                if (zoneSensorResults[_zone][sensReal] != Result.deadZone && val < zoneSensorResults[_zone][sensReal])
                                    zoneSensorResults[_zone][sensReal] = val;
                            }
                            else
                            {
                                if (val > zoneSensorResults[_zone][sensReal])
                                    zoneSensorResults[_zone][sensReal] = val;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                            log.add(LogRecord.LogReason.error, ex.StackTrace);
                        }
                    }
                }
            }
            zoneResults[zone] = true;
            for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
            {
                if (sensor < 4)
                {
                    if (DrawResults.IsBrakThick(zoneSensorResults[zone][sensor])) zoneResults[zone] = false;
                }
                else
                {
                    if (DrawResults.IsBrakDef(zoneSensorResults[zone][sensor])) zoneResults[zone] = false;
                }
            }
        }
        public void AddZone()
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: zone = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, zone);
            ListSensors listSensors = new ListSensors();
            for (int sens = 0; sens < USPCData.countSensors; sens++)
            {
                listSensors.Add(new ListValues());
            }
            values.Add(listSensors);
            zonesLengths.Add(AppSettings.s.zoneSize);
            zone++;
        }
        
        
        public const double notMeasured = 101.0;
        public const double deadZone = 102.0;
        public Ascan notMeasuredAscan;
        public Ascan deadAscan;
        public void ClearZoneSensorResult()
        {
            for (int z = 0; z < USPCData.countZones; z++)
            {
                zoneSensorResults[z] = new double[USPCData.countSensors];
                for (int s = 0; s < USPCData.countSensors; s++)
                    zoneSensorResults[z][s] = notMeasured;
                zoneResults[z] = false;
            }
        }
        public Result()
        {
            zone = 0;
            zoneSensorResults = new double[USPCData.countZones][];
            zoneResults = new bool[USPCData.countZones];
            notMeasuredAscan = new Ascan()
            {
                G1Amp = (byte)notMeasured,
            };
            deadAscan = new Ascan()
            {
                G1Amp = (byte)deadZone,
            };
            ClearZoneSensorResult();
        }
        public void Clear()
        {
            zone = 0;
            offsets.Clear();
            values.Clear();
            ClearZoneSensorResult();
        }


        internal bool GetTubeResult()
        {
            bool res = true;
            for (int z = 0; z < zone; z++)
            {
                if (zoneResults[z] == false)
                {
                    res = false;
                    break;
                }
            }
            return res;
        }
        public int GetDataSize()
        {
            int ret = 0;
            for (int z = 0; z < values.Count;z++ )
            {
                for (int ch = 0; ch < values[z].Count;ch++ )
                {
                    ret += values[z][ch].Count;
                }
            }
            return ret;
        }
        public void save(StreamWriter _writer)
        {
            int z =0;
            int ch =0;
            int meas=0;
            log.add(LogRecord.LogReason.info, "{0} {1} {2}", values.Count, values[0].Count, values[0][0].Count);
            try
            {
                for (z = 0; z < values.Count; z++)
                {
                    for (ch = 0; ch < values[z].Count; ch++)
                    {
                        for (meas = 0; meas < values[z][ch].Count; meas++)
                        {
                            _writer.WriteLine("{0};{1};{2};{3}", z, ch, meas, values[z][ch][meas]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                log.add(LogRecord.LogReason.error, "zone = {0} channel={1} meas={2} val={3}", z, ch, meas, values[z][ch][meas]);
            }
        }
    }
}
