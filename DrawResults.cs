using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Settings;
using Data;

namespace USPC
{
    //------------------------------------------------------------------------------
    //! Перечисление всех возможных признаков достоврености измерения
    public enum th_status
    {
        TH_OK = 0,	// толщина стенки определена
        TH_NOT_CALCED = 10     //Не расчитано
    };

    
    class DrawResults
    {
        //! Цвет барака
        private static Color Brack = Color.Red;
        //! Цвет второго класса
        private static Color Class2 = Color.Yellow;
        //! Цвет годной, хорошей трубы
        private static Color Good = Color.Green;
        //! Цвет неопределенного измерения
        //private static Color NotMeasured = Color.Gray;

        private static Color NotMeasured = Color.White;

        //! Все пороги берем из текущего типоразмера
        //! Порог брака
        public static double defectTreshold
        {
            get
            {
                return Program.typeSize.currentTypeSize.defectTreshold;
            }
        }
        //! Порог класса 2
        public static double class2Treshold
        {
            get
            {
                return Program.typeSize.currentTypeSize.class2Treshold;
            }
        }

        //! Минимальная толщина трубы для функции GetColor - берем из настроек
        public static double minThickness
        {
            get
            {
                return Program.typeSize.currentTypeSize.minDetected;
            }
        }

        //! Максимальная толщина трубы для функции GetColor - берем из настроек
        public static double maxThickness
        {
            get
            {
                return Program.typeSize.currentTypeSize.maxDetected;
            }
        }

        //! Минимальный годный участок
        //public static int minGoodLength = 1;
        //! Решение по трубе (Брак, Годно, Класс 2)
        public static string decision = "";
        //! Цвет результата (брак -красный и т.д.)
        public static Color ResultColor = Color.White;
        //! Минимальная толщина по всей трубе
        public static double min_thickness = -1;
        //! Зона с минимальной толщиной
        public static int min_thickness_zone = -1;

        //! Конструктор
        public DrawResults()
        {
        }
        //! Возвращает цвет измерения (зоны, датчика, смещения - чего угодно)
        public static Color GetThicknessColor(double measure)
        {
            // возвращает цвет зоны в зависимости от толщины в этой зоне
            if (measure == Result.notMeasured)
                return NotMeasured;
            else if (measure > minThickness)
                return Good;
            else 
                return Brack;
        }
        //! Возвращает цвет измерения (зоны, датчика, смещения - чего угодно)
        public static Color GetDefectColor(double measure)
        {
            // возвращает цвет зоны в зависимости от толщины в этой зоне
            if (measure == Result.notMeasured)
                return NotMeasured;
            else if (measure > class2Treshold)
                return Good;
            else if (measure <= defectTreshold)
                return Brack;
            else
                return Class2;
        }

        //! Принимает решение по всей трубе и вычисляет зоны отреза
        public static void MakeDecision(List<double> thickness)
        {
            decision = "Годно";			// решение не принято
            // ищем зону с минимальной значение толщины
            double min = 10e7;
            for (int i = 0; i < (int)thickness.Count; i++)
            {
                if (thickness[i] < min)
                {
                    min = thickness[i];
                    min_thickness = thickness[i];
                    min_thickness_zone = i + 1;
                }
            }
            min_thickness = (double)System.Math.Round(min_thickness, -2);
        }

        public static bool IsBrak(double measure)
        {
            return (measure <= defectTreshold);
        }
    }
}
