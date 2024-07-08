using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USPC.Data
{
    public static class ThickConverter
    {
        public static double TofToMm(UInt32 _tof)
        {
            //return 2.5e-6 * _tof * Program.scopeVelocity;
            //нс * м/с
            return (double)_tof * Program.scopeVelocity/2.0 / 1000000.0;
        }

        public static double TofToMm(AcqAscan _scan)
        {
            return TofToMm(_scan.G1Tof * 5);
        }

        public static uint MmToTof(double _mm)
        {
            return (uint)((double)_mm * 100000.0 * 2 / Program.scopeVelocity);
        }
    }
}
