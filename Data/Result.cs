using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using PROTOCOL;

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
        public int crossSensors = 4;
        public int linearSensors = 4;
        public int thicknesSensors = 4;
        public int zones;

        public ListZones values = new ListZones();
        public double[][] zoneSensorResults;
        public void addZone()
        {
            for (int numBoard = 0; numBoard < 2; numBoard++)
            {
                USPCData data = Program.data[numBoard];
                int currentOffsetFrames = data.currentOffsetFrames;
                data.currentOffsetZones++;                
                data.offsets[data.currentOffsetZones] = currentOffsetFrames;
                int numberOfFrames = data.offsets[data.currentOffsetZones] - data.offsets[data.currentOffsetZones - 1];
                ListSensors listSensors = new ListSensors();
                for (int sens = 0; sens < sensors; sens++)
                {
                    listSensors.Add(new ListValues());
                }
                for (int frameOffset = 0; frameOffset < numberOfFrames; frameOffset++)
                {
                    AcqAscan scan = data.ascanBuffer[data.offsets[data.currentOffsetZones - 1]+frameOffset];
                    int channel = scan.Channel;
                    double def = scan.G1Amp;
                    double thick = 2.5e-6 * scan.G1Tof * Program.scopeVelocity;
                    if (channel < 0) continue;
                    else if (channel < 4)
                    {
                        listSensors[channel].Add(thick);
                        if (thick < zoneSensorResults[zones][channel])zoneSensorResults[zones][channel]  = thick;
                    }
                    else
                    {
                        listSensors[channel].Add(def);
                        if (def < zoneSensorResults[zones][channel]) zoneSensorResults[zones][channel] = def;
                    }
                }
                zones++;
                values.Add(listSensors);
            }

            log.add(LogRecord.LogReason.debug,"{0}: {1}: {2} {3}",GetType().Name,System.Reflection.MethodBase.GetCurrentMethod().Name,"Добавлена зона",zones);
        }
        
        public Result()
        {
            zones = 0;
            double[] sensorResults = new double[sensors];
            zoneSensorResults = new double[USPCData.countZones][];
            for (int z = 0; z < USPCData.countZones; z++)
            {
                zoneSensorResults[z] = new double[sensors];
                for (int s = 0; s < sensors; s++)
                    zoneSensorResults[z][s] = double.MaxValue;
            }
        }
        public void Clear()
        {
            values.Clear();
        }

    }

}
