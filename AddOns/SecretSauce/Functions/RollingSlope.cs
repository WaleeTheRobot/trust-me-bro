using System;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class RollingSlope
    {
        /// <summary>
        /// Ordinary-least-squares regression of price on bar index
        /// over the last N points.
        /// </summary>
        /// <returns>(slope, stdErr, tStat, pValue, rSquared)</returns>
        public static (double slope,
                       double stdErr,
                       double tStat,
                       double pValue,
                       double rSquared)
            RollingSlopeTest(IReadOnlyList<double> y,
                             double tolerance = 1e-10)
        {
            int n = y?.Count ?? 0;
            if (n < 3) return (0, 0, 0, 1, 0);

            double sumX = (n - 1) * n / 2.0;
            double sumXX = (n - 1) * n * (2 * n - 1) / 6.0;
            double meanX = sumX / n;
            double meanY = y.Average();

            double Sxy = 0, Sxx = 0, SST = 0, RSS = 0;
            for (int i = 0; i < n; i++)
            {
                double dx = i - meanX;
                double dy = y[i] - meanY;
                Sxy += dx * dy;
                Sxx += dx * dx;
                SST += dy * dy;
            }

            if (Math.Abs(Sxx) < tolerance) return (0, 0, 0, 1, 0);

            double slope = Sxy / Sxx;
            double intercept = meanY - slope * meanX;

            for (int i = 0; i < n; i++)
            {
                double fitted = intercept + slope * i;
                RSS += (y[i] - fitted) * (y[i] - fitted);
            }

            double stdErr = Math.Sqrt(RSS / (n - 2)) / Math.Sqrt(Sxx);
            double tStat = slope / (stdErr < tolerance ? 1 : stdErr);
            double pValue = 2 * (1 - Distribution.StudentTCdf(Math.Abs(tStat), n - 2));
            double r2 = 1 - RSS / SST;

            return (slope, stdErr, tStat, pValue, r2);
        }
    }
}
