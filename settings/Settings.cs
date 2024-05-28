using PROTOCOL;
using PCIE1730;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Data;

namespace Settings
{
    /// <summary>
    /// Делегат на смену параметров
    /// </summary>
    public delegate void OnChangeSettings(object[] _params);
    /// <summary>
    /// Класс для хранения настроек параметров приложения. Хранить будем в XML
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        ///Длина трубы (мм)
        /// </summary>
        [DisplayName("Длина"), Browsable(true), Description("Длина трубы (мм)"), Category("1.Труба")]
        public int tubeLength { get; set; }
        /// <summary>
        /// Размер одной зоны
        /// </summary>
        [DisplayName("Размер зоны"), Browsable(true), Description("Размер зоны (мм)"), Category("1.Труба")]
        public int zoneSize { get; set; }
        /// <summary>
        /// Настройки для PCIE1730
        /// </summary>
        [Category("3.Оборудование")]
        public PCIE1730Settings pcie1730Settings { get; set; }

        /// <summary>
        /// Настройки установки. Скорость движения трубы
        /// </summary>
        [Category("4.Установка")]
        [DisplayName("Расстояние до базы(мм):"), Browsable(true), Description("Расстояние до базы (мм)")]
        public int distanceToBase { get; set; }
        [DisplayName("Скорость(м/с)"), Browsable(true), Description("Скорость движения трубы (м/с)")]
        public double speed { get; set; }

        /// <summary>
        /// Текущий выбранный типоразмер
        /// </summary>
        [DisplayName("Выбранный типоразмер"), Browsable(true), Description("Текущий выбраный типоразмер"), Category("2.Типоразмеры")]
        [TypeConverter(typeof(TypesizeTypeConverter))]
        public string currenTypeSizeName { get; set; }
        /// <summary>
        /// Настройки типоразмеров
        /// </summary>
        [DisplayName("Типоразмеры"), Browsable(true),Description("Настройка типоразмеров"), Category("2.Типоразмеры")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        public List<_TypeSize> tss { get; private set; }
        /// <summary>
        /// Признак изменения настроек
        /// </summary>
        [XmlIgnore]
        public bool changed = false;

        /// <summary>
        /// Функция вызывается при изменении настроек
        /// </summary>
        [XmlIgnore]
        public OnChangeSettings onChangeSettings = null;

        private Settings()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            pcie1730Settings = new PCIE1730Settings();
            tss = new List<_TypeSize>();
        }

        private const string fileName = "settings.xml";
        /// <summary>
        /// Загрузка параметров из файла
        /// </summary>
        /// <returns>Указатель на параметры</returns>
        public static Settings load()
        {
            Settings s;
            try
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}", "Settings", System.Reflection.MethodBase.GetCurrentMethod().Name);
                // передаем в конструктор тип класса
                XmlSerializer formatter = new XmlSerializer(typeof(Settings));            
                // десериализация
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    s = (Settings)formatter.Deserialize(fs);
                }
                return s;
            }
            catch(Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", "Settings", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //Обработка первого запуска - файл настроек ещё не записан
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "Settings", System.Reflection.MethodBase.GetCurrentMethod().Name, "Первый запуск - файл настроек ещё не записан");
                s = new Settings();
                Settings.save(s);
                return s;
            }
        }
        /// <summary>
        /// Запись параметров в файл
        /// </summary>
        /// <param name="_s">Параметры для записи</param>
        public static void save(Settings _s)
        {
            if (_s.changed)
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}", "Settings", System.Reflection.MethodBase.GetCurrentMethod().Name);
                // передаем в конструктор тип класса
                try
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(Settings));            
                    // сериализация
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        formatter.Serialize(fs, _s);
                        _s.changed = false;
                    }
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error,"{0}: {1}: {2}", "Settings", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                }
            }
            return;
        }
        public bool check()
        {
            //Есть ли что-нибудь в списке типоразмеров
            if (tss == null || tss.Count == 0)
                return false;
            return true;
        }

    }
    /// <summary>
    /// Класс для глобального доступа к параметрам
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// хранит настройки приложения
        /// </summary>
        public static readonly Settings settings = null;
        /// <summary>
        /// конструктор выполняется один раз при запуске приложения
        /// </summary>
        static AppSettings()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", "AppSettings", System.Reflection.MethodBase.GetCurrentMethod().Name);
             if (settings == null) settings = Settings.load(); 
             //appSettings = new Settings();
         }

        /// <summary>
        /// Получаем доступ к объекту-свойствам
        /// </summary>
        public static Settings s
        {
            get
            {
                return settings;
            }
        }
    }
}
