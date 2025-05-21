using System;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class Distribution
    {
        // Abramowitz & Stegun rational approximation
        public static double NormalCdf(double z)
        {
            double t = 1.0 / (1.0 + 0.2316419 * z);
            double e = Math.Exp(-z * z / 2.0);
            double poly = t *
                          (0.31938153 +
                           t * (-0.356563782 +
                           t * (1.781477937 +
                           t * (-1.821255978 +
                           t * 1.330274429))));
            return 1.0 - e * poly / Math.Sqrt(2 * Math.PI);
        }

        // Two-sided Student-t CDF
        public static double StudentTCdf(double t, int df)
        {
            if (df <= 0) return 0.5;
            double x = df / (df + t * t);
            double a = 0.5;            // beta parameters
            double b = df / 2.0;
            double betacf = BetaCf(a, b, x);
            double betainc = betacf * Math.Exp(a * Math.Log(x) +
                              b * Math.Log(1 - x) -
                              LogBeta(a, b));
            return 0.5 + (t >= 0 ? 0.5 * betainc : -0.5 * betainc);
        }

        private static double BetaCf(double a, double b, double x,
                                     int maxIter = 100, double eps = 3e-7)
        {
            double am = 1, bm = 1, az = 1, qab = a + b,
                   qap = a + 1, qam = a - 1, bz = 1 - qab * x / qap;
            for (int m = 1; m <= maxIter; m++)
            {
                int m2 = 2 * m;
                double d = m * (b - m) * x /
                          ((qam + m2) * (a + m2));
                double ap = az + d * am;
                double bp = bz + d * bm;
                d = -(a + m) * (qab + m) * x /
                    ((a + m2) * (qap + m2));
                double app = ap + d * az;
                double bpp = bp + d * bz;
                double aold = az;
                am = ap / bpp;
                bm = bp / bpp;
                az = app / bpp;
                bz = 1.0;
                if (Math.Abs(az - aold) < eps * Math.Abs(az))
                    return az;
            }
            return az; // fallback if not converged
        }

        private static double LogBeta(double a, double b)
        {
            return GammaLn(a) + GammaLn(b) - GammaLn(a + b);
        }

        // Lanczos approximation
        private static double GammaLn(double z)
        {
            double[] p = {
                0.99999999999980993,
                676.5203681218851,
               -1259.1392167224028,
                771.32342877765313,
               -176.61502916214059,
                12.507343278686905,
               -0.13857109526572012,
                0.0000099843695780195716,
                0.00000015056327351493116
            };
            if (z < 0.5)
                return Math.Log(Math.PI) -
                       Math.Log(Math.Sin(Math.PI * z)) -
                       GammaLn(1 - z);

            z -= 1;
            double x = p[0];
            for (int i = 1; i < p.Length; i++)
                x += p[i] / (z + i);

            double t = z + p.Length - 0.5;
            return 0.5 * Math.Log(2 * Math.PI) +
                   (z + 0.5) * Math.Log(t) - t + Math.Log(x);
        }
    }
}
