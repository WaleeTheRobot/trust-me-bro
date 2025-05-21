using NinjaTrader.Custom.AddOns.SecretSauce.Functions;
using NinjaTrader.NinjaScript;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Models
{
    /// <summary>
    /// Combines trend OLS regression slope, Mann-Kendall tau, and Sen's slope 
    /// with provided price windows and classifies the result into a trend score.
    /// </summary>
    public class TrendClassifier
    {
        private readonly int _period;

        public TrendClassifier(int period)
        {
            _period = period;
        }

        public double CalculateTrendScore(ISeries<double> priceSeries, int currentBar)
        {
            if (currentBar + 1 < _period)
                return 0;

            var pricesForSlope = new List<double>();
            // Reverse order due to NT series
            for (int i = _period - 1; i >= 0; i--)
                pricesForSlope.Add(priceSeries[i]);

            if (pricesForSlope.Count == 0)
            {
                return 0;
            }

            return GetNormalizedTrendScore(pricesForSlope);
        }

        /// <summary>
        /// Computes a normalized trend score between -1 and 1 by combining OLS regression slope, Mann-Kendall tau, and Sen's slope.
        /// The score integrates linear and non-parametric trend metrics, weighted by statistical significance, using recent closing prices.
        /// </summary>
        /// <param name="prices">
        /// A list of recent closing prices in chronological order for trend analysis.
        /// </param>
        /// <param name="tolerance"></param>
        /// <returns>
        /// Returns 0 for invalid inputs (e.g., null, fewer than 3 prices, or NaN values).
        /// Otherwise, returns a double between -1 and 1, where 1 indicates a strong uptrend, -1 a strong downtrend, and 0 a neutral or insignificant trend.
        /// </returns>
        private static double GetNormalizedTrendScore(IReadOnlyList<double> prices, double tolerance = 1e-6)
        {
            if (prices == null ||
                prices.Count < 3 ||
                prices.Any(double.IsNaN) ||
                Math.Abs(prices[0]) < tolerance ||
                Math.Abs(prices.Max() - prices.Min()) < tolerance ||
                Math.Abs(prices.Average()) < tolerance)
                return 0;


            // OLS-based metric
            var (slope, _, _, pValue, rSquared) = RollingSlope.RollingSlopeTest(prices, tolerance);
            double r = Math.Sign(slope) * Math.Sqrt(rSquared);
            double wR = (1 - pValue);
            double rAdj = r * wR;

            // Mann–Kendall metric
            var (tauRaw, mkP, senSlope) = MannKendall.MannKendallTest(prices);
            double tauAdj = tauRaw * (1 - mkP);

            // Sen’s slope normalized
            double relChange = senSlope * (prices.Count - 1) / prices.Average();
            relChange = Common.Clamp(relChange, -10, 10);
            double senNorm = Math.Tanh(relChange);

            // Weighted blend
            const double wTau = 0.4, wRho = 0.3, wSen = 0.3;
            double combined = wTau * tauAdj + wRho * rAdj + wSen * senNorm;

            return Common.Clamp(combined, -1, 1);
        }
    }
}
