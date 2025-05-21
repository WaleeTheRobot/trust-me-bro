using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX;
using SharpDX.Direct2D1;
using System.Collections.Generic;
using static NinjaTrader.Custom.AddOns.TrustMeBro.Models.Pivots;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        private double _threshold;
        private PivotBar _previousBar = null;
        private List<PivotPoint> _pivots = null;
        private bool _hasFirstPivot = false;
        private bool _isLookingForHigh = false;
        private double _currentMaxHigh, _currentMinLow;
        private int _maxHighBar, _minLowBar;

        private void PivotProcessingDataLoaded()
        {
            _previousBar = new PivotBar(CurrentBar, 0, 0, 0, 0);
            _pivots = new List<PivotPoint>(PivotLimit);
            _currentMaxHigh = 0;
            _maxHighBar = 0;
            _currentMinLow = 0;
            _minLowBar = 0;
            _threshold = TickSize * ThresholdTicks;
        }

        private void OnRenderPivotProcessing(ChartControl chartControl, ChartScale chartScale)
        {
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

        private void ProcessPivotPoint()
        {
            _previousBar = new PivotBar(CurrentBar - 1, Open[1], High[1], Low[1], Close[1]);

            if (IsFirstPivot())
                return;

            // Skip inside bar
            if (High[0] < High[1] && Low[0] > Low[1])
                return;

            if (_isLookingForHigh)
                IsLookingForHigh();
            else
                IsLookingForLow();
        }

        private bool IsFirstPivot()
        {
            if (!_hasFirstPivot)
            {
                if (_previousBar.Open <= _previousBar.Close)
                {
                    _pivots.Add(new PivotPoint(_previousBar, false));
                    _isLookingForHigh = true;
                    _currentMaxHigh = High[0];
                    _maxHighBar = CurrentBar;
                }
                else
                {
                    _pivots.Add(new PivotPoint(_previousBar, true));
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

                // Draw the zig-zag line for the new pivot
                if (_pivots.Count >= 2)
                {
                    PivotPoint pivotA = _pivots[_pivots.Count - 2];
                    PivotPoint pivotB = _pivots[_pivots.Count - 1];
                    int barsAgoA = CurrentBar - pivotA.PivotBar.BarNumber;
                    int barsAgoB = CurrentBar - pivotB.PivotBar.BarNumber;
                    double priceA = GetPivotPrice(pivotA);
                    double priceB = GetPivotPrice(pivotB);
                    SolidColorBrush solidBrush = TrendLineColor as SolidColorBrush;
                    if (solidBrush != null)
                    {
                        Color baseColor = solidBrush.Color;
                        Color lineColorWithOpacity = Color.FromArgb(TrendLineOpacity, baseColor.R, baseColor.G, baseColor.B);
                        SolidColorBrush brushWithOpacity = new SolidColorBrush(lineColorWithOpacity);
                        string tag = $"zigzag_{pivotA.PivotBar.BarNumber}_{pivotB.PivotBar.BarNumber}";
                        var trendLine = Draw.Line(this, tag, false, barsAgoA, priceA, barsAgoB, priceB, brushWithOpacity, TrendLineDashStyle, TrendLineWidth);
                        trendLine.ZOrderType = DrawingToolZOrder.AlwaysDrawnLast;
                    }
                }

                if (_pivots.Count > PivotLimit)
                {
                    PivotPoint oldPivot = _pivots[0];
                    if (oldPivot.IsDrawn)
                    {
                        RemoveDrawObject($"pivot_{oldPivot.PivotBar.BarNumber}");
                    }
                    _pivots.RemoveAt(0);
                }

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

                // Draw zig-zag line for the new pivot
                if (_pivots.Count >= 2)
                {
                    PivotPoint pivotA = _pivots[_pivots.Count - 2];
                    PivotPoint pivotB = _pivots[_pivots.Count - 1];
                    int barsAgoA = CurrentBar - pivotA.PivotBar.BarNumber;
                    int barsAgoB = CurrentBar - pivotB.PivotBar.BarNumber;
                    double priceA = GetPivotPrice(pivotA);
                    double priceB = GetPivotPrice(pivotB);
                    SolidColorBrush solidBrush = TrendLineColor as SolidColorBrush;
                    if (solidBrush != null)
                    {
                        Color baseColor = solidBrush.Color;
                        Color lineColorWithOpacity = Color.FromArgb(TrendLineOpacity, baseColor.R, baseColor.G, baseColor.B);
                        SolidColorBrush brushWithOpacity = new SolidColorBrush(lineColorWithOpacity);
                        string tag = $"zigzag_{pivotA.PivotBar.BarNumber}_{pivotB.PivotBar.BarNumber}";
                        var trendLine = Draw.Line(this, tag, false, barsAgoA, priceA, barsAgoB, priceB, brushWithOpacity, TrendLineDashStyle, TrendLineWidth);
                        trendLine.ZOrderType = DrawingToolZOrder.AlwaysDrawnLast;
                    }
                }

                if (_pivots.Count > PivotLimit)
                {
                    PivotPoint oldPivot = _pivots[0];
                    if (oldPivot.IsDrawn)
                    {
                        RemoveDrawObject($"pivot_{oldPivot.PivotBar.BarNumber}");
                    }
                    _pivots.RemoveAt(0);
                }

                _isLookingForHigh = true;
                _currentMaxHigh = High[0];
                _maxHighBar = CurrentBar;
            }
        }

        private static double GetPivotPrice(PivotPoint pivot)
        {
            return pivot.IsHigh ? pivot.PivotBar.High : pivot.PivotBar.Low;
        }

        private double GetPivotLevelPrice(PivotPoint pivot)
        {
            switch (LevelPriceType)
            {
                case LevelPriceTypeOption.Close:
                    return pivot.PivotBar.Close;
                case LevelPriceTypeOption.HighLow:
                    return pivot.IsHigh ? pivot.PivotBar.High : pivot.PivotBar.Low;
                default:
                    return pivot.PivotBar.Close;
            }
        }

        private void UpdatePivotStatus()
        {
            foreach (var pivot in _pivots)
            {
                if (!pivot.DisplayLevel)
                    continue;

                double pivotClose = pivot.PivotBar.Close;

                if (pivot.IsHigh)
                {
                    if (Open[0] > pivotClose + _threshold)
                    {
                        pivot.DisplayLevel = false;
                    }
                }
                else
                {
                    if (Open[0] < pivotClose - _threshold)
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
                    var levelLine = Draw.Line(this, tag, false, CurrentBar - startBar, pivotPrice, 0, pivotPrice, brushWithOpacity, LevelDashStyle, LevelWidth);
                    levelLine.ZOrderType = DrawingToolZOrder.AlwaysDrawnLast;
                }
            }
        }
    }
}
