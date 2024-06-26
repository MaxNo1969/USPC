﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace PCIE1730
{
    class Latch
    {
        public bool val;
        public Signal signal;
        public bool terminated;
        public string reason;
        public ManualResetEvent ev;
        public Latch(bool _val, Signal _signal)
        {
            val = _val;
            signal = _signal;
            terminated = false;
            reason = "";
            ev = new ManualResetEvent(false);
        }
    }
}
