using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    class Result
    {
        public int sensors;
        public int zones;
        public List<List<double>> values;
        public void addZone()
        {
        }
        public List<int> zoneStart;
        public Result(int _sensors)
        {
            sensors = _sensors;
            zones = 0;
            values = new List<List<double>>();
            zoneStart = new List<int>();
        }
    }

    class CrossResult : Result
    {
        public CrossResult(int _sensors):base(_sensors){}
    }
    class LinearResult : Result
    {
        public LinearResult(int _sensors):base(_sensors){}
    }
    class ThickResult : Result
    {
        public ThickResult(int _sensors) : base(_sensors) { }
    }
    class TubeResult
    {
        public CrossResult cross;
        public LinearResult linear;
        public ThickResult thick;
    }
}
