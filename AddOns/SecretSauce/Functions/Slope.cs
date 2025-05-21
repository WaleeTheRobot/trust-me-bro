using System;
using System.Collections.Generic;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class Slope
    {
        public static double CalculateRegressionSlope(
            IReadOnlyList<double> series,
            double tolerance = 1e-10)
        {
            if (series == null) return 0.0;
            int n = series.Count;
            if (n < 2) return 0.0;

            double sumX = 0.0, sumY = 0.0, sumXY = 0.0, sumXX = 0.0;

            for (int i = 0; i < n; i++)
            {
                double y = series[i];
                sumX += i;
                sumY += y;
                sumXY += i * y;
                sumXX += i * i;
            }

            double denom = n * sumXX - sumX * sumX;
            if (Math.Abs(denom) < tolerance)
                return 0.0;

            return (n * sumXY - sumX * sumY) / denom;
        }
    }
}
