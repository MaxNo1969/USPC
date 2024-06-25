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
        public int sensors = 12;
        public int zones;

        
        public ListZones values = new ListZones();
        //Итоги в разрезе зона/датчик
        public double[][] zoneSensorResults;
        //Итоги по зоне
        public bool[] zoneResults;
        public void addZone(int[] _offsets)
        {
            ListSensors listSensors = new ListSensors();
            for (int sens = 0; sens < sensors; sens++)
            {
                listSensors.Add(new ListValues());
            }
            for (int numBoard = 0; numBoard < Program.numBoards; numBoard++)
            {
                USPCData data = Program.data[numBoard];
                //int currentOffsetFrames = data.currentOffsetFrames;
                int currentOffsetFrames = _offsets[numBoard];
                data.currentOffsetZones++;                
                data.offsets[data.currentOffsetZones] = currentOffsetFrames;
                int numberOfFrames = data.offsets[data.currentOffsetZones] - data.offsets[data.currentOffsetZones - 1];
                for (int frameOffset = 0; frameOffset < numberOfFrames; frameOffset++)
                {
                    AcqAscan scan = data.ascanBuffer[data.offsets[data.currentOffsetZones - 1]+frameOffset];
                    int channel = scan.Channel;
                    double def = scan.G1Amp;
                    double thick = 2.5e-6 * scan.G1Tof * Program.scopeVelocity;
                    //С первой платы получаем данные по толщинометрии
                    if (numBoard == 0)
                    {
                        if (channel < 4)
                        {
                            listSensors[channel].Add(thick);
                            if (thick < zoneSensorResults[zones][channel]) zoneSensorResults[zones][channel] = thick;
                        }
                        else
                        {
                            log.add(LogRecord.LogReason.warning, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Номер канала на первой плате больше 4");
                        }
                    }
                    //Со второй платы данные по дефектоскопии
                    else
                    {
                        //Продольная дефектоскопия
                        if (channel < 4)
                        {
                            listSensors[channel + 4].Add(def);
                            if (def < zoneSensorResults[zones][channel + 4]) zoneSensorResults[zones][channel + 4] = def;
                        }
                        //Поперечная дефектоскопия
                        else if (channel < 8)
                        {
                            listSensors[channel + 4].Add(def);
                            if (def < zoneSensorResults[zones][channel + 4]) zoneSensorResults[zones][channel + 4] = def;
                        }
                        else
                        {
                            log.add(LogRecord.LogReason.warning, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Номер канала на второй плате больше 8");
                        }
                    }
                }
            }
            zoneResults[zones] = true;
            for (int sensor = 0; sensor < sensors; sensor++)
            {
                if(sensor<4)
                {
                    if(DrawResults.IsBrakThick(zoneSensorResults[zones][sensor]))zoneResults[zones]=false;
                }
                else
                {
                    if (DrawResults.IsBrakDef(zoneSensorResults[zones][sensor])) zoneResults[zones] = false;
                }
            }
            zones++;
            values.Add(listSensors);
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2} {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Добавлена зона", zones);
        }

        public const int notMeasured = 101;
        public void ClearZoneSensorResult()
        {
            for (int z = 0; z < USPCData.countZones; z++)
            {
                zoneSensorResults[z] = new double[sensors];
                for (int s = 0; s < 4; s++)
                    zoneSensorResults[z][s] = notMeasured;
                for (int s = 4; s < 12; s++)
                    zoneSensorResults[z][s] = notMeasured;
                zoneResults[z] = false;
            }
        }
        public Result()
        {
            zones = 0;
            zoneSensorResults = new double[USPCData.countZones][];
            zoneResults = new bool[USPCData.countZones];
            ClearZoneSensorResult();
        }
        public void Clear()
        {
            zones = 0;
            values.Clear();
            ClearZoneSensorResult();
        }

    }

}
