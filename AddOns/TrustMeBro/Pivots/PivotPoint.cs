using NinjaTrader.Custom.AddOns.TrustMeBro.Common;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Pivots
{
    public class PivotPoint
    {
        public BarData BarData { get; set; }
        public bool IsHigh { get; set; }
        public int BrokenBarNumber { get; set; }
        public bool IsBroken { get; set; }
        public bool DisplayLevel { get; set; }
        public bool IsDrawn { get; set; }

        public PivotPoint(BarData barData, bool isHigh, int brokenBarNumber = 0, bool isBroken = false, bool displayLevel = true)
        {
            BarData = barData;
            IsHigh = isHigh;
            BrokenBarNumber = brokenBarNumber;
            IsBroken = isBroken;
            DisplayLevel = displayLevel;
            IsDrawn = false;
        }
    }
}
