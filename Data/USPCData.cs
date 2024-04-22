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
        public const int countZones = 300;
        public const int countSensors = 8;
        public const int zoneLength = 50;
        public const int countFrames = 900000;
        //public const int countFrames = 9000;
        //public const int countFramesPerChannel = countFrames / countSensors;
        //public const int countFramesPerZone = countFrames / countZones;

        public const int scopeVelocity = 6400;

        public static int lengthCaretka = 20;
        
        public int currentOffsetFrames;     //Номер последнего кадра 
        public int currentOffsetZones;	    //номер смещения кадра в зоне
    	public AcqAscan[] ascanBuffer;	    //собранные кадры
	    public int[] offsets;               //смещение кадров по зонам
        public int[] offsSensor;            //смещение кадров по датчикам
	    public double[] minZoneThickness;	//Минимальная толщина по зоне
	    public int samplesPerZone;
	    public int deadZoneSamplesBeg;
        public int deadZoneSamplesEnd;
        public void Start()                     // Выполнить перед началом цикла сбора кадров с платы
        {
            currentOffsetFrames = 0;
        }
        public void OffsetCounter(int offs)
        {
            currentOffsetFrames += offs;
        }

        public double TofToMm(int _tof)
        {
            return 2.5e-6 * _tof * scopeVelocity;
        }

        public double TofToMm(AcqAscan _scan)
        {
            return TofToMm((int)_scan.G1Tof);
        }

        public uint MmToTof(double _mm)
        {
            return (uint)(_mm / (2.5e-6 * scopeVelocity));
        }

        public double[] sensorThickness(int _sensor)
        {
            double[] ret = new double[countFrames/countSensors];
            for (int i = 0; i < countFrames / countSensors; i++)
            {
                if (ascanBuffer[i * countSensors + _sensor].Channel == _sensor)
                {
                    ret[i] = TofToMm(ascanBuffer[i * countSensors + _sensor]);
                }
                else
                {
                    throw new Exception("Номер канала не совпадает с требуемым");
                }
            }
            return ret;
        }

        public double getMinThickness(int _sensor, int _zone)
        {
            if (_sensor > countSensors - 1) return 0;
            if (_zone > countZones - 2) return 0;
            int countFrames = (offsets[_zone + 1] - offsets[_zone]) / countSensors;
            double ret = Program.typeSize.maxDetected;
            for (int i = 0; i < countFrames; i++)
            {
                AcqAscan scan = ascanBuffer[offsets[_zone] + i * countSensors + _sensor];
                if (scan.Channel == _sensor)
                {
                    double val = TofToMm(ascanBuffer[offsets[_zone] + i * countSensors + _sensor]);
                    if (val < ret)
                        ret = val;
                }
                else
                {
                    throw new Exception("Номер канала не совпадает с требуемым");
                }
            }
            return ret;
        }


        public double[] sensorThickness(int _sensor, int _zone)
        {
            if (_sensor > countSensors - 1) return null;
            if (_zone > countZones - 2) return null;
            int countFrames = (offsets[_zone + 1] - offsets[_zone]) / countSensors;
            double[] ret = new double[countFrames];
            for (int i = 0; i < countFrames; i++)
            {
                if (ascanBuffer[offsets[_zone] + i * countSensors + _sensor].Channel == _sensor)
                {
                    ret[i] = TofToMm(ascanBuffer[i * countSensors + _sensor]);
                }
                else
                {
                    throw new Exception("Номер канала не совпадает с требуемым");
                }
            }
            return ret;
        }

        public int[] sensorDefects(int _sensor, int _zone)
        {
            if (_sensor > countSensors - 1) return null;
            if (_zone > countZones - 2) return null;
            int countFrames = (offsets[_zone + 1] - offsets[_zone]) / countSensors;
            int[] ret = new int[countFrames];
            for (int i = 0; i < countFrames; i++)
            {
                if (ascanBuffer[offsets[_zone] + i * countSensors + _sensor].Channel == _sensor)
                {
                    ret[i] = ascanBuffer[i * countSensors + _sensor].G1Amp;
                }
                else
                {
                    throw new Exception("Номер канала не совпадает с требуемым");
                }
            }
            return ret;
        }

        //! @brief Возвращает массив толщин по одной зоне, одному датчику с возможностью фильтрации.
        //! @brief Функция для детального просмотра измерений во ViewTubeDetails
        //! @param[in] SensorNo Номер датчика
        //! @param[in] ZoneNo Номер зоны
        //! @param[in] medianFilter Включать ли медианный фильтр
        //! @note при расчете трубы, финального результата медианный фильтр включен ВСЕГДА.
        //! эта функция создана для просмотра фильтрованного и нефильтрованного сигнала по зоне
        public double[] evalZone(int _zone, int _sensor, bool medianFilter = true)
        {
            //Количество сканов по одному датчику в одной зоне
            int cnt = 0;
            if(offsets[_zone+1]-offsets[_zone] > 0)
            {
                cnt = (offsets[_zone+1]-offsets[_zone])/USPCData.countSensors;
            }
            // вектор толщин для всех измерений в пределах данной зоны _для одного датчика_
            double[] ths = new double[cnt];
            for (int i = 0; i < cnt; i++)
            {
                ths[i] = TofToMm(ascanBuffer[offsets[_zone] + i * USPCData.countSensors + _sensor]);
            }
            if (medianFilter) ths = Median.Filter(ths, Program.medianFilterWidth);
            return ths;
        }

        public USPCData()
        {
            ascanBuffer = new AcqAscan[countFrames];
            offsets = new int[countZones];
            offsSensor = new int[countSensors];
            minZoneThickness = new double[countZones];
        }


        public bool saveAsync(string _fileName)
        {
            System.Threading.WaitCallback callback = save;
            ThreadPool.QueueUserWorkItem(save, (object)_fileName);
            return true;
        }
        
        public void SamplesPerZone(int tubeLength, int deadArea0, int deadArea1)
        {
            //samplesPerZone = (double)zoneLength * currentOffsetFrames / (tubeLength + lengthCaretka);
            samplesPerZone = (int) ((double)zoneLength * currentOffsetFrames / tubeLength);	
	        for(int i = 0; i < countZones; ++i)
            {
                offsets[i] = samplesPerZone * i;
            }
	        //смещение в отчётах датчиков на каретке
	        //TL::foreach<OffsetsTable::items_list, __sensors_offset_in_samples__>()(&Singleton<OffsetsTable>::Instance().items, this);
	        currentOffsetZones = (int)((double)(tubeLength) / zoneLength);
	        int lastZoneSize = tubeLength - currentOffsetZones * zoneLength;
	        if(lastZoneSize > zoneLength / 3)  ++currentOffsetZones;
	        //число отчётов в мёртвой зоне начало
	        double t = deadArea0;
	        t *= samplesPerZone;
	        t /=  zoneLength;
            deadZoneSamplesBeg  = (int)t;
	        deadZoneSamplesBeg /= countSensors;
	        deadZoneSamplesBeg *= countSensors;
	        //число отчётов в мёртвой зоне конец
	        t = tubeLength - deadArea1;
	        t *= samplesPerZone;
	        t /=  zoneLength;
            deadZoneSamplesEnd  = (int)t;
            deadZoneSamplesEnd /= countSensors;
	        --deadZoneSamplesEnd;
	        deadZoneSamplesEnd *= countSensors;

	        for(int i = 0; i < countZones; ++i)
            {
                offsets[i] /= countSensors;
		        offsets[i] *= countSensors;
            }
        }

        public void save(Object obj)
        {
            string fileName = (string)obj;
            if (fileName == null) return;
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                }
                log.add(LogRecord.LogReason.info, "{0}: {1}: Файл \"{2}\" сохранен", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, fileName);
                return;
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return;
            }
        }

        public static USPCData load(string _fileName)
        {
            if (_fileName == null) return null;
            try
            {
                using (FileStream stream = new FileStream(_fileName, FileMode.Open))
                {
                    log.add(LogRecord.LogReason.info, "{0}: {1}: Начало загрузки файла \"{2}\" ", "USPCData", System.Reflection.MethodBase.GetCurrentMethod().Name, _fileName);
                    IFormatter formatter = new BinaryFormatter();
                    USPCData data = (USPCData)formatter.Deserialize(stream);
                    log.add(LogRecord.LogReason.info, "{0}: {1}: Файл \"{2}\" загружен", "USPCData", System.Reflection.MethodBase.GetCurrentMethod().Name, _fileName);
                    return data;
                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", "USPCData", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
        }
    };
}
