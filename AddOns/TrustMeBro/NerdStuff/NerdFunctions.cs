using System;
using System.Collections.Generic;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.NerdStuff
{
    public static class NerdFunctions
    {
        public static double RegressionSlope(
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

        public static double Autocorrelation(
            IReadOnlyList<double> series,
            int lag = 1,
            double tolerance = 1e-10)
        {
            if (series == null || series.Count <= lag)
                return 0.0;

            int n = series.Count;
            double mean = 0.0;

            for (int i = 0; i < n; i++)
                mean += series[i];
            mean /= n;

            double num = 0.0, den = 0.0;

            for (int i = lag; i < n; i++)
            {
                double d0 = series[i] - mean;
                double dLag = series[i - lag] - mean;
                num += d0 * dLag;
            }

            for (int i = 0; i < n; i++)
            {
                double d = series[i] - mean;
                den += d * d;
            }

            return Math.Abs(den) < tolerance ? 0.0 : num / den;
        }
    }
}
