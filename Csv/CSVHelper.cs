using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PROTOCOL;
using System.Diagnostics;

namespace USPC
{
    class CSVHelper
    {
        public static char[] separator = new char[] { ';' };
        public static Dictionary<string,List<int>> readCsv(string _fName = null)
        {
            string s;
            Dictionary<string,List<int>> data = new Dictionary<string,List<int>>();
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = "csv",
                Filter = "Файлы CSV (*.csv)|*.csv|Все файлы (*.*)|*.*"
            };
            if (_fName != null || ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (_fName == null) _fName = ofd.FileName;
                using (StreamReader reader = new StreamReader(_fName, false))
                {
                    //Читаем заголовок
                    s = reader.ReadLine();
                    string[] splitted = s.Split(new char[] { ';' });
                    for (int i = 0; i < splitted.Count(); i++)
                    {
                        data.Add(splitted[i], new List<int>());
                    }
                    while ((s = reader.ReadLine()) != null)
                    {
                        int i;
                        int val;
                        try
                        {
                            string[] values = s.Split(new char[] { ';' });
                            for (i = 0; i < values.Count(); i++)
                            {
                                if (values[i] != null && values[i] != "")
                                {
                                    val = Convert.ToInt32(values[i]);
                                    data[splitted[i]].Add(val);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.add(LogRecord.LogReason.error, "{0}: {1}: Ошибка: {2}", "CSVHelper", "readCsv", ex.Message);
                            return null;
                        }
                    }
                    reader.Close();
                    log.add(LogRecord.LogReason.info, "{0}: {1}: Считано {2} строк", "CSVHelper", "readCsv", data.First().Value.Count);
                    return data;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool writeCsv(Dictionary<string, List<int>> _data, string _fName = null)
        {
            string s;
            SaveFileDialog ofd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "csv",
                Filter = "Файлы CSV (*.csv)|*.csv|Все файлы (*.*)|*.*"
            };
            if (_fName != null || ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (_fName == null) _fName = ofd.FileName;
                using (StreamWriter writer = new StreamWriter(_fName, false))
                {
                    //Пишем заголовок
                    s = "";
                    foreach (string item in _data.Keys) s += item + ";";
                    writer.WriteLine(s);
                    for (int i = 0; i < _data.First().Value.Count; i++)
                    {
                        s = "";
                        foreach (List<int> lst in _data.Values) s += lst[i].ToString() + ";";
                        writer.WriteLine(s);
                    }
                    writer.Close();
                    log.add(LogRecord.LogReason.info, "{0}: {1}:Файл: {2} Записано {3} строк", "CSVHelper", "writeCsv", _fName, _data.First().Value.Count);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
