using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript;
using System.Windows.Media;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Indicators
{
    public class TrendIndicatorConfig : ITrendIndicator
    {
        public int Period { get; set; }
        public int PivotLimit { get; set; }
        public LevelPriceType LevelPriceType { get; set; }
        public Series<double> ATRUpper { get; set; }
        public Series<double> ATRLower { get; set; }
        public Series<double> EMAUpper { get; set; }
        public Series<double> EMALower { get; set; }
        public Brush UpperFillColor { get; set; }
        public Brush LowerFillColor { get; set; }
        public Brush FormingFillColor { get; set; }
        public Brush TrendLineColor { get; set; }
        public DashStyleHelper TrendLineDashStyle { get; set; }
        public int TrendLineWidth { get; set; }
        public Brush LevelColor { get; set; }
        public DashStyleHelper LevelDashStyle { get; set; }
        public int LevelWidth { get; set; }
        public byte ATRUpperOpacity { get; set; }
        public byte ATRLowerOpacity { get; set; }
        public byte EMAUpperOpacity { get; set; }
        public byte EMALowerOpacity { get; set; }
        public byte UpperFillOpacity { get; set; }
        public byte LowerFillOpacity { get; set; }
        public byte FormingFillOpacity { get; set; }
        public byte TrendLineOpacity { get; set; }
        public byte LevelOpacity { get; set; }

        public TrendIndicatorConfig() { }

        public TrendIndicatorConfig(ITrendIndicator source)
        {
            Period = source.Period;
            PivotLimit = source.PivotLimit;
            LevelPriceType = source.LevelPriceType;
            ATRUpper = source.ATRUpper;
            ATRLower = source.ATRLower;
            EMAUpper = source.EMAUpper;
            EMALower = source.EMALower;
            UpperFillColor = source.UpperFillColor;
            LowerFillColor = source.LowerFillColor;
            FormingFillColor = source.FormingFillColor;
            TrendLineColor = source.TrendLineColor;
            TrendLineDashStyle = source.TrendLineDashStyle;
            TrendLineWidth = source.TrendLineWidth;
            LevelColor = source.LevelColor;
            LevelDashStyle = source.LevelDashStyle;
            LevelWidth = source.LevelWidth;
            ATRUpperOpacity = source.ATRUpperOpacity;
            ATRLowerOpacity = source.ATRLowerOpacity;
            EMAUpperOpacity = source.EMAUpperOpacity;
            EMALowerOpacity = source.EMALowerOpacity;
            UpperFillOpacity = source.UpperFillOpacity;
            LowerFillOpacity = source.LowerFillOpacity;
            FormingFillOpacity = source.FormingFillOpacity;
            TrendLineOpacity = source.TrendLineOpacity;
            LevelOpacity = source.LevelOpacity;
        }
    }
}
