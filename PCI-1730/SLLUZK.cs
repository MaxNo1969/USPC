using PROTOCOL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace PCI1730
{
    /// <summary>
    /// Делегат при тревожном сигнале
    /// </summary>
    public delegate void OnAlarm();
    /// <summary>
    /// Статический класс для PCIE1730
    /// Работает все время работы программы
    /// При выходе в том числе при аварии снимаем все выходные сигналы
    /// </summary>
    public static class SL 
    {
        private static SLLUZK sl = null;
        /// <summary>
        /// Получить список сигналов
        /// </summary>
        /// <returns></returns>
        public static SLLUZK getInst()
        {
            if (sl == null) sl = new SLLUZK();
            return sl;
        }
        /// <summary>
        /// Снимаем все выходные сигналы и чистим
        /// </summary>
        public static void Destroy()
        {
            if (sl != null)
            {
                sl.ClearAllSignals();
                sl.Dispose();
            }
        }
    }
    /// <summary>
    /// Именованый список сигналов для нашего приложения
    /// </summary>
    public class SLLUZK : SignalList
    {
        private Signal iCC_;
        /// <summary>
        /// ЦУП-Наличие сигнала показывает, что цепи управления влючены. ЦЕПИ УПРАВЛЕНИЯ. Питание +24В для внутренних нужд. При нажатом аварийном стопе снимается.
        /// </summary>
        public SignalIn iCC;
        
        private Signal iCYC_;
        /// <summary>
        /// ЦИКЛ. Сигнал установлен при нахождении оборудования ЛУЗК в автоматическом режиме.
        /// </summary>
        public SignalIn iCYC;

        private Signal iWRK_;
        /// <summary>
        /// РАБОТА. Сигнал устанавливается при готовности системы к контролю трубы.
        /// </summary>
        public SignalIn iWRK;

        private Signal iBASE_;
        /// <summary>
        ///  БАЗА. Сигнал устанавливается при достижении кареткой определенного положения. Используется для контроля скорости перемещения каретки.
        /// </summary>
        public SignalIn iBASE;

        private Signal iSTRB_;
        /// <summary>
        /// СТРОБ. Метка новой зоны
        /// </summary>
        public SignalIn iSTRB;

        private Signal oREADY_;
        /// <summary>
        /// ПЕРЕКЛАДКА. Сигнал устанавливается по завершению всех действий с результатом контроля трубы – разрешение забирать трубу из ванны.
        /// </summary>
        public SignalOut oREADY;

        /// <summary>
        /// Событие "Авария"
        /// </summary>
        public event OnAlarm onAlarm = null;


        /// <summary>
        /// Конструктор
        /// </summary>
        public SLLUZK() : base()
        {
            log.add(LogRecord.LogReason.info,"{0}: {1}","SLLUZK",System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                iCC_ = Find("ЦУП", true); iCC = new SignalIn(iCC_); MIn.Add(iCC);
                iCYC_ = Find("ЦИКЛ", true); iCYC = new SignalIn(iCYC_); MIn.Add(iCYC);
                iWRK_ = Find("РАБОТА", true); iWRK = new SignalIn(iWRK_); MIn.Add(iWRK);
                iBASE_ = Find("БАЗА", true); iBASE = new SignalIn(iBASE_); MIn.Add(iBASE);
                iSTRB_ = Find("СТРОБ", true); iSTRB = new SignalIn(iSTRB_); MIn.Add(iSTRB);

                oREADY_ = Find("ГОТОВНОСТЬ", false); oREADY = new SignalOut(oREADY_); MOut.Add(oREADY);
                Start();
            }
            catch (KeyNotFoundException)
            {
                log.add(LogRecord.LogReason.info, "SLLUZK: Ошибка причтении списка сигналов. Проверте настройки.");
            }
            catch
            {
                throw new Exception("Ошибка в конструкторк SignalListDef");
            }
        }
        /// <summary>
        /// Проверяем необходимые сигналы
        /// </summary>
        /// <returns></returns>
        public bool checkSignals()
        {
            if (iCC.Val == false)
            {
                throw new Exception("Пропал сигнал цепи управления");
            }
            else if (iCYC.Val == false)
            {
                throw new Exception("Пропал сигнал \"ЦИКЛ\"");
            }
            else
                return true;
        }
        /// <summary>
        /// Проверяем на состояние "Авария"
        /// </summary>
        protected override void CheckAlarm()
        {
            if (onAlarm != null) onAlarm();
        }
        /// <summary>
        /// Снимаем все выходные сигналы
        /// </summary>
        public void ClearAllSignals()
        {
            log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", "SLLUZK",System.Reflection.MethodBase.GetCurrentMethod().Name,"Снимаем выходные сигналы");
            if (oREADY != null) oREADY.Val = false;
        }
        /// <summary>
        /// Снимаем все входные сигналы (для эмулятора)
        /// </summary>
        /// <param name="_bIccOff">Надо ли выключать "Цепи управления"</param>
        public void ClearAllInputSignals(bool _bIccOff = false)
        {
            if (_bIccOff)
                set(iCC, false);
            set(iCYC, false);
            set(iWRK, false);
            set(iBASE, false);
            set(iSTRB, false);
        }
    }
}
