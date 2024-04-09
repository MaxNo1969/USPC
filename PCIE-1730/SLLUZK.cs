using PROTOCOL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace PCIE1730
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
        
        private Signal iREADY_;
        /// <summary>
        /// ГОТОВНОСТЬ. Сигнал устанавливается при наличии в ванне готовой к контролю трубы.
        /// </summary>
        public SignalIn iREADY;

        private Signal iCNTR_;
        /// <summary>
        /// КОНТРОЛЬ. Сигнал устанавливается при поступлении первого датчика каретки на поверхность трубы, снимается по сходу последнего датчика каретки с трубы.
        /// </summary>
        public SignalIn iCNTR;

        private Signal iBASE_;
        /// <summary>
        ///  БАЗА. Сигнал устанавливается при достижении кареткой определенного положения. Используется для контроля скорости перемещения каретки.
        /// </summary>
        public SignalIn iBASE;
        
        private Signal iREZ_;
        /// <summary>
        /// РЕЗЕРВ. Резервный сигнал
        /// </summary>
        public SignalIn iREZ;

        
        private Signal oWRK_;
        /// <summary>
        /// РАБОТА. Сигнал устанавливается при готовности системы к контролю трубы.
        /// </summary>
        public SignalOut oWRK;
        
        private Signal oPEREKL_;
        /// <summary>
        /// ПЕРЕКЛАДКА. Сигнал устанавливается по завершению всех действий с результатом контроля трубы – разрешение забирать трубу из ванны.
        /// </summary>
        public SignalOut oPEREKL;
        
        private Signal oRES1_;
        /// <summary>
        /// РЕЗУЛЬТАТ1. Сигнал для кодирования результата контроля трубы.
        /// </summary>
        public SignalOut oRES1;

        private Signal oRES2_;
        /// <summary>
        /// РЕЗУЛЬТАТ2. Сигнал для кодирования результата контроля трубы.
        /// </summary>
        public SignalOut oRES2;

        private Signal oPBM_;
        /// <summary>
        /// ПИТАНИЕ БМ. Сигнал включает подачу питания на блок мультиплексоров.
        /// </summary>
        public SignalOut oPBM;


        private Signal oREZ_;
        /// <summary>
        /// РЕЗЕРВ. Резервный сигнал.
        /// </summary>
        public SignalOut oREZ;

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
                iREADY_ = Find("ГОТОВНОСТЬ", true); iREADY = new SignalIn(iREADY_); MIn.Add(iREADY);
                iCNTR_ = Find("КОНТРОЛЬ", true); iCNTR = new SignalIn(iCNTR_); MIn.Add(iCNTR);
                iBASE_ = Find("БАЗА", true); iBASE = new SignalIn(iBASE_); MIn.Add(iBASE);
                iREZ_ = Find("РЕЗЕРВВХ", true); iREZ = new SignalIn(iREZ_); MIn.Add(iREZ);

                oWRK_ = Find("РАБОТА", false); oWRK = new SignalOut(oWRK_); MOut.Add(oWRK);
                oPEREKL_ = Find("ПЕРЕКЛ", false); oPEREKL = new SignalOut(oPEREKL_); MOut.Add(oPEREKL);
                oRES1_ = Find("РЕЗУЛТ1", false); oRES1 = new SignalOut(oRES1_); MOut.Add(oRES1);
                oRES2_ = Find("РЕЗУЛТ2", false); oRES2 = new SignalOut(oRES2_); MOut.Add(oRES2);
                oPBM_ = Find("ПИТБМ", false); oPBM = new SignalOut(oPBM_); MOut.Add(oPBM);
                oREZ_ = Find("РЕЗЕРВВЫХ", false); oREZ = new SignalOut(oREZ_); MOut.Add(oREZ);
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

            if (oWRK != null) oWRK.Val = false;
            if (oPEREKL != null) oPEREKL.Val = false;
            if (oPBM != null) oPBM.Val = false;
            if (oRES1 != null) oRES1.Val = false;
            if (oRES2 != null) oRES2.Val = false;
            if (oREZ != null) oREZ.Val = false;
        }
        /// <summary>
        /// Снимаем все входные сигналы (для эмулятора)
        /// </summary>
        /// <param name="_bIccOff">Надо ли выключать "Цепи управления"</param>
        public void ClearAllInputSignals(bool _bIccOff = false)
        {
            if (_bIccOff)
                set(iCC, false);
            set(iREADY, false);
            set(iCYC, false);
            set(iCNTR, false);
            set(iBASE, false);
            set(iREZ, false);
        }
    }
}
