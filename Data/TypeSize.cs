using PROTOCOL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using USPC;
using Settings;

namespace Data
{
    /// <summary>
    /// Типоразмер для сериализации
    /// </summary>
    [DisplayName("Типоразмер"), Description("Настройка типоразмеров"), Category(" ")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class _TypeSize
    {
        /// <summary>
        /// Имя типоразмера
        /// </summary>
        [DisplayName(" 1.Наименование"), Description("Наименование типоразмера в формате <Тип(\"НКТ\" или \"СБТ\">-<Диаметр>-<Дополнительный признак>"), Category("1.Типоразмер")]
        public string name { get; set; }
        /// <summary>
        /// Признак типа трубы НКТ или СБТ
        /// </summary>
        [DisplayName(" 2.Тип"), Description("Тип \"НКТ\" или \"СБТ\""), Category("1.Типоразмер")]
        public string type 
        { 
            get
            {
                string _t,_dop;
                int _d;
                if (parseName(name, out _t, out _d, out _dop))
                    return _t;
                else
                    return null;                
            } 
        }
        /// <summary>
        /// Диаметр
        /// </summary>
        [DisplayName(" 3.Диаметр"), Description("Диаметр"), Category("1.Типоразмер")]
        public int diameter
        {
            get
            {
                string _t,_dop;
                int _d;
                if (parseName(name, out _t, out _d, out _dop))
                    return _d;
                else
                    return -1;
            }
        }
        /// <summary>
        /// Дополнительные признаки из названия типоразмера (до первого разделителя)
        /// </summary>
        [DisplayName(" 4.Дополнительно"), Description("Дополнительные признаки"), Category("1.Типоразмер")]
        public string dop
        {
            get
            {
                string _t,_dop;
                int _d;
                if (parseName(name, out _t, out _d, out _dop))
                    return _dop;
                else
                    return null;
            }
        }
        /// <summary>
        /// Минимальный годный участок
        /// </summary>
        [DisplayName(" 4.Минимальный годный участок"), Description("Минимальный годный участок"), Category("1.Типоразмер")]
        public int minGoodLength { get; set; }
        /// <summary>
        /// Порог класса 1
        /// </summary>
        [DisplayName(" 5.Порог класса 1"), Description("Порог класса 1"), Category("2.Пороги")]
        public double border1 { get; set; }
        /// <summary>
        /// Порог класса 2
        /// </summary>
        [DisplayName(" 6.Порог класса 2"), Description("Порог класса 2"), Category("2.Пороги")]
        public double border2 { get; set; }
        /// <summary>
        /// Порог класса 1 внутрений
        /// </summary>
        [DisplayName(" 7.Порог класса 1 внутрений"), Description("Порог класса 1 внутрений"), Category("2.Пороги")]
        public double border1In { get; set; }
        /// <summary>
        /// Порог класса 2 внутрений
        /// </summary>
        [DisplayName(" 8.Порог класса 2 внутрений"), Description("Порог класса 2 внутрений"), Category("2.Пороги")]
        public double border2In { get; set; }
        /// <summary>
        /// Мервая зона в начале трубы
        /// </summary>
        [DisplayName(" 9.Мервая зона в начале трубы"), Description("Мервая зона в начале трубы"), Category("3.Мертвые зоны")]
        public int deadZoneStart { get; set; }
        /// <summary>
        /// Мервая зона в конце трубы
        /// </summary>
        [DisplayName("10.Мервая зона в конце трубы"), Description("Мервая зона в конце трубы"), Category("3.Мертвые зоны")]
        public int deadZoneEnd { get; set; }
        /// <summary>
        /// Преобразование в строку для вывода в PropertyGrid
        /// </summary>
        /// <returns>строка для вывода в PropertyGrid</returns>
        public override string ToString()
        {
            return name;
        }
        #region Разбор имени типоразмера
        /// <summary>
        /// Возможные типы труб
        /// </summary>
        private static readonly string[] _types = { "НКТ", "СБТ" };
        /// <summary>
        /// Возможные диаметры для НКТ
        /// </summary>
        private static readonly int[] _dNKT = { 73, 89, 102, 114 };
        /// <summary>
        /// Возможные диаметры для СБТ
        /// </summary>
        private static readonly int[] _dSBT = { 73, 89, 102, 114, 127 };
        /// <summary>
        /// Проверка имени типоразмера на соответствие для разбора
        /// разобранные части записываются в выходные переменные
        /// </summary>
        /// <param name="_str">Строка для разбора</param>
        /// <param name="_type">out: тип трубы НКТ или СБТ</param>
        /// <param name="_diameter">out: Диаметр трубы</param>
        /// <param name="_dop">out: оставшаяся часть в имени типоразмера</param>
        /// <returns>true - если удалось разобрать имя типоразмера</returns>
        public static bool parseName(string _str, out string _type, out int _diameter, out string _dop)
        {
            //Имя задаем в виде (НКТ|СБТ)-(диаметр)-(доп.признак) 
            _type = string.Empty;
            _diameter = 0;
            _dop = string.Empty;
            if (_str == null) return false;
            string[] parsedTypeSize = _str.Split(new char[] { '-', ' ' });
            if (parsedTypeSize.Length < 2) return false;
            if (parsedTypeSize[0] == "НКТ" || parsedTypeSize[0] == "СБТ")
            {
                _type = parsedTypeSize[0];
                int diam;
                try
                {
                    diam = Convert.ToInt32(parsedTypeSize[1]);
                }
                catch
                {
                    return false;
                }
                //Проверка допустимых параметров для типов труб
                if ((_type == "НКТ" && !Array.Exists<int>(_dNKT, el => el == diam))
                    || (_type == "СБТ" && !Array.Exists<int>(_dSBT, el => el == diam))) return false;
                _diameter = diam;
                if (parsedTypeSize.Length > 2) _dop = parsedTypeSize[2];
                return true;
            }
            return false;
        }
        #endregion
    };

    /// <summary>
    /// Типоразмер
    /// </summary>
    [Serializable]
    public class TypeSize
    {
        /// <summary>
        /// Делегат смены типоразмера
        /// </summary>
        public OnChangeSettings onChangeTypeSize = saveToAppSettings;

        private static void saveToAppSettings(object[] _param)
        {
            AppSettings.settings.currenTypeSizeName = (string)_param[0];
            AppSettings.settings.changed = true;
        }

        /// <summary>
        /// Все типоразмеры (при инициализации будут считаны из базы)
        /// </summary>
        private List<_TypeSize> tss = null;
        /// <summary>
        /// Текущий выбранный типоразмер - из него будем брать все парвметры
        /// </summary>
        public _TypeSize currentTypeSize = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public TypeSize()
        {
            tss = AppSettings.s.tss;
            string _current = AppSettings.s.currenTypeSizeName;
            if (tss.Count > 0)
            {
                if (_current != null)
                    select(_current);
                else
                    select(AppSettings.settings.tss[0].name);
            }
        }

        /// <summary>
        /// Выбор типоразмера по имени
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public bool select(string _str)
        {
            if (_str == null)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не задан текущий типоразмер");
                return false;
            }
            currentTypeSize = tss.Find(x => x.name == _str);
            if (currentTypeSize == null)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Типоразмер \"{2}\" не найден в списке типоразмеров.", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _str);
                return false;
            }
            log.add(LogRecord.LogReason.info, "{0}: {1}: Выбран типоразмер: \"{2}\"", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, currentTypeSize.name);
            if(onChangeTypeSize != null)onChangeTypeSize.Invoke(new object[] { currentTypeSize.name });
            return true;
        }

        /// <summary>
        /// Название выбранного типоразмера
        /// </summary>
        public string name { get { return currentTypeSize.name; } }
        /// <summary>
        /// Диаметр
        /// </summary>
        public double diameter { get { return currentTypeSize.diameter; } }
        /// <summary>
        /// Порог брака
        /// </summary>
        public double defectTreshold { get { return currentTypeSize.border1; } }
        /// <summary>
        /// Порог класса2
        /// </summary>
        public double class2Treshold { get { return currentTypeSize.border2; } }
        /// <summary>
        /// Флаг, что типоразмер выбран
        /// </summary>
        public bool selected { get { return currentTypeSize != null; } }

        /// <summary>
        /// Заполнение списка всех типоразмеров
        /// </summary>
        /// <returns>Список типоразмеров</returns>
        public string[] allTypesizes()
        {
            string[] ret = new string[tss.Count];
            for (int i = 0; i < tss.Count; i++) ret[i] = tss[i].name;
            return ret;
        }
    }
}
