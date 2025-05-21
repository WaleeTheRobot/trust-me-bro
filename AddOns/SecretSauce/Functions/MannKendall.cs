using System;
using System.Collections.Generic;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class MannKendall
    {
        /// <summary>
        /// Mann–Kendall τ and two-sided p value (normal approximation)
        /// together with Sen’s median slope.
        /// </summary>
        public static (double tau,
                       double pValue,
                       double senSlope)
            MannKendallTest(IReadOnlyList<double> series)
        {
            int n = series?.Count ?? 0;
            if (n < 3) return (0, 1, 0);

            long S = 0;
            for (int i = 0; i < n - 1; i++)
                for (int j = i + 1; j < n; j++)
                    S += Math.Sign(series[j] - series[i]);

            // Variance ignoring ties (good enough for price series)
            double varS = n * (n - 1) * (2 * n + 5) / 18.0;

            double z = S > 0 ? (S - 1) / Math.Sqrt(varS)
                     : S < 0 ? (S + 1) / Math.Sqrt(varS)
                     : 0.0;
            double p = 2 * (1 - Distribution.NormalCdf(Math.Abs(z)));

            // Sen’s slope (median of all pair-wise slopes)
            List<double> slopes = new();
            for (int i = 0; i < n - 1; i++)
                for (int j = i + 1; j < n; j++)
                    slopes.Add((series[j] - series[i]) / (j - i));
            slopes.Sort();
            double sen = slopes.Count % 2 == 1
                       ? slopes[slopes.Count / 2]
                       : 0.5 * (slopes[slopes.Count / 2 - 1] +
                                slopes[slopes.Count / 2]);

            double tau = 2.0 * S / (n * (n - 1));
            return (tau, p, sen);
        }
    }
}
