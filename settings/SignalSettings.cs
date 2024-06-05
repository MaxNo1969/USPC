using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using USPC;
using Settings;

namespace PCI1730
{
    /// <summary>
    /// Параметры сигнала с платы цифрового ввода/вывода
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class SignalSettings
    {
        /// <summary>
        /// Название сигнала
        /// </summary>
        [DisplayName(" 1.Наименование"), Description("Название сигнала"), Category("Сигналы")]
        public string name { get; set; }
        /// <summary>
        /// Номер платы
        /// </summary>
        [DisplayName(" 2.Номер платы"), Description("Номер платы"), Category("Сигналы")]
        public int board { get; set; }
        /// <summary>
        /// Входной/Выходной
        /// </summary>
        [DisplayName(" 3.Входящий"), Description("Входной/Выходной"), Category("Сигналы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool input { get; set; }
        /// <summary>
        /// Цифровой
        /// </summary>
        [DisplayName(" 4.Цифровой"), Description("Цифровой"), Category("Сигналы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool digital { get; set; }
        /// <summary>
        /// Битовая позиция
        /// </summary>
        [DisplayName(" 5.Позиция"), Description("Позиция"), Category("Сигналы")]
        public int position { get; set; }
        /// <summary>
        /// Описание сигнала
        /// </summary>
        [DisplayName(" 6.Описание"), Description("Описание сигнала"), Category("Сигналы")]
        public string hint { get; set; }
        /// <summary>
        /// Ошибка ON
        /// </summary>
        [DisplayName(" 7.Ошибка ON"), Description("Ошибка ON"), Category("Сигналы")]
        public string eOn { get; set; }
        /// <summary>
        /// Ошибка OFF
        /// </summary>
        [DisplayName(" 8.Ошибка OFF"), Description("Ошибка OFF"), Category("Сигналы")]
        public string eOff { get; set; }
        /// <summary>
        /// Таймаут
        /// </summary>
        [DisplayName(" 9.Таймаут"), Description("Таймаут"), Category("Сигналы")]
        public TimeSpan timeout { get; set; }
        /// <summary>
        /// Не снимать
        /// </summary>
        [DisplayName("10.Не снимать"), Description("Не снимать"), Category("Сигналы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool no_reset { get; set; }
        /// <summary>
        /// Вербальный (Пишется в лог)
        /// </summary>
        [DisplayName("11.Вербальный"), Description("Вербальный"), Category("Сигналы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool verbal { get; set; }

        /// <summary>
        /// Строковое представление (для PropertyGrid)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
    }
}