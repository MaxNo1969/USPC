using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using System.ComponentModel;
using PROTOCOL;
using Settings;

namespace Data
{
    //! @brief Класс для гненерации различных измерений 
    public static class DataGenerator
    {
        static Random r = new Random();
        public static void GenerateThicknessData(double _thick, int _board = 0, int _count = USPCData.countFrames, BackgroundWorker _w = null, DoWorkEventArgs _e = null)
        {
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", "DataGenerator", System.Reflection.MethodBase.GetCurrentMethod().Name, "Начало генерации данных");
            if (Program.data == null) return;
            AcqAscan[] scans = Program.data[_board].ascanBuffer;
            for (int frame = 0; frame < _count - USPCData.countSensors; frame += USPCData.countSensors)
            {
                if (_w != null && _w.CancellationPending)
                {
                    if (_e != null) _e.Cancel = true;
                }
                else
                {
                    int percent = (int)((double)frame * 100 / _count);
                    if (_w != null) _w.ReportProgress(percent);
                    for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
                    {
                        scans[frame + sensor].Channel = (byte)sensor;
                        double val = _thick + (r.NextDouble() - 0.5) * 2;
                        uint G1Tof = (uint)(val / (2.5e-6 * Program.scopeVelocity));
                        scans[frame + sensor].G1Tof = G1Tof;
                    }
                }
                _w.ReportProgress((int)(frame * 100 / _count));
            }
            for (int zone = 0; zone < USPCData.countZones; zone++)
            {
                int countFramesPerZone = USPCData.countFrames / USPCData.countZones;
                countFramesPerZone /= USPCData.countSensors;
                countFramesPerZone *= USPCData.countSensors;
                Program.data[_board].offsets[zone] = zone * countFramesPerZone;
            }
            Program.data[_board].samplesPerZone = (int)((double)AppSettings.s.zoneSize * USPCData.countFrames / AppSettings.s.tubeLength);
            for (int zone = 0; zone < USPCData.countZones; zone++)
            {
                double minZoneThickness = Program.typeSize.maxDetected;
                for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
                {
                    Program.data[_board].minZoneSensorThickness[zone] = Program.data[_board].sensorThickness(sensor, zone);
                    double min = Program.data[_board].minZoneSensorThickness[sensor][zone];
                    if (minZoneThickness > min) minZoneThickness = min;
                }
                Program.data[_board].minZoneThickness[zone] = minZoneThickness;
            }
            log.add(LogRecord.LogReason.debug, "{0}: {1}: {2}", "DataGenerator", System.Reflection.MethodBase.GetCurrentMethod().Name, "Генерация данных закончена");
        }
        public static void GeterateDefectsData(int _board,int _percent, int _count)
        {
            if (Program.data == null) return;
            AcqAscan[] scans = Program.data[_board].ascanBuffer;
            for (int frame = 0; frame < _count; frame += USPCData.countSensors)
            {
                for (int sensor = 0; sensor < USPCData.countSensors; sensor++)
                {
                    scans[frame * USPCData.countSensors + sensor].Channel = (byte)sensor;
                    int val = _percent + r.Next(-10, 10);
                    scans[frame * USPCData.countSensors + sensor].G1Amp = (byte)val;
                }
            }
        }
        //public static AcqAscan thickScan(double _thick)
        //{
        //    AcqAscan scan = new AcqAscan();
        //    double val = _thick + (r.NextDouble() - 0.5) * 2;
        //    uint G1Tof = (uint)(val / (2.5e-6 * Program.scopeVelocity));
        //    scan.G1Tof = G1Tof;
        //    return scan;
        //}
        //public static AcqAscan defScan(int _percent)
        //{
        //    AcqAscan scan = new AcqAscan();
        //    byte G1Amp = (byte)(_percent + r.Next(-10, 10));
        //    scan.G1Amp = G1Amp;
        //    return scan;
        //}

        //public static AcqAscan[] thickPacket(double _thick, int _size)
        //{
        //    int numberOfScans = _size + r.Next(-10, 10);
        //    AcqAscan[] scans = new AcqAscan[numberOfScans];
        //    for (int i = 0; i < numberOfScans;i++)
        //    {
        //        scans[i] = thickScan(_thick);
        //    }
        //    return scans;
        //}
        //public static AcqAscan[] defPacket(int _percent, int _size)
        //{
        //    int numberOfScans = _size + r.Next(-10, 10);
        //    AcqAscan[] scans = new AcqAscan[numberOfScans];
        //    for (int i = 0; i < numberOfScans; i++)
        //    {
        //        scans[i] = defScan(_percent);
        //    }
        //    return scans;
        //}

        public static AcqAscan[] packet(double _thick,int _percent,int _size)
        {
            int numberOfScans = _size + r.Next(-10, 10);
            AcqAscan[] scans = new AcqAscan[numberOfScans];
            for (int i = 0; i < numberOfScans; i++)
            {
                byte G1Amp = (byte)(_percent + r.Next(-10, 10));
                scans[i].G1Amp = G1Amp;
                double val = _thick + (r.NextDouble() - 0.5) * 2;
                uint G1Tof = (uint)(val / (2.5e-6 * Program.scopeVelocity));
                scans[i].G1Tof = G1Tof;
            }
            return scans;
        }
    }
}
