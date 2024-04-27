using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace USPC
{
    public static class StructHelper
    {
        public static Dictionary<string,object> convert<T>(T _struct)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            Type type = _struct.GetType();
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                var value = type.GetField(fieldInfo.Name).GetValue(_struct);
                ret.Add(fieldInfo.Name, value);
            }        

            return ret;
        }
        public static string header<T>(T _struct)
        {
            Dictionary<string, object> dict = convert<T>(_struct);
            StringBuilder ret = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in dict)
            {
                ret.Append(pair.Key + ";");
            }
            return ret.ToString();
        }
        public static string row<T>(T _struct)
        {
            Dictionary<string, object> dict = convert<T>(_struct);
            StringBuilder ret = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in dict)
            {
                ret.Append(pair.Value + ";");
            }
            return ret.ToString();
        }
    }
}
