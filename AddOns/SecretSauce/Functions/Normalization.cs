using System;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class Normalization
    {
        public static double NormalizeMinMax(
           double value,
           double min,
           double max,
           double tolerance = 1e-10)
        {
            if (min > max) throw new ArgumentException("Min cannot be greater than Max");

            double range = max - min;
            if (range < tolerance)
            {
                return 0;
            }
            return (value - min) / range;
        }

        public static double CalculateZScore(
            double value,
            IReadOnlyList<double> series,
            double tolerance = 1e-10)
        {
            if (series == null || series.Count <= 1) return 0.0;

            double mean = series.Average();
            double variance = series.Select(x => Math.Pow(x - mean, 2)).Average();
            double stdDev = Math.Sqrt(variance);

            if (stdDev < tolerance)
                return 0;

            return (value - mean) / stdDev;
        }
    }
}
