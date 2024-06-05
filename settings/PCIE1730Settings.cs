using USPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Settings;

namespace PCI1730
{
    /// <summary>
    /// Настройки платы PCIE-1730
    /// </summary>
    [DisplayName("PCIE-1730"), Description("Настройки платы PCIE-1730")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class PCIE1730Settings
    {
        /// <summary>
        /// Название устройства
        /// </summary>
        [DisplayName("1. Название устройства"), Description("Название устройства"), Category("1.Настройка модуля"), DefaultValue(0)]
        public string name { get; set; }
        /// <summary>
        /// Номер устройства
        /// </summary>
        [DisplayName("2.Номер устройства"), Description("Номер устройства"), Category("1.Настройка модуля"),DefaultValue(0)]
        public int devNum { get; set; }
        /// <summary>
        /// Количество входящих портов
        /// </summary>
        [DisplayName("3.Количество входящих портов"), Description("Количество входящих портов"), Category("1.Настройка модуля"),DefaultValue(4)]
        public int portInCnt { get; set; }
        /// <summary>
        /// Количество исходящих портов
        /// </summary>
        [DisplayName("4.Количество исходящих портов"), Description("Количество исходящих портов"), Category("1.Настройка модуля"), DefaultValue(4)]
        public int portOutCnt { get; set; }
        /// <summary>
        /// Задержка в потоке чтения портов
        /// </summary>
        [DisplayName("5.Задежка"), Description("Задержка в потоке чтения портов"), Category("1.Настройка модуля")]
        public int timeout { get; set; }
        /// <summary>
        /// Управление подключенными сигналами
        /// </summary>
        [DisplayName("6.Сигналы"), Description("Управление подключенными сигналами"), Category("1.Настройка модуля")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        public List<SignalSettings> sl {get; set;}
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public PCIE1730Settings()
        {
            name = "PCI-1730";
            devNum = 15;
            portInCnt = 4;
            portOutCnt = 4;
            timeout = 100;
            sl = new List<SignalSettings>();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_name">Название устройства</param>
        /// <param name="_devNum">Номер устройства</param>
        /// <param name="_portInCnt">Количество входных портов</param>
        /// <param name="_portOutCnt">Количество выходных портов</param>
        /// <param name="_timeout">Задержка в потоке чтения портов</param>
        public PCIE1730Settings(string _name,int _devNum = 0, int _portInCnt = 4, int _portOutCnt = 4, int _timeout=100 )
        {
            devNum = _devNum;
            portInCnt = _portInCnt;
            portOutCnt = _portOutCnt;
            timeout = _timeout;
            sl = new List<SignalSettings>();
        }
        /// <summary>
        /// Строковое преобразование (для PropertyGrid)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},BID#{1}", name, devNum);
        }
    }
}
