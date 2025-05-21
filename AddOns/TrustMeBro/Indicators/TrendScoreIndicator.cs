using NinjaTrader.Gui;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class TrendScoreIndicator : Indicator
    {
        #region Properties

        public const string GROUP_NAME_DEFAULT = "1. Trend Score Indicator";

        [NinjaScriptProperty, ReadOnly(true)]
        [Display(Name = "Trend Score Indicator", Description = "Used to visualize the trend scores from Trust Me Bro.", GroupName = GROUP_NAME_DEFAULT, Order = 0)]
        public string TrendScoreIndicatorReadOnly => "Visualize the trend scores from Trust Me Bro";

        [NinjaScriptProperty]
        [Display(Name = "Enable Dynamic Bar Colors", Description = "Enable the dynamic bar colors.", GroupName = GROUP_NAME_DEFAULT, Order = 1)]
        public bool EnableDynamicBarColors { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Positive shape Color", Description = "The positive shape color.", GroupName = GROUP_NAME_DEFAULT, Order = 2)]
        public Brush PositiveShapeColor { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Negative shape Color", Description = "The negative shape color.", GroupName = GROUP_NAME_DEFAULT, Order = 3)]
        public Brush NegativeShapeColor { get; set; }

        #endregion

        #region Serialization

        [Browsable(false)]
        public string PositiveShapeColorSerialize
        {
            get => Serialize.BrushToString(PositiveShapeColor);
            set => PositiveShapeColor = Serialize.StringToBrush(value);
        }

        [Browsable(false)]
        public string NegativeShapeColorSerialize
        {
            get => Serialize.BrushToString(NegativeShapeColor);
            set => NegativeShapeColor = Serialize.StringToBrush(value);
        }

        #endregion

        private TrustMeBro _tmb;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Name = "_TrendScoreIndicator";
                Description = "Used to visualize the trend scores from Trust Me Bro.";
                Calculate = Calculate.OnBarClose;
                IsOverlay = false;
                DisplayInDataBox = true;
                DrawOnPricePanel = false;
                DrawHorizontalGridLines = false;
                DrawVerticalGridLines = false;

                EnableDynamicBarColors = true;
                PositiveShapeColor = Brushes.Green;
                NegativeShapeColor = Brushes.Red;

                AddPlot(
                    new Stroke(Brushes.SteelBlue, DashStyleHelper.Solid, (float)1.5),
                    PlotStyle.Bar,
                    "CurrentTrendScoreColor"
                );

                AddPlot(
                    new Stroke(Brushes.DarkOrange, DashStyleHelper.Solid, (float)1.5),
                    PlotStyle.Line,
                    "AverageTrendScoreColor"
                );

                AddLine(
                    new Stroke(Brushes.CadetBlue, DashStyleHelper.Solid, 1),
                    0,
                    "ZeroLine"
                );
            }
            else if (State == State.DataLoaded)
            {
                _tmb = TrustMeBro();
            }
        }

        protected override void OnBarUpdate()
        {
            _tmb.Update();

            Values[0][0] = _tmb.CurrentTrendScore;
            Values[1][0] = _tmb.AverageTrendScore;

            if (!EnableDynamicBarColors) return;

            if (_tmb.CurrentTrendScore >= 0)
            {
                PlotBrushes[0][0] = PositiveShapeColor;
            }
            else
            {
                PlotBrushes[0][0] = NegativeShapeColor;
            }
        }

        public override void OnCalculateMinMax()
        {
            MinValue = -1;
            MaxValue = 1;
        }
    }
}
