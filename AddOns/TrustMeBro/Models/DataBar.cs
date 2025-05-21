namespace NinjaTrader.Custom.AddOns.TrustMeBro.Models
{
    public class DataBar
    {
        public int BarNumber { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        public DataBar(int barNumber, double open, double high, double low, double close)
        {
            BarNumber = barNumber;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }
    }
}
