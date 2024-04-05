using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;

namespace USPC
{
    class CollectionTypeConverter : TypeConverter
    {
        /// <summary>
        /// Только в строку
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        /// <summary>
        /// И только так
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return "<" + (value as ICollection).Count + ">";
        }
    }
    /// <summary>
    /// TypeConverter для списка типоразмеров
    /// </summary>
    class TypesizeTypeConverter : StringConverter
    {
            /// <summary>
            /// Будем предоставлять выбор из списка
            /// </summary>
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            /// <summary>
            /// ... и только из списка
            /// </summary>
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                // false - можно вводить вручную
                // true - только выбор из списка
                return true;
            }
            /// <summary>
            /// А вот и список
            /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // возвращаем список строк из настроек программы
                // (базы данных, интернет и т.д.)
                return new StandardValuesCollection(AppSettings.settings.tss);
            }
        }
    class BooleanTypeConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return (bool)value ? "Да" : "Нет";
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == "Да";
        }
    }
    class EnumTypeConverter : EnumConverter
    {
        private Type _enumType;
        public EnumTypeConverter(Type type)
            : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
          Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value, Type destType)
        {
            FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }
        public string Desc(object _o)
        {
            return (ConvertTo(null, null, _o, null) as string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
          Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                DescriptionAttribute dna =
                  (DescriptionAttribute)Attribute.GetCustomAttribute(
                    fi, typeof(DescriptionAttribute));

                if ((dna != null) && ((string)value == dna.Description))
                    return Enum.Parse(_enumType, fi.Name);
            }

            return Enum.Parse(_enumType, (string)value);
        }
    }
}