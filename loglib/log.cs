using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PROTOCOL
{
    public class LogRecord
    {
        public DateTime dt { get; private set; }
        public enum LogReason { debug = 0, info = 1, warning = 2, error = 3 };
        public LogReason reason { get; private set; }
        public string text { get; private set; }
        public LogRecord(string _text = "", LogReason _reason = LogReason.info)
        {
            dt = DateTime.Now;
            text = _text;
            reason = _reason;
        }
    }
    public static class log
    {
        public delegate void OnLogChanged();
        public static OnLogChanged onLogChanged = null;

        static Queue<LogRecord> p = new Queue<LogRecord>();

        public static void add(LogRecord.LogReason _reason, string _s, params object[] _args)
        {
            string s = _s;
            if(_args != null)
                s = string.Format(_s, _args);
            Debug.WriteLine(s);
            p.Enqueue(new LogRecord(s, _reason));
            if (onLogChanged != null) onLogChanged();
        }
        public static LogRecord get() 
        {
            if (p.Count() > 0)
                return p.Dequeue();
            else
                return null;
        }
        public static LogRecord peek()
        {
            if (p.Count() > 0)
                return p.Peek();
            else
                return null;
        }
        public static int size() { return p.Count; }
    }
}
