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
        [DisplayName(" 1.Наименование"), Description("Наименование типоразмера"), Category("1.Типоразмер")]
        public string name { get; set; }
        /// <summary>
        /// Диаметр
        /// </summary>
        [DisplayName(" 2.Диаметр"), Description("Диаметр"), Category("1.Типоразмер")]
        public double diameter {get;set;}
        /// <summary>
        /// Минимальный годный участок
        /// </summary>
        [DisplayName(" 3.Минимальный годный участок"), Description("Минимальный годный участок"), Category("1.Типоразмер")]
        public int minGoodLength { get; set; }
        /// <summary>
        /// Порог класса 1
        /// </summary>
        [DisplayName(" 4.Порог брака"), Description("Порог класса 1"), Category("2.Пороги")]
        public double defectTreshold { get; set; }
        /// <summary>
        /// Порог класса 2
        /// </summary>
        [DisplayName(" 5.Порог класса 2"), Description("Порог класса 2"), Category("2.Пороги")]
        public double class2Treshold { get; set; }
        /// <summary>
        /// Минимальная толщина
        /// </summary>
        [DisplayName(" 6.Минимальная толщина"), Description("Минимальная толщина"), Category("2.Пороги")]
        public double minDetected { get; set; }
        /// <summary>
        /// Максимальная толщина
        /// </summary>
        [DisplayName(" 7.Максимальная толщина"), Description("Максимальная толщина"), Category("2.Пороги")]
        public double maxDetected { get; set; }
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
        /// Имя файла настроек
        /// </summary>
        [DisplayName("Имя файла настроек"), Description("Имя файла настроек"), Category("4.Настройки")]
        public string configName { get; set; }
        /// <summary>
        /// Преобразование в строку для вывода в PropertyGrid
        /// </summary>
        /// <returns>строка для вывода в PropertyGrid</returns>
        public override string ToString()
        {
            return name;
        }
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
        public double defectTreshold { get { return currentTypeSize.defectTreshold; } }
        /// <summary>
        /// Порог класса2
        /// </summary>
        public double class2Treshold { get { return currentTypeSize.class2Treshold; } }
        /// <summary>
        /// Минимальная толщина
        /// </summary>
        public double minDetected { get { return currentTypeSize.minDetected; } }
        /// <summary>
        /// Максимальная толщина
        /// </summary>
        public double maxDetected { get { return currentTypeSize.maxDetected; } }


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
