using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript;
using System.Windows.Media;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Indicators
{
    public interface ITrendIndicator
    {
        int Period { get; set; }
        int PivotLimit { get; set; }
        LevelPriceType LevelPriceType { get; set; }
        Series<double> ATRUpper { get; set; }
        Series<double> ATRLower { get; set; }
        Series<double> EMAUpper { get; set; }
        Series<double> EMALower { get; set; }
        Brush UpperFillColor { get; set; }
        Brush LowerFillColor { get; set; }
        Brush FormingFillColor { get; set; }
        Brush TrendLineColor { get; set; }
        DashStyleHelper TrendLineDashStyle { get; set; }
        int TrendLineWidth { get; set; }
        Brush LevelColor { get; set; }
        DashStyleHelper LevelDashStyle { get; set; }
        int LevelWidth { get; set; }
        byte ATRUpperOpacity { get; set; }
        byte ATRLowerOpacity { get; set; }
        byte EMAUpperOpacity { get; set; }
        byte EMALowerOpacity { get; set; }
        byte UpperFillOpacity { get; set; }
        byte LowerFillOpacity { get; set; }
        byte FormingFillOpacity { get; set; }
        byte TrendLineOpacity { get; set; }
        byte LevelOpacity { get; set; }
    }
}
