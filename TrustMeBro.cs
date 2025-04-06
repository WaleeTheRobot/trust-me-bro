using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX;
using SharpDX.Direct2D1;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace NinjaTrader.NinjaScript.Indicators
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

        public PivotPoint(PivotBar pivotBar, bool isHigh, int brokenBarNumber = 0, bool isBroken = false, bool displayLevel = true)
        {
            PivotBar = pivotBar;
            IsHigh = isHigh;
            BrokenBarNumber = brokenBarNumber;
            IsBroken = isBroken;
            DisplayLevel = displayLevel;
        }
    }

    public class TrustMeBro : Indicator
    {
        public const string GROUP_NAME_GENERAL = "1. General";
        public const string GROUP_NAME_TRUST_ME_BRO = "2. Trust Me Bro";
        public const string GROUP_NAME_PLOTS = "Plots";

        private EMA _emaHigh, _emaLow, _ema;
        private ATR _atr;

        private PivotBar _previousBar = null;
        private List<PivotPoint> _pivots = null;
        private PivotPoint _currentPivot = null;
        private bool _hasFirstPivot = false;
        private bool _isLookingForHigh = false;
        private double _currentMaxHigh, _currentMinLow;
        private int _maxHighBar, _minLowBar;
        private double _tickSize;

        #region General Properties

        [NinjaScriptProperty]
        [Display(Name = "Version", Description = "Trust Me Bro Version", Order = 0, GroupName = GROUP_NAME_GENERAL)]
        [ReadOnly(true)]
        public string Version => "1.0.0";

        #endregion

        #region Trust Me Bro Properties

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", Description = "Period used for EMA/ATR", GroupName = GROUP_NAME_TRUST_ME_BRO, Order = 0)]
        public int Period
        { get; set; }

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Threshold Ticks", Description = "Threshold to include with levels to consider broken", GroupName = GROUP_NAME_TRUST_ME_BRO, Order = 1)]
        public int ThresholdTicks
        { get; set; }

        #endregion

        #region Plots

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "ATR Upper", GroupName = GROUP_NAME_PLOTS, Order = 0)]
        public Series<double> ATRUpper
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "ATR Lower", GroupName = GROUP_NAME_PLOTS, Order = 1)]
        public Series<double> ATRLower
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "EMA Upper", GroupName = GROUP_NAME_PLOTS, Order = 2)]
        public Series<double> EMAUpper
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "EMA Lower", GroupName = GROUP_NAME_PLOTS, Order = 3)]
        public Series<double> EMALower
        {
            get { return Values[3]; }
        }

        [NinjaScriptProperty]
        [Display(Name = "Upper Fill Color", Description = "Color for the upper fill and border", GroupName = GROUP_NAME_PLOTS, Order = 4)]
        public Brush UpperFillColor { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Lower Fill Color", Description = "Color for the lower fill and border", GroupName = GROUP_NAME_PLOTS, Order = 5)]
        public Brush LowerFillColor { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Trend Line Color", Description = "The trend line color.", GroupName = GROUP_NAME_PLOTS, Order = 6)]
        public Brush TrendLineColor { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Trend Line Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 7)]
        public DashStyleHelper TrendLineDashStyle { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Trend Line Width", GroupName = GROUP_NAME_PLOTS, Order = 8)]
        public int TrendLineWidth { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Level Color", Description = "The level color.", GroupName = GROUP_NAME_PLOTS, Order = 9)]
        public Brush LevelColor { get; set; }
        [NinjaScriptProperty]
        [Display(Name = "Level Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 10)]
        public DashStyleHelper LevelDashStyle { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Level Width", GroupName = GROUP_NAME_PLOTS, Order = 11)]
        public int LevelWidth { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ATR Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 12)]
        public byte ATRUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ATR Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 13)]
        public byte ATRLowerOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "EMA Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 14)]
        public byte EMAUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "EMA Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 15)]
        public byte EMALowerOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Upper Fill Opacity", Description = "The opacity for the upper fill. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 16)]
        public byte UpperFillOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Lower Fill Opacity", Description = "The opacity for the lower fill. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 17)]
        public byte LowerFillOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Trend Line Opacity", Description = "The opacity for the trend line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 18)]
        public byte TrendLineOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Level Opacity", Description = "The opacity for the levels. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 19)]
        public byte LevelOpacity { get; set; }

        #endregion

        #region Serialization

        [Browsable(false)]
        public string UpperFillColorSerialize
        {
            get => Serialize.BrushToString(UpperFillColor);
            set => UpperFillColor = Serialize.StringToBrush(value);
        }

        [Browsable(false)]
        public string LowerFillColorSerialize
        {
            get => Serialize.BrushToString(LowerFillColor);
            set => LowerFillColor = Serialize.StringToBrush(value);
        }

        [Browsable(false)]
        public string TrendLineColorSerialize
        {
            get => Serialize.BrushToString(TrendLineColor);
            set => TrendLineColor = Serialize.StringToBrush(value);
        }

        [Browsable(false)]
        public string LevelColorSerialize
        {
            get => Serialize.BrushToString(LevelColor);
            set => LevelColor = Serialize.StringToBrush(value);
        }

        #endregion

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
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 3;
                ThresholdTicks = 8;
                ATRUpperOpacity = 0;
                ATRLowerOpacity = 0;
                EMAUpperOpacity = 0;
                EMALowerOpacity = 0;
                UpperFillOpacity = 20;
                LowerFillOpacity = 20;
                TrendLineOpacity = 150;
                LevelOpacity = 100;

                AddPlot(Brushes.SlateGray, "ATR Upper");
                AddPlot(Brushes.SlateGray, "ATR Lower");
                AddPlot(Brushes.DarkRed, "EMA Upper");
                AddPlot(Brushes.DarkGreen, "EMA Lower");
                UpperFillColor = Brushes.DarkRed;
                LowerFillColor = Brushes.DarkGreen;
                TrendLineColor = Brushes.Gold;
                TrendLineDashStyle = DashStyleHelper.Solid;
                TrendLineWidth = 2;
                LevelColor = Brushes.DarkTurquoise;
                LevelDashStyle = DashStyleHelper.Dash;
                LevelWidth = 2;
            }
            else if (State == State.DataLoaded)
            {
                _emaHigh = EMA(High, Period);
                _emaLow = EMA(Low, Period);
                _ema = EMA(Period);
                _atr = ATR(Period);
                _previousBar = new PivotBar(CurrentBar, 0, 0, 0, 0);
                _pivots = new List<PivotPoint>();
                _currentPivot = new PivotPoint(null, false);
                _currentMaxHigh = 0;
                _maxHighBar = 0;
                _currentMinLow = 0;
                _minLowBar = 0;
                _tickSize = TickSize;
            }
            else if (State == State.Configure)
            {
                Plots[0].Opacity = ATRUpperOpacity;
                Plots[1].Opacity = ATRLowerOpacity;
                Plots[2].Opacity = EMAUpperOpacity;
                Plots[3].Opacity = EMALowerOpacity;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Period)
            {
                ATRUpper.Reset();
                ATRLower.Reset();
                EMAUpper.Reset();
                EMALower.Reset();

                return;
            }

            ProcessPivotPoint();
            UpdatePivotStatus();
            DrawHistoricalZigZag();
            DrawPivotLevels();
            DrawFill();
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            base.OnRender(chartControl, chartScale);

            if (_pivots == null || _pivots.Count == 0 || CurrentBar < Period || ChartBars == null || ChartBars.Bars.Count == 0 || RenderTarget == null)
                return;

            PivotPoint lastPivot = _pivots[_pivots.Count - 1];
            int lastPivotBarNumber = lastPivot.PivotBar.BarNumber;
            int lastPivotBarsAgo = CurrentBar - lastPivotBarNumber;

            if (lastPivotBarsAgo < 0 || lastPivotBarsAgo > CurrentBar)
                return;

            double lastPivotPrice = GetPivotPrice(lastPivot);
            double targetPrice = _isLookingForHigh ? _currentMaxHigh : _currentMinLow;
            int targetBarsAgo = _isLookingForHigh ? CurrentBar - _maxHighBar : CurrentBar - _minLowBar;

            if (targetBarsAgo < 0 || targetBarsAgo > CurrentBar)
                return;

            if (lastPivotBarNumber < 0 || lastPivotBarNumber >= ChartBars.Bars.Count ||
                (_isLookingForHigh && (_maxHighBar < 0 || _maxHighBar >= ChartBars.Bars.Count)) ||
                (!_isLookingForHigh && (_minLowBar < 0 || _minLowBar >= ChartBars.Bars.Count)))
                return;

            float x1 = chartControl.GetXByBarIndex(ChartBars, lastPivotBarNumber);
            float x2 = chartControl.GetXByBarIndex(ChartBars, _isLookingForHigh ? _maxHighBar : _minLowBar);
            float y1 = chartScale.GetYByValue(lastPivotPrice);
            float y2 = chartScale.GetYByValue(targetPrice);

            SolidColorBrush solidBrush = TrendLineColor as SolidColorBrush;
            if (solidBrush == null)
                return;
            Color baseColor = solidBrush.Color;
            SharpDX.Color4 dxColor = new SharpDX.Color4(
                baseColor.R / 255f,
                baseColor.G / 255f,
                baseColor.B / 255f,
                TrendLineOpacity / 255f
            );
            using (SharpDX.Direct2D1.SolidColorBrush dashBrush = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, dxColor))
            using (SharpDX.Direct2D1.StrokeStyle dashStyle = new(RenderTarget.Factory, new StrokeStyleProperties { DashStyle = SharpDX.Direct2D1.DashStyle.Dash }))
            {
                RenderTarget.DrawLine(
                    new Vector2(x1, y1),
                    new Vector2(x2, y2),
                    dashBrush,
                    2f,
                    dashStyle
                );
            }
        }

        #region Pivot Processing

        private void ProcessPivotPoint()
        {
            _previousBar = new PivotBar(CurrentBar - 1, Open[1], High[1], Low[1], Close[1]);

            if (IsFirstPivot())
            {
                return;
            }

            if (_isLookingForHigh)
            {
                IsLookingForHigh();
            }
            else
            {
                IsLookingForLow();
            }
        }

        private bool IsFirstPivot()
        {
            if (!_hasFirstPivot)
            {
                if (_previousBar.Open <= _previousBar.Close)
                {
                    _currentPivot = new PivotPoint(_previousBar, false);
                    _pivots.Add(_currentPivot);
                    _isLookingForHigh = true;
                    _currentMaxHigh = High[0];
                    _maxHighBar = CurrentBar;
                }
                else
                {
                    _currentPivot = new PivotPoint(_previousBar, true);
                    _pivots.Add(_currentPivot);
                    _isLookingForHigh = false;
                    _currentMinLow = Low[0];
                    _minLowBar = CurrentBar;
                }
                _hasFirstPivot = true;
                return true;
            }

            return false;
        }

        private void IsLookingForHigh()
        {
            if (High[0] > _currentMaxHigh)
            {
                _currentMaxHigh = High[0];
                _maxHighBar = CurrentBar;
            }
            else if (High[0] < _currentMaxHigh)
            {
                PivotBar swingHighBar = new PivotBar(_maxHighBar, Open.GetValueAt(_maxHighBar),
                    High.GetValueAt(_maxHighBar), Low.GetValueAt(_maxHighBar), Close.GetValueAt(_maxHighBar));
                PivotPoint swingHigh = new PivotPoint(swingHighBar, true);
                _pivots.Add(swingHigh);
                _isLookingForHigh = false;
                _currentMinLow = Low[0];
                _minLowBar = CurrentBar;
            }
        }

        private void IsLookingForLow()
        {
            if (Low[0] < _currentMinLow)
            {
                _currentMinLow = Low[0];
                _minLowBar = CurrentBar;
            }
            else if (Low[0] > _currentMinLow)
            {
                PivotBar swingLowBar = new PivotBar(_minLowBar, Open.GetValueAt(_minLowBar),
                    High.GetValueAt(_minLowBar), Low.GetValueAt(_minLowBar), Close.GetValueAt(_minLowBar));
                PivotPoint swingLow = new PivotPoint(swingLowBar, false);
                _pivots.Add(swingLow);
                _isLookingForHigh = true;
                _currentMaxHigh = High[0];
                _maxHighBar = CurrentBar;
            }
        }

        private static double GetPivotPrice(PivotPoint pivot)
        {
            return pivot.IsHigh ? pivot.PivotBar.High : pivot.PivotBar.Low;
        }

        private static double GetPivotLevelPrice(PivotPoint pivot)
        {
            return pivot.PivotBar.Close;
        }

        private void DrawHistoricalZigZag()
        {
            for (int i = 0; i < _pivots.Count - 1; i++)
            {
                PivotPoint pivotA = _pivots[i];
                PivotPoint pivotB = _pivots[i + 1];
                int barsAgoA = CurrentBar - pivotA.PivotBar.BarNumber;
                int barsAgoB = CurrentBar - pivotB.PivotBar.BarNumber;
                double priceA = GetPivotPrice(pivotA);
                double priceB = GetPivotPrice(pivotB);

                SolidColorBrush solidBrush = TrendLineColor as SolidColorBrush;
                if (solidBrush == null)
                    continue;
                Color baseColor = solidBrush.Color;
                Color lineColorWithOpacity = Color.FromArgb(
                    TrendLineOpacity,
                    baseColor.R,
                    baseColor.G,
                    baseColor.B
                );
                SolidColorBrush brushWithOpacity = new SolidColorBrush(lineColorWithOpacity);
                string tag = $"zigzag_{pivotA.PivotBar.BarNumber}_{pivotB.PivotBar.BarNumber}";
                Draw.Line(this, tag, false, barsAgoA, priceA, barsAgoB, priceB, brushWithOpacity, TrendLineDashStyle, TrendLineWidth);
            }
        }

        private void UpdatePivotStatus()
        {
            if (!IsFirstTickOfBar)
                return;

            double threshold = _tickSize * ThresholdTicks;

            foreach (var pivot in _pivots)
            {
                if (!pivot.DisplayLevel)
                    continue;

                double pivotClose = pivot.PivotBar.Close;

                if (pivot.IsHigh)
                {
                    if (Open[0] > pivotClose + threshold)
                    {
                        pivot.DisplayLevel = false;
                    }
                }
                else
                {
                    if (Open[0] < pivotClose - threshold)
                    {
                        pivot.DisplayLevel = false;
                    }
                }
            }
        }

        private void DrawPivotLevels()
        {
            foreach (var pivot in _pivots)
            {
                if (!pivot.DisplayLevel)
                {
                    string removeTag = $"pivot_{pivot.PivotBar.BarNumber}";
                    RemoveDrawObject(removeTag);
                    continue;
                }

                double pivotPrice = GetPivotLevelPrice(pivot);
                int startBar = pivot.PivotBar.BarNumber;
                SolidColorBrush solidBrush = LevelColor as SolidColorBrush;
                if (solidBrush != null)
                {
                    Color baseColor = solidBrush.Color;
                    Color levelColorWithOpacity = Color.FromArgb(
                        LevelOpacity,
                        baseColor.R,
                        baseColor.G,
                        baseColor.B
                    );
                    SolidColorBrush brushWithOpacity = new SolidColorBrush(levelColorWithOpacity);
                    string tag = $"pivot_{startBar}";
                    Draw.Line(this, tag, false, CurrentBar - startBar, pivotPrice, 0, pivotPrice, brushWithOpacity, LevelDashStyle, LevelWidth);
                }
            }
        }

        #endregion

        #region Fill

        private void DrawFill()
        {
            double mid = _ema[0];
            double rawAtr = _atr[0];

            ATRUpper[0] = mid + rawAtr;
            ATRLower[0] = mid - rawAtr;
            EMAUpper[0] = _emaHigh[0];
            EMALower[0] = _emaLow[0];

            Brush upperFillBrush = UpperFillColor.Clone();
            upperFillBrush.Opacity = UpperFillOpacity / 255.0;
            Brush lowerFillBrush = LowerFillColor.Clone();
            lowerFillBrush.Opacity = LowerFillOpacity / 255.0;

            Draw.Region(this, "UpperFill", CurrentBar, 0, ATRUpper, EMAUpper, upperFillBrush, upperFillBrush, UpperFillOpacity);
            Draw.Region(this, "LowerFill", CurrentBar, 0, EMALower, ATRLower, lowerFillBrush, lowerFillBrush, LowerFillOpacity);
        }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private TrustMeBro[] cacheTrustMeBro;
        public TrustMeBro TrustMeBro(int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return TrustMeBro(Input, period, thresholdTicks, upperFillColor, lowerFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, trendLineOpacity, levelOpacity);
        }

        public TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            if (cacheTrustMeBro != null)
                for (int idx = 0; idx < cacheTrustMeBro.Length; idx++)
                    if (cacheTrustMeBro[idx] != null && cacheTrustMeBro[idx].Period == period && cacheTrustMeBro[idx].ThresholdTicks == thresholdTicks && cacheTrustMeBro[idx].UpperFillColor == upperFillColor && cacheTrustMeBro[idx].LowerFillColor == lowerFillColor && cacheTrustMeBro[idx].TrendLineColor == trendLineColor && cacheTrustMeBro[idx].TrendLineDashStyle == trendLineDashStyle && cacheTrustMeBro[idx].TrendLineWidth == trendLineWidth && cacheTrustMeBro[idx].LevelColor == levelColor && cacheTrustMeBro[idx].LevelDashStyle == levelDashStyle && cacheTrustMeBro[idx].LevelWidth == levelWidth && cacheTrustMeBro[idx].ATRUpperOpacity == aTRUpperOpacity && cacheTrustMeBro[idx].ATRLowerOpacity == aTRLowerOpacity && cacheTrustMeBro[idx].EMAUpperOpacity == eMAUpperOpacity && cacheTrustMeBro[idx].EMALowerOpacity == eMALowerOpacity && cacheTrustMeBro[idx].UpperFillOpacity == upperFillOpacity && cacheTrustMeBro[idx].LowerFillOpacity == lowerFillOpacity && cacheTrustMeBro[idx].TrendLineOpacity == trendLineOpacity && cacheTrustMeBro[idx].LevelOpacity == levelOpacity && cacheTrustMeBro[idx].EqualsInput(input))
                        return cacheTrustMeBro[idx];
            return CacheIndicator<TrustMeBro>(new TrustMeBro() { Period = period, ThresholdTicks = thresholdTicks, UpperFillColor = upperFillColor, LowerFillColor = lowerFillColor, TrendLineColor = trendLineColor, TrendLineDashStyle = trendLineDashStyle, TrendLineWidth = trendLineWidth, LevelColor = levelColor, LevelDashStyle = levelDashStyle, LevelWidth = levelWidth, ATRUpperOpacity = aTRUpperOpacity, ATRLowerOpacity = aTRLowerOpacity, EMAUpperOpacity = eMAUpperOpacity, EMALowerOpacity = eMALowerOpacity, UpperFillOpacity = upperFillOpacity, LowerFillOpacity = lowerFillOpacity, TrendLineOpacity = trendLineOpacity, LevelOpacity = levelOpacity }, input, ref cacheTrustMeBro);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.TrustMeBro TrustMeBro(int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(Input, period, thresholdTicks, upperFillColor, lowerFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, trendLineOpacity, levelOpacity);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(input, period, thresholdTicks, upperFillColor, lowerFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, trendLineOpacity, levelOpacity);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.TrustMeBro TrustMeBro(int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(Input, period, thresholdTicks, upperFillColor, lowerFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, trendLineOpacity, levelOpacity);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input, int period, int thresholdTicks, Brush upperFillColor, Brush lowerFillColor, Brush trendLineColor, DashStyleHelper trendLineDashStyle, int trendLineWidth, Brush levelColor, DashStyleHelper levelDashStyle, int levelWidth, byte aTRUpperOpacity, byte aTRLowerOpacity, byte eMAUpperOpacity, byte eMALowerOpacity, byte upperFillOpacity, byte lowerFillOpacity, byte trendLineOpacity, byte levelOpacity)
        {
            return indicator.TrustMeBro(input, period, thresholdTicks, upperFillColor, lowerFillColor, trendLineColor, trendLineDashStyle, trendLineWidth, levelColor, levelDashStyle, levelWidth, aTRUpperOpacity, aTRLowerOpacity, eMAUpperOpacity, eMALowerOpacity, upperFillOpacity, lowerFillOpacity, trendLineOpacity, levelOpacity);
        }
    }
}

#endregion
