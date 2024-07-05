using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using PROTOCOL;
using Settings;

namespace Data
{
    public class ListValues
    {
        private List<double> _list = new List<double>();
        public void Add(double _val)
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
        public double this[int _index]
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
                    listSensors[sensor].Add(Result.deadZone);
                    zoneSensorResults[zone][sensor] = Result.deadZone;
                }
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
                        listSensors[channel].Add(Result.deadZone);
                    zoneSensorResults[zone][channel] = Result.deadZone;
                }
            }
            //zone++;
            zonesLengths.Add(Program.typeSize.currentTypeSize.deadZoneEnd);
            values.Add(listSensors);
            Program.frMain.zbWorker_ProgressChanged(null, null);
        }

        public void addZone(int[] _offsets)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: zones = {2}, offsets[0]={3}, ofsets[1]={4}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, zone, _offsets[0], _offsets[1]);
            ListSensors listSensors = new ListSensors();
            for (int sens = 0; sens < USPCData.countSensors; sens++)
            {
                listSensors.Add(new ListValues());
            }
            for (int numBoard = 0; numBoard < Program.numBoards; numBoard++)
            {
                USPCData data = Program.data[numBoard];
                int currentOffsetFrames = _offsets[numBoard];
                offsets.Add(_offsets[numBoard]);
                int numberOfFrames = currentOffsetFrames - offsets[zone-1];
                for (int frameOffset = 0; frameOffset < numberOfFrames; frameOffset++)
                {
                    AcqAscan scan = data.ascanBuffer[offsets[zone-1]+frameOffset];
                    int channel = scan.Channel;
                    double def = scan.G1Amp;
                    uint tof = (scan.G1Tof & AcqAscan.TOF_MASK) * 5;
                    double thick = USPCData.TofToMm(tof);
                    //С первой платы получаем данные по толщинометрии
                    if (numBoard == 0)
                    {
                        if (channel < 4)
                        {
                            listSensors[channel].Add(thick);
                            if (zoneSensorResults[zone][channel] == notMeasured || thick < zoneSensorResults[zone][channel]) zoneSensorResults[zone][channel] = thick;
                        }
                        else
                        {
                            //log.add(LogRecord.LogReason.warning, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Номер канала на первой плате больше 4");
                        }
                    }
                    //Со второй платы данные по дефектоскопии
                    else
                    {
                        //Продольная дефектоскопия
                        if (channel < 4)
                        {
                            listSensors[channel + 4].Add(def);
                            if (zoneSensorResults[zone][channel + 4]==notMeasured || def > zoneSensorResults[zone][channel + 4]) zoneSensorResults[zone][channel + 4] = def;
                        }
                        //Поперечная дефектоскопия
                        else if (channel < 8)
                        {
                            listSensors[channel + 4].Add(def);
                            if (zoneSensorResults[zone][channel + 4] == notMeasured || def > zoneSensorResults[zone][channel + 4]) zoneSensorResults[zone][channel + 4] = def;
                        }
                        else
                        {
                            //log.add(LogRecord.LogReason.warning, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Номер канала на второй плате больше 8");
                        }
                    }
                }
            }
            zoneResults[zone-1] = true;
            for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
            {
                if(sensor<4)
                {
                    if(DrawResults.IsBrakThick(zoneSensorResults[zone][sensor]))zoneResults[zone]=false;
                }
                else
                {
                    if (DrawResults.IsBrakDef(zoneSensorResults[zone][sensor])) zoneResults[zone] = false;
                }
            }
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2} {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Добавлена зона", zone);
            zonesLengths.Add(AppSettings.s.zoneSize);
            values.Add(listSensors);
            zone++;
        }

        public void CalcZone(int _zone)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: _zone = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _zone);
            for (int board = 0; board < Program.numBoards; board++)
            {
                int sensorsCount = (board == 0) ? 4 : 8;
                for (int sensor = 0; sensor < sensorsCount; sensor++)
                {
                    for (int meas = 0; meas < values[_zone].Count; meas++)
                    {

                        if (zoneSensorResults[_zone][sensor + board * 4] == notMeasured)
                            zoneSensorResults[_zone][sensor + board * 4] = values[_zone][sensor + board * 4][meas];
                        else if (board == 0)
                        {
                            if (values[_zone][sensor][meas] < zoneSensorResults[_zone][sensor])
                                zoneSensorResults[_zone][sensor] = values[_zone][sensor][meas];
                        }
                        else
                        {
                            if (values[_zone][sensor + 4][meas] > zoneSensorResults[_zone][sensor + 4])
                                zoneSensorResults[_zone][sensor + 4] = values[_zone][sensor + 4][meas];
                        }
                    }
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
            return true;
        }
    }

}
