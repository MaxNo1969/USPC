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

        public int currentOffsetFrames {get;private set;}     //Номер последнего кадра 
    	public AcqAscan[] ascanBuffer;	    //собранные кадры массив по платам

        public void Start()                     // Выполнить перед началом цикла сбора кадров с платы
        {
            currentOffsetFrames = 0;
        }
        public void OffsetCounter(int offs)
        {
            currentOffsetFrames += offs;
        }

        public bool addData(AcqAscan[] _data, int _cnt)
        {
            if (currentOffsetFrames + _cnt > countFrames)
                return false;
            else
            {
                Array.Copy(_data, 0, ascanBuffer, currentOffsetFrames, _cnt);
                OffsetCounter(_cnt);
                return true;
            }
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
        }
    };
}
