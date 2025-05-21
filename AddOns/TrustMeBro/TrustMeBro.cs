using NinjaTrader.Custom.AddOns.SecretSauce.Models;
using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        public const string GROUP_NAME_GENERAL = "1. General";

        [NinjaScriptProperty, ReadOnly(true)]
        [Display(Name = "Version", Description = "Trust Me Bro Version", Order = 0, GroupName = GROUP_NAME_GENERAL)]
        public string Version => "2.0.0";

        private Queue<double> _trendScores;
        private double _trendScore;
        private double _avgTrendScore;
        private TrendClassifier _trendClassifier;

        Color _negativeBrush;
        Color _positiveBrush;

        public double CurrentTrendScore => _trendScore;
        public double AverageTrendScore => _avgTrendScore;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Think Less. Trade Worse. Trust Me Bro.";
                Name = "_TrustMeBro";
                Calculate = Calculate.OnEachTick;
                IsOverlay = true;
                DrawOnPricePanel = true;
                PaintPriceMarkers = true;
                ScaleJustification = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 8;
                PivotLimit = 150;
                LevelPriceType = LevelPriceTypeOption.Close;
                ThresholdTicks = 8;
                TrendClassifierPeriod = 8;

                PositiveTrendScoreColor = Brushes.Green;
                NegativeTrendScoreColor = Brushes.Red;

                // Pivot Processing
                TrendLineOpacity = 150;
                LevelOpacity = 100;
                TrendLineColor = Brushes.Gold;
                TrendLineDashStyle = DashStyleHelper.Solid;
                TrendLineWidth = 2;
                LevelColor = Brushes.DarkTurquoise;
                LevelDashStyle = DashStyleHelper.Dash;
                LevelWidth = 2;
            }
            else if (State == State.Configure)
            {
                _trendClassifier = new TrendClassifier(TrendClassifierPeriod);
                _trendScores = new Queue<double>();

                SolidColorBrush negativeBrush = (SolidColorBrush)NegativeTrendScoreColor;
                SolidColorBrush positiveBrush = (SolidColorBrush)PositiveTrendScoreColor;
                _negativeBrush = negativeBrush.Color;
                _positiveBrush = positiveBrush.Color;
            }
            else if (State == State.DataLoaded)
            {
                PivotProcessingDataLoaded();
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Period) return;
            ProcessPivotPoint();

            if (IsFirstTickOfBar)
            {
                _trendScore = _trendClassifier.CalculateTrendScore(Closes[0], CurrentBar);
                _trendScores.Enqueue(_trendScore);
                if (_trendScores.Count > Period)
                    _trendScores.Dequeue();
                _avgTrendScore = _trendScores.Average();

                PaintPreviousBar();
                UpdatePivotStatus();
                DrawPivotLevels();
            }
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
            base.OnRender(chartControl, chartScale);

            OnRenderPivotProcessing(chartControl, chartScale);
        }

        private void PaintPreviousBar()
        {
            Brush barColor = GetTrendColor(_trendScore);
            BarBrushes[1] = barColor;
            CandleOutlineBrushes[1] = barColor;
        }

        private static Color InterpolateColor(Color color1, Color color2, double t)
        {
            byte r = (byte)(color1.R * (1 - t) + color2.R * t);
            byte g = (byte)(color1.G * (1 - t) + color2.G * t);
            byte b = (byte)(color1.B * (1 - t) + color2.B * t);
            return Color.FromRgb(r, g, b);
        }

        // Map trend score (-1 to 1) to a gradient color between colors
        private Brush GetTrendColor(double trendScore)
        {
            // Normalize trendScore from [-1, 1] to [0, 1]
            double t = (trendScore + 1) / 2.0; // -1 -> 0, 1 -> 1
            Color color = InterpolateColor(_negativeBrush, _positiveBrush, t);
            return new SolidColorBrush(color);
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private TrustMeBro[] cacheTrustMeBro;
        public TrustMeBro TrustMeBro()
        {
            return TrustMeBro(Input);
        }

        public TrustMeBro TrustMeBro(ISeries<double> input)
        {
            if (cacheTrustMeBro != null)
                for (int idx = 0; idx < cacheTrustMeBro.Length; idx++)
                    if (cacheTrustMeBro[idx] != null && cacheTrustMeBro[idx].EqualsInput(input))
                        return cacheTrustMeBro[idx];
            return CacheIndicator<TrustMeBro>(new TrustMeBro(), input, ref cacheTrustMeBro);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.TrustMeBro TrustMeBro()
        {
            return indicator.TrustMeBro(Input);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input)
        {
            return indicator.TrustMeBro(input);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.TrustMeBro TrustMeBro()
        {
            return indicator.TrustMeBro(Input);
        }

        public Indicators.TrustMeBro TrustMeBro(ISeries<double> input)
        {
            return indicator.TrustMeBro(input);
        }
    }
}

#endregion
