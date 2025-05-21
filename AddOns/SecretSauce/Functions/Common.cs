using System;

namespace NinjaTrader.Custom.AddOns.SecretSauce.Functions
{
    public static class Common
    {
        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
