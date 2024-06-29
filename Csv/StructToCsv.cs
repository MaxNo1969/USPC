using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace USPC
{
    static class StructToCsv
    {
        public static void writeCsv<T>(string _fileName, T[] _array)
        {
            bool noNeedHeader = File.Exists(_fileName);
            using (StreamWriter writer = new StreamWriter(_fileName, true))
            {
                if (!noNeedHeader) writer.WriteLine(StructHelper.header<T>(_array[0]));
                for(int i = 0;i<_array.Length;i++)
                {
                    writer.WriteLine(StructHelper.row<T>(_array[i]));
                }
            }
        }
    }
}
