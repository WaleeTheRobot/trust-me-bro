#region Using declarations
using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Custom.AddOns.TrustMeBro.Indicators;
using NinjaTrader.Custom.AddOns.TrustMeBro.Pivots;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript.DrawingTools;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
#endregion

namespace NinjaTrader.NinjaScript.Indicators
{
    public class TrendIndicator : Indicator, ITrendIndicator
    {
        private EMA _emaHigh, _emaLow, _ema;
        private ATR _atr;

        #region Properties

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

        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"";
                Name = "_TrendIndicator";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                AddPlot(Brushes.SlateGray, "ATR Upper");
                AddPlot(Brushes.SlateGray, "ATR Lower");
                AddPlot(Brushes.DarkRed, "EMA Upper");
                AddPlot(Brushes.DarkGreen, "EMA Lower");
            }
            else if (State == State.DataLoaded)
            {
                _emaHigh = EMA(High, Period);
                _emaLow = EMA(Low, Period);
                _ema = EMA(Period);
                _atr = ATR(Period);
            }
            else if (State == State.Configure)
            {
                Plots[0].Opacity = ATRUpperOpacity;
                Plots[1].Opacity = ATRLowerOpacity;
                Plots[2].Opacity = EMAUpperOpacity;
                Plots[3].Opacity = EMALowerOpacity;
            }
        }

        public void SetProperties(ITrendIndicator config)
        {
            Period = config.Period;
            PivotLimit = config.PivotLimit;
            LevelPriceType = config.LevelPriceType;
            ATRUpper = config.ATRUpper;
            ATRLower = config.ATRLower;
            EMAUpper = config.EMAUpper;
            EMALower = config.EMALower;
            UpperFillColor = config.UpperFillColor;
            LowerFillColor = config.LowerFillColor;
            FormingFillColor = config.FormingFillColor;
            TrendLineColor = config.TrendLineColor;
            TrendLineDashStyle = config.TrendLineDashStyle;
            TrendLineWidth = config.TrendLineWidth;
            LevelColor = config.LevelColor;
            LevelDashStyle = config.LevelDashStyle;
            LevelWidth = config.LevelWidth;
            ATRUpperOpacity = config.ATRUpperOpacity;
            ATRLowerOpacity = config.ATRLowerOpacity;
            EMAUpperOpacity = config.EMAUpperOpacity;
            EMALowerOpacity = config.EMALowerOpacity;
            UpperFillOpacity = config.UpperFillOpacity;
            LowerFillOpacity = config.LowerFillOpacity;
            FormingFillOpacity = config.FormingFillOpacity;
            TrendLineOpacity = config.TrendLineOpacity;
            LevelOpacity = config.LevelOpacity;
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar <= Period)
            {
                ATRUpper.Reset();
                ATRLower.Reset();
                EMAUpper.Reset();
                EMALower.Reset();

                return;
            }
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            //base.OnRender(chartControl, chartScale);

            //if (_pivots == null || _pivots.Count == 0 || CurrentBar < Period || ChartBars == null || ChartBars.Bars.Count == 0 || RenderTarget == null)
            //    return;

            //PivotPoint lastPivot = _pivots[_pivots.Count - 1];
            //int lastPivotBarNumber = lastPivot.PivotBar.BarNumber;
            //int lastPivotBarsAgo = CurrentBar - lastPivotBarNumber;

            //if (lastPivotBarsAgo < 0 || lastPivotBarsAgo > CurrentBar)
            //    return;

            //double lastPivotPrice = GetPivotPrice(lastPivot);
            //double targetPrice = _isLookingForHigh ? _currentMaxHigh : _currentMinLow;
            //int targetBarsAgo = _isLookingForHigh ? CurrentBar - _maxHighBar : CurrentBar - _minLowBar;

            //if (targetBarsAgo < 0 || targetBarsAgo > CurrentBar)
            //    return;

            //if (lastPivotBarNumber < 0 || lastPivotBarNumber >= ChartBars.Bars.Count ||
            //    _isLookingForHigh && (_maxHighBar < 0 || _maxHighBar >= ChartBars.Bars.Count) ||
            //    !_isLookingForHigh && (_minLowBar < 0 || _minLowBar >= ChartBars.Bars.Count))
            //    return;

            //float x1 = chartControl.GetXByBarIndex(ChartBars, lastPivotBarNumber);
            //float x2 = chartControl.GetXByBarIndex(ChartBars, _isLookingForHigh ? _maxHighBar : _minLowBar);
            //float y1 = chartScale.GetYByValue(lastPivotPrice);
            //float y2 = chartScale.GetYByValue(targetPrice);

            //SolidColorBrush solidBrush = TrendLineColor as SolidColorBrush;
            //if (solidBrush == null)
            //    return;
            //Color baseColor = solidBrush.Color;
            //Color4 dxColor = new Color4(
            //    baseColor.R / 255f,
            //    baseColor.G / 255f,
            //    baseColor.B / 255f,
            //    TrendLineOpacity / 255f
            //);
            //using (SharpDX.Direct2D1.SolidColorBrush dashBrush = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, dxColor))
            //using (StrokeStyle dashStyle = new(RenderTarget.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }))
            //{
            //    RenderTarget.DrawLine(
            //        new Vector2(x1, y1),
            //        new Vector2(x2, y2),
            //        dashBrush,
            //        2f,
            //        dashStyle
            //    );
            //}
        }

        private double GetPivotLevelPrice(PivotPoint pivot)
        {
            switch (LevelPriceType)
            {
                case LevelPriceType.Close:
                    return pivot.BarData.Close;
                case LevelPriceType.HighLow:
                    return pivot.IsHigh ? pivot.BarData.High : pivot.BarData.Low;
                default:
                    return pivot.BarData.Close;
            }
        }

        private static double GetPivotPrice(PivotPoint pivot)
        {
            return pivot.IsHigh ? pivot.BarData.High : pivot.BarData.Low;
        }

        private void DrawPivotLevels()
        {
            //foreach (var pivot in _pivots)
            //{
            //    if (!pivot.DisplayLevel)
            //    {
            //        string removeTag = $"pivot_{pivot.PivotBar.BarNumber}";
            //        RemoveDrawObject(removeTag);
            //        continue;
            //    }

            //    double pivotPrice = GetPivotLevelPrice(pivot);
            //    int startBar = pivot.PivotBar.BarNumber;
            //    SolidColorBrush solidBrush = LevelColor as SolidColorBrush;
            //    if (solidBrush != null)
            //    {
            //        Color baseColor = solidBrush.Color;
            //        Color levelColorWithOpacity = Color.FromArgb(
            //            LevelOpacity,
            //            baseColor.R,
            //            baseColor.G,
            //            baseColor.B
            //        );
            //        SolidColorBrush brushWithOpacity = new SolidColorBrush(levelColorWithOpacity);
            //        string tag = $"pivot_{startBar}";
            //        Draw.Line(this, tag, false, CurrentBar - startBar, pivotPrice, 0, pivotPrice, brushWithOpacity, LevelDashStyle, LevelWidth);
            //    }
            //}
        }

        private void DrawFill()
        {
            if (ChartBars == null || ChartBars.Bars == null)
                return;

            double mid = _ema[0];
            double rawAtr = _atr[0];
            ATRUpper[0] = mid + rawAtr;
            ATRLower[0] = mid - rawAtr;
            EMAUpper[0] = _emaHigh[0];
            EMALower[0] = _emaLow[0];

            var upperBrush = UpperFillColor.Clone();
            upperBrush.Opacity = UpperFillOpacity / 255.0;
            var lowerBrush = LowerFillColor.Clone();
            lowerBrush.Opacity = LowerFillOpacity / 255.0;
            var formingBrush = FormingFillColor.Clone();
            formingBrush.Opacity = FormingFillOpacity / 255.0;

            // Static
            if (IsFirstTickOfBar)
            {
                Draw.Region(this, "HistUpper",
                            CurrentBar, 1,
                            ATRUpper, EMAUpper,
                            upperBrush, upperBrush, UpperFillOpacity);

                Draw.Region(this, "HistLower",
                            CurrentBar, 1,
                            EMALower, ATRLower,
                            lowerBrush, lowerBrush, LowerFillOpacity);
            }

            // Dynamic
            Draw.Region(this, "LiveUpper",
                        1, 0,
                        ATRUpper, EMAUpper,
                        formingBrush, formingBrush, FormingFillOpacity);

            Draw.Region(this, "LiveLower",
                        1, 0,
                        EMALower, ATRLower,
                        formingBrush, formingBrush, FormingFillOpacity);
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private TrendIndicator[] cacheTrendIndicator;
        public TrendIndicator TrendIndicator()
        {
            return TrendIndicator(Input);
        }

        public TrendIndicator TrendIndicator(ISeries<double> input)
        {
            if (cacheTrendIndicator != null)
                for (int idx = 0; idx < cacheTrendIndicator.Length; idx++)
                    if (cacheTrendIndicator[idx] != null && cacheTrendIndicator[idx].EqualsInput(input))
                        return cacheTrendIndicator[idx];
            return CacheIndicator<TrendIndicator>(new TrendIndicator(), input, ref cacheTrendIndicator);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.TrendIndicator TrendIndicator()
        {
            return indicator.TrendIndicator(Input);
        }

        public Indicators.TrendIndicator TrendIndicator(ISeries<double> input)
        {
            return indicator.TrendIndicator(input);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.TrendIndicator TrendIndicator()
        {
            return indicator.TrendIndicator(Input);
        }

        public Indicators.TrendIndicator TrendIndicator(ISeries<double> input)
        {
            return indicator.TrendIndicator(input);
        }
    }
}

#endregion
