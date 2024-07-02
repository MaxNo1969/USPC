using System;
using System.Collections.Generic;
using System.Diagnostics;
using USPC;
using PROTOCOL;
using Settings;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
namespace Data
{
    /// <summary>
    /// Данные для сохранения трубы
    /// </summary>
    [Serializable]
    class USPCData
    {
        public const int countZones = 70;
        public const int countSensors = 12;
        public const int countFrames = 900000;

        public static int lengthCaretka = 20;
        
        public int currentOffsetFrames;     //Номер последнего кадра 
        public int currentOffsetZones;	    
    	public AcqAscan[] ascanBuffer;	    //собранные кадры массив по платам
        public TimeLabels labels;
        //public List<Slice> allData;
        //public List<Slice> zoneData; 

	    public int[] offsets;               //смещение кадров по зонам
        public int[] offsSensor;            //смещение кадров по датчикам
	    public double[] minZoneThickness;	//Минимальная толщина по зоне
        public double[][] minZoneSensorThickness;	//Минимальная толщина по зоне/датчику
        public int samplesPerZone;
        public void Start()                     // Выполнить перед началом цикла сбора кадров с платы
        {
            currentOffsetFrames = 0;
            currentOffsetZones = 0;
            labels.Clear();
        }
        public void OffsetCounter(int offs)
        {
            currentOffsetFrames += offs;
            labels.Add(new BufferStamp(DateTime.Now,currentOffsetFrames));
        }

        public static double TofToMm(UInt32 _tof)
        {
            //return 2.5e-6 * _tof * Program.scopeVelocity;
            //нс * м/с
            return _tof * Program.scopeVelocity *100*5 / 1000000000;
        }

        public static double TofToMm(AcqAscan _scan)
        {
            return TofToMm(_scan.G1Tof*5);
        }

        public static uint MmToTof(double _mm)
        {
            return (uint)(_mm * 1000000000 / Program.scopeVelocity/100);
        }
        public USPCData()
        {
            ascanBuffer = new AcqAscan[countFrames];
            offsets = new int[countZones];
            offsSensor = new int[countSensors];
            minZoneThickness = new double[countZones];
            minZoneSensorThickness = new double[countZones][];
            for (int i = 0; i < countZones; i++)
                minZoneSensorThickness[i] = new double[countSensors];
            labels = new TimeLabels();
        }
    };
}
