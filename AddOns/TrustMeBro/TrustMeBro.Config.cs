using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Custom.AddOns.TrustMeBro.Indicators;
using NinjaTrader.Gui;
using System.Windows.Media;


namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        public void TrendIndicatorDefault()
        {
            LevelPriceType = LevelPriceType.Close;
            ATRUpperOpacity = 0;
            ATRLowerOpacity = 0;
            EMAUpperOpacity = 0;
            EMALowerOpacity = 0;
            UpperFillOpacity = 20;
            LowerFillOpacity = 20;
            FormingFillOpacity = 20;
            TrendLineOpacity = 150;
            LevelOpacity = 100;

            UpperFillColor = Brushes.DarkRed;
            LowerFillColor = Brushes.DarkGreen;
            FormingFillColor = Brushes.SlateGray;
            TrendLineColor = Brushes.Gold;
            TrendLineDashStyle = DashStyleHelper.Solid;
            TrendLineWidth = 2;
            LevelColor = Brushes.DarkTurquoise;
            LevelDashStyle = DashStyleHelper.Dash;
            LevelWidth = 2;

            InitializeTrendIndicator();
        }

        private void InitializeTrendIndicator()
        {
            var trendIndicator = TrendIndicator();
            trendIndicator.Calculate = Calculate;
            trendIndicator.SetProperties(new TrendIndicatorConfig
            {
                Period = Period,
                PivotLimit = PivotLimit,
                LevelPriceType = LevelPriceType,
                ATRUpper = ATRUpper,
                ATRLower = ATRLower,
                EMAUpper = EMAUpper,
                EMALower = EMALower,
                UpperFillColor = UpperFillColor,
                LowerFillColor = LowerFillColor,
                FormingFillColor = FormingFillColor,
                TrendLineColor = TrendLineColor,
                TrendLineDashStyle = TrendLineDashStyle,
                TrendLineWidth = TrendLineWidth,
                LevelColor = LevelColor,
                LevelDashStyle = LevelDashStyle,
                LevelWidth = LevelWidth,
                ATRUpperOpacity = ATRUpperOpacity,
                ATRLowerOpacity = ATRLowerOpacity,
                EMAUpperOpacity = EMAUpperOpacity,
                EMALowerOpacity = EMALowerOpacity,
                UpperFillOpacity = UpperFillOpacity,
                LowerFillOpacity = LowerFillOpacity,
                FormingFillOpacity = FormingFillOpacity,
                TrendLineOpacity = TrendLineOpacity,
                LevelOpacity = LevelOpacity
            });
        }
    }
}
