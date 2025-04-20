using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Custom.AddOns.TrustMeBro.Events;
using NinjaTrader.Custom.AddOns.TrustMeBro.Pivots;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using Brush = System.Windows.Media.Brush;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        private PivotManager _pivotManager;
        private BarData _previousBar = null;
        private BarData _currentBar = null;
        private BarData _barLookup = null;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Think Less. Trade Worse. Use Trust Me, Bro.";
                Name = "_TrustMeBro";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 6;
                ThresholdTicks = 8;
                PivotLimit = 150;

                TrendIndicatorDefault();
            }
            else if (State == State.DataLoaded)
            {
                _pivotManager = new PivotManager(PivotLimit, TickSize * ThresholdTicks);
                _previousBar = new BarData(0, 0, 0, 0, 0);
                _currentBar = new BarData(0, 0, 0, 0, 0);
                _barLookup = new BarData(0, 0, 0, 0, 0);

                EventManager.OnPrintMessage += HandlePrintMessage;
            }
            else if (State == State.Configure)
            {
            }
        }


        protected override void OnBarUpdate()
        {
            if (CurrentBar <= Period)
                return;

            UpdateBars();
            _pivotManager.ProcessPivotPoint(_previousBar, _currentBar, barIdx =>
            {
                _barLookup.BarNumber = barIdx;
                _barLookup.Open = Open.GetValueAt(barIdx);
                _barLookup.High = High.GetValueAt(barIdx);
                _barLookup.Low = Low.GetValueAt(barIdx);
                _barLookup.Close = Close.GetValueAt(barIdx);
                return _barLookup;
            });

            if (IsFirstTickOfBar)
            {
                _pivotManager.UpdatePivotStatus();
            }
        }

        private void UpdateBars()
        {
            _previousBar.BarNumber = CurrentBar - 1;
            _previousBar.Open = Open[1];
            _previousBar.High = High[1];
            _previousBar.Low = Low[1];
            _previousBar.Close = Close[1];

            _currentBar.BarNumber = CurrentBar;
            _currentBar.Open = Open[0];
            _currentBar.High = High[0];
            _currentBar.Low = Low[0];
            _currentBar.Close = Close[0];
        }


        private void HandlePrintMessage(string eventMessage, bool addNewLine)
        {
            Print(eventMessage);

            if (addNewLine)
            {
                Print("");
            }
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : Gui.NinjaScript.IndicatorRenderBase
    {
        private TrustMeBro[] cacheTrustMeBro;
        public TrustMeBro TrustMeBro(int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return TrustMeBro(Input, period, thresholdTicks, pivotLimit, levelPriceType, upperFillColor, lowerFillColor, formingFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, formingFillOpacity, trendLineOpacity, levelOpacity);
        }

        public TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            if (cacheTrustMeBro != null)
                for (int idx = 0; idx < cacheTrustMeBro.Length; idx++)
                    if (cacheTrustMeBro[idx] != null && cacheTrustMeBro[idx].Period == period && cacheTrustMeBro[idx].ThresholdTicks == thresholdTicks && cacheTrustMeBro[idx].PivotLimit == pivotLimit && cacheTrustMeBro[idx].LevelPriceType == levelPriceType && cacheTrustMeBro[idx].UpperFillColor == upperFillColor && cacheTrustMeBro[idx].LowerFillColor == lowerFillColor && cacheTrustMeBro[idx].FormingFillColor == formingFillColor && cacheTrustMeBro[idx].TrendLineColor == trendLineColor && cacheTrustMeBro[idx].TrendLineDashStyle == trendLineDashStyle && cacheTrustMeBro[idx].TrendLineWidth == trendLineWidth && cacheTrustMeBro[idx].LevelColor == levelColor && cacheTrustMeBro[idx].LevelDashStyle == levelDashStyle && cacheTrustMeBro[idx].LevelWidth == levelWidth && cacheTrustMeBro[idx].ATRUpperOpacity == aTRUpperOpacity && cacheTrustMeBro[idx].ATRLowerOpacity == aTRLowerOpacity && cacheTrustMeBro[idx].EMAUpperOpacity == eMAUpperOpacity && cacheTrustMeBro[idx].EMALowerOpacity == eMALowerOpacity && cacheTrustMeBro[idx].UpperFillOpacity == upperFillOpacity && cacheTrustMeBro[idx].LowerFillOpacity == lowerFillOpacity && cacheTrustMeBro[idx].FormingFillOpacity == formingFillOpacity && cacheTrustMeBro[idx].TrendLineOpacity == trendLineOpacity && cacheTrustMeBro[idx].LevelOpacity == levelOpacity && cacheTrustMeBro[idx].EqualsInput(input))
                        return cacheTrustMeBro[idx];
            return CacheIndicator(new TrustMeBro() { Period = period, ThresholdTicks = thresholdTicks, PivotLimit = pivotLimit, LevelPriceType = levelPriceType, UpperFillColor = upperFillColor, LowerFillColor = lowerFillColor, FormingFillColor = formingFillColor, TrendLineColor = trendLineColor, TrendLineDashStyle = trendLineDashStyle, TrendLineWidth = trendLineWidth, LevelColor = levelColor, LevelDashStyle = levelDashStyle, LevelWidth = levelWidth, ATRUpperOpacity = aTRUpperOpacity, ATRLowerOpacity = aTRLowerOpacity, EMAUpperOpacity = eMAUpperOpacity, EMALowerOpacity = eMALowerOpacity, UpperFillOpacity = upperFillOpacity, LowerFillOpacity = lowerFillOpacity, FormingFillOpacity = formingFillOpacity, TrendLineOpacity = trendLineOpacity, LevelOpacity = levelOpacity }, input, ref cacheTrustMeBro);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.TrustMeBro TrustMeBro(int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(Input, period, thresholdTicks, pivotLimit, levelPriceType, upperFillColor, lowerFillColor, formingFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, formingFillOpacity, trendLineOpacity, levelOpacity);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(input, period, thresholdTicks, pivotLimit, levelPriceType, upperFillColor, lowerFillColor, formingFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, formingFillOpacity, trendLineOpacity, levelOpacity);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.TrustMeBro TrustMeBro(int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(Input, period, thresholdTicks, pivotLimit, levelPriceType, upperFillColor, lowerFillColor, formingFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, formingFillOpacity, trendLineOpacity, levelOpacity);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, int pivotLimit, LevelPriceType levelPriceType, Brush upperFillColor, Brush lowerFillColor, Brush formingFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte formingFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(input, period, thresholdTicks, pivotLimit, levelPriceType, upperFillColor, lowerFillColor, formingFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, formingFillOpacity, trendLineOpacity, levelOpacity);
        }
    }
}

#endregion
