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
            zoneStart.Add(Program.data[board].currentOffsetFrames);
        }
        public List<int> zoneStart;
        public Result(int _board,int _sensors)
        {
            board = _board;
            sensors = _sensors;
            zones = 0;
            values = new List<List<double>>();
            zoneStart = new List<int>();
            zoneStart.Add(0);
        }
    }

    class CrossResult : Result
    {
        public CrossResult(int _sensors):base(0,_sensors){}
        public override void addZone(int _board)
        {
            base.addZone();
        }
    }
    class LinearResult : Result
    {
        public LinearResult(int _sensors):base(0,_sensors){}
        public override void addZone(int _board)
        {
            base.addZone();
        }
    }
    class ThickResult : Result
    {
        public ThickResult(int _sensors) : base(1,_sensors) { }
        public override void addZone(int _board)
        {
            base.addZone();
        }
    }
    class TubeResult
    {
        public CrossResult cross;
        public LinearResult linear;
        public ThickResult thick;
    }
}
