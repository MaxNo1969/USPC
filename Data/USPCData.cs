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
        public int currentOffsetFrames;     //Номер последнего кадра 
        public int currentOffsetZones;	    //номер смещения кадра в зоне
    	public AcqAscan[] ascanBuffer;	    //собранные кадры
	    public int[] offsets;               //смещение кадров по зонам
        public int[] offsSensor;            //смещение кадров по датчикам
	    public int[] commonStatus;			//общий статус по зонам
	    public double samplesPerZone;
	    public int deadZoneSamplesBeg;
        public int deadZoneSamplesEnd;
        public void Start()                        // Выполнить перед началом цикла сбора кадров с платы
        {
            currentOffsetFrames = 0;
        }
        public void OffsetCounter(int offs)
        {
            currentOffsetFrames += offs;
        }

        public double TofToMm(int _tof)
        {
            return 2.5e-6 * _tof * Program.scopeVelocity;
        }

        public double TofToMm(AcqAscan _scan)
        {
            return TofToMm((int)_scan.G1Tof);
        }

        public uint MmToTof(double _mm)
        {
            return (uint)(_mm / (2.5e-6 * Program.scopeVelocity));
        }

        public double[] sensorThickness(int _sensor)
        {
            double[] ret = new double[Program.countFramesPerChannel];
            for (int i = 0; i < Program.countFrames/Program.countSensors; i ++)
            {
                if (ascanBuffer[i * Program.countSensors + _sensor].Channel == _sensor)
                {
                    ret[i] = TofToMm(ascanBuffer[i * Program.countSensors + _sensor]);
                }
                else
                {
                    throw new Exception("Номер канала не совпадает с требуемым");
                }
            }
            return ret;
        }

        public int[] sensorDefects(int _sensor)
        {
            int[] ret = new int[Program.countFramesPerChannel];
            for (int i = 0; i < Program.countFrames / Program.countSensors; i++)
            {
                if (ascanBuffer[i * Program.countSensors + _sensor].Channel == _sensor)
                {
                    ret[i] = ascanBuffer[i * Program.countSensors + _sensor].G1Amp;
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
        public double[] evalZone(int ZoneNo, int SensorNo, bool medianFilter = true)
        {
            //Количество сканов по одному датчику в одной зоне
            int cnt = 0;
            if(offsets[ZoneNo+1]-offsets[ZoneNo] > 0)
            {
                cnt = offsets[ZoneNo+1]-offsets[ZoneNo];
            }
            // вектор толщин для всех измерений в пределах данной зоны _для одного датчика_
            double[] ths = new double[cnt];
            for (int i = 0; i < cnt; i++)
            {
                ths[i] = TofToMm(ascanBuffer[offsets[ZoneNo] + SensorNo]);
            }
            if (medianFilter) ths = Median.Filter(ths, Program.medianFilterWidth);
            return ths;
        }



	    public void SamplesPerZone(int tubeLength, int deadArea0, int deadArea1)
        {
            samplesPerZone = (double)Program.zoneLength * currentOffsetFrames / (tubeLength + Program.lengthCaretka);
            for(int i =0 ; i < offsets.Length;i++)offsets[i]=0;
            for(int i = 0; i < Program.countZones; ++i)
            {
                offsets[i] = (int)(samplesPerZone * i);
            }
            currentOffsetZones = (int)((double)(tubeLength) / Program.zoneLength);
	        int lastZoneSize = tubeLength - currentOffsetZones * Program.zoneLength;
            if(lastZoneSize > Program.zoneLength / 3)  ++currentOffsetZones;
	        //число отчётов в мёртвой зоне начало
	        double t = deadArea0;
            t *= samplesPerZone;
            t /=  Program.zoneLength;
            deadZoneSamplesBeg  = (int)t;
            deadZoneSamplesBeg /= Program.countSensors;
	        deadZoneSamplesBeg *= Program.countSensors;
	        //число отчётов в мёртвой зоне конец
	        t = tubeLength - deadArea1;
	        t *= samplesPerZone;
	        t /=  Program.zoneLength;
            deadZoneSamplesEnd  = (int)t;
            deadZoneSamplesEnd /= Program.countSensors;
	        --deadZoneSamplesEnd;
            deadZoneSamplesEnd *= Program.countSensors;

	        for(int i = 0; i < Program.countZones; ++i)
	        {
                offsets[i] /= Program.countSensors;
		        offsets[i] *= Program.countSensors;
            }
        }
        
        public USPCData()
        {
            ascanBuffer = new AcqAscan[Program.countFrames];
            int ascanBufferMemoryUsed = ascanBuffer.Length*8*4;
            offsets = new int[Program.countZones];
            int offsetsMemoryUsed = offsets.Length * sizeof(int);
            offsSensor = new int[Program.countSensors];
            int offsSensorMemoryUsed = offsSensor.Length * sizeof(int);
            commonStatus = new int[Program.countZones];
            int commonStatusMemoryUsed = commonStatus.Length * sizeof(char); ;
            log.add(LogRecord.LogReason.info, "{0}: {1}: ascanBufferMemoryUsed={2}, offsetsMemoryUsed={3}, offsSensorMemoryUsed={4}, commonStatusMemoryUsed={5}",
                GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ascanBufferMemoryUsed, offsetsMemoryUsed, offsSensorMemoryUsed, commonStatusMemoryUsed);
        }


        public bool saveAsync(string _fileName)
        {
            System.Threading.WaitCallback callback = save;
            ThreadPool.QueueUserWorkItem(save, (object)_fileName);
            return true;
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
