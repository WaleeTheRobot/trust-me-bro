namespace NinjaTrader.Custom.AddOns.TrustMeBro.Models
{
    public class Pivots
    {
        public class PivotBar
        {
            public int BarNumber { get; set; }
            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }

            public PivotBar(int barNumber, double open, double high, double low, double close)
            {
                BarNumber = barNumber;
                Open = open;
                High = high;
                Low = low;
                Close = close;
            }
        }

        public class PivotPoint
        {
            public PivotBar PivotBar { get; set; }
            public bool IsHigh { get; set; }
            public int BrokenBarNumber { get; set; }
            public bool IsBroken { get; set; }
            public bool DisplayLevel { get; set; }
            public bool IsDrawn { get; set; }

            public PivotPoint(PivotBar pivotBar, bool isHigh, int brokenBarNumber = 0, bool isBroken = false, bool displayLevel = true)
            {
                PivotBar = pivotBar;
                IsHigh = isHigh;
                BrokenBarNumber = brokenBarNumber;
                IsBroken = isBroken;
                DisplayLevel = displayLevel;
                IsDrawn = false;
            }
        }
    }
}
