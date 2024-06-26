﻿using Automation.BDaq;
using PROTOCOL;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PCIE1730
{
    /// <summary>
    /// Плата цифрового ввода/вывода (PCIE1730Сохранение размера и положения формы)
    /// </summary>
    public class PCIE_1730_real:PCIE_1730
    {
        /// <summary>
        /// Очистить
        /// </summary>
        public override void Dispose()
        {
            ctrl_in.Cleanup();
            ctrl_out.Cleanup();
            base.Dispose();
        }
        private InstantDiCtrl ctrl_in;
        private InstantDoCtrl ctrl_out;
        private int portStart = 0;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_name">Имя платы для инициализации</param>
        /// <param name="_portCount_in">Количество входящих сигналов</param>
        /// <param name="_portCount_out">Количество исходящих сигналов</param>
        public PCIE_1730_real(string _name, int _portCount_in, int _portCount_out):base(_name,_portCount_in,_portCount_out)
        {
            ctrl_in = new InstantDiCtrl();
            ctrl_out = new InstantDoCtrl();
            try
            {
                ctrl_in.SelectedDevice = new DeviceInformation(name);
                ctrl_out.SelectedDevice = new DeviceInformation(name);
            }
            catch (Exception e)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Читаем входные сигналы
        /// </summary>
        /// <returns></returns>
        public override byte[] Read()
        {
            if (disposed)
                return null;
            try
            {
                ErrorCode ret = ctrl_in.Read(portStart, values_in.Length, values_in);
                if (ret != ErrorCode.Success)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: Error: {3})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, ret.ToString());
                    return null;
                }
            }
            catch (Exception e)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, e.Message);
                return null;
            }
            return (values_in);
        }
        /// <summary>
        /// Читаем выходные сигналы
        /// </summary>
        /// <returns></returns>
        public override byte[] ReadOut()
        {
            if (disposed)
                return null;
            try
            {
                ErrorCode ret = ctrl_out.Read(portStart, values_out.Length, values_out);
                if (ret != ErrorCode.Success)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: Error: {3})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, ret.ToString());
                    return null;
                }
            }
            catch (Exception e)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, e.Message);
                return null;
            }
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
            try
            {
                ErrorCode ret = ctrl_out.Write(portStart, values_out.Length, _values_out);
                if (ret != ErrorCode.Success)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: {2}: Error: {3})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, ret.ToString());
                    return;
                }
            }
            catch (Exception e)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2})", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, name, e.Message);
                throw e;
            }
        }
    }
}
