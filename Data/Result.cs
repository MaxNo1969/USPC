using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USPC;
using PROTOCOL;

namespace Data
{
    class Result
    {
        public int crossSensors = 4;
        public int linearSensors = 4;
        public int thicknesSensors = 4;
        public int zones;

        public List<List<byte>> crossValues;
        public List<List<byte>> linearValues;
        public List<List<double>> thicknesValues;
        
        public void addZone()
        {
        }
        public List<int>[] zoneStart = new List<int>[2];

        public Result()
        {
            zones = 0;
            crossValues = new List<List<byte>>();
            for (int i = 0; i < crossSensors; i++)
            {
                crossValues.Add(new List<byte>());
            }
            linearValues = new List<List<byte>>();
            for (int i = 0; i < linearSensors; i++)
            {
                linearValues.Add(new List<byte>());
            }
            thicknesValues = new List<List<double>>();
            for (int i = 0; i < thicknesSensors; i++)
            {
                thicknesValues.Add(new List<double>());
            }
            for (int i = 0; i < 2; i++)
            {
                zoneStart[i] = new List<int>();
                zoneStart[i].Add(0);
            }
        }
        public void Clear()
        {
            for (int i = 0; i < crossSensors; i++)
            {
                crossValues[i].Clear();
            }
            //crossValues.Clear();
            for (int i = 0; i < linearSensors; i++)
            {
                linearValues[i].Clear();
            }
            //linearValues.Clear();
            for (int i = 0; i < thicknesSensors; i++)
            {
                thicknesValues[i].Clear();
            }
            //thicknesValues.Clear();
        }

    }

}
