using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace USPC
{
    class Program
    {
        static void Main(string[] args)
        {
            Ascan ascan = new Ascan();
            Dictionary<string, object> dict = StructHelper.convert<Ascan>(ascan);
            foreach (KeyValuePair<string, object> pair in dict)
            {
                Console.WriteLine("{0} = {1}", pair.Key, pair.Value);

            }
        }
    }
}
