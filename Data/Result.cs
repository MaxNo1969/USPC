using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;

namespace Data
{
    class Result
    {
        public int sensors;
        public int zones;
        public int board;

        public List<List<double>> values;
        public virtual void addZone()
        {
            int zBegin = zoneStart.Last();
            int zEnd = Program.data[board].currentOffsetFrames;
            Program.data[board].OffsetCounter(zEnd);
            zoneStart.Add(zEnd);
        }
        public List<int> zoneStart;
        public Result(int _board,int _sensors)
        {
            board = _board;
            sensors = _sensors;
            zones = 0;
            values = new List<List<double>>();
            for (int i = 0; i < sensors; i++)
            {
                values.Add(new List<double>());
            }
            zoneStart = new List<int>();
            zoneStart.Add(0);
        }
        public void Clear()
        {
            for (int i = 0; i < sensors; i++)
            {
                values[i].Clear();
            }
        }

    }

    class CrossResult : Result
    {
        public CrossResult(int _sensors):base(0,_sensors){}
        public override void addZone()
        {
            Random r = new Random();
            base.addZone();
            int zBegin = zoneStart[zoneStart.Count() - 2];
            int zEnd = zoneStart[zoneStart.Count() - 1];
            
            for (int i = 0; i < sensors; i++)
            {
                values[i].Add(90.0 + r.Next(10));
            }
        }
    }
    class LinearResult : Result
    {
        public LinearResult(int _sensors):base(0,_sensors){}
        public override void addZone()
        {
            Random r = new Random();
            base.addZone();
            for (int i = 0; i < sensors; i++)
            {
                values[i].Add(90.0 + r.Next(10));
            }
        }
    }
    class ThickResult : Result
    {
        public ThickResult(int _sensors) : base(1,_sensors) { }
        public override void addZone()
        {
            Random r = new Random();
            base.addZone();
            for (int i = 0; i < sensors; i++)
            {
                values[i].Add(90.0 + r.Next(10));
            }
        }
    }
    class TubeResult
    {
        public CrossResult cross;
        public LinearResult linear;
        public ThickResult thick;
        public TubeResult()
        {
            cross = new CrossResult(4);
            linear = new LinearResult(4);
            thick = new ThickResult(4);
        }
        public void AddNewZone()
        {
            cross.addZone();
            linear.addZone();
            thick.addZone();
        }

        internal void Clear()
        {
            cross.Clear();
            linear.Clear();
            thick.Clear();
        }
    }
}
