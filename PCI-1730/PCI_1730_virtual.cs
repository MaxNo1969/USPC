using PROTOCOL;
using System.Diagnostics;
using System;

namespace PCI1730
{
    /// <summary>
    /// Эмулятор платы ввода/вывода 
    /// </summary>
    public class PCI_1730_virtual  : PCI_1730
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_portCount_in"></param>
        /// <param name="_portCount_out"></param>
        public PCI_1730_virtual(string _name, int _portCount_in, int _portCount_out):base(_name,_portCount_in,_portCount_out)
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        /// <summary>
        /// Читаем входные сигналы
        /// </summary>
        /// <returns></returns>
        public override byte[] Read()
        {
            if (disposed)
                return (null);
            return (values_in);
        }
        /// <summary>
        /// Читаем выходные сигналы
        /// </summary>
        /// <returns></returns>
        public override byte[] ReadOut()
        {
            if (disposed)
                return (null);
            return (values_out);
        }
        /// <summary>
        /// Записываем выходные сигналы
        /// </summary>
        /// <param name="_values_out"></param>
        public override void Write(byte[] _values_out)
        {
            if (disposed)
                return;
            Array.Copy(_values_out, values_out, portCount_out);
        }
    }
}
