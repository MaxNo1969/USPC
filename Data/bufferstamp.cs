using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    class BufferStamp
    {
        public DateTime timestamp { get; private set; }
        public int packet { get; private set; }
        public BufferStamp(DateTime _span, int _packet)
        {
            timestamp = _span;
            packet = _packet;
        }
    }
    class TimeLabels : List<BufferStamp>
    {
    }
}
