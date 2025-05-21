using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        public const string GROUP_NAME_DEFAULT = "2. Trust Me Bro";

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", Description = "Period used for EMA/ATR", GroupName = GROUP_NAME_DEFAULT, Order = 0)]
        public int Period { get; set; }

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Threshold Ticks", Description = "Threshold to include with levels to consider broken", GroupName = GROUP_NAME_DEFAULT, Order = 1)]
        public int ThresholdTicks { get; set; }

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Pivot Limit", Description = "Limit the number of pivots to increase performance. This will stop extending levels after the pivot limit.", GroupName = GROUP_NAME_DEFAULT, Order = 2)]
        public int PivotLimit { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Level Price Type", Description = "Price to use for pivot levels.", GroupName = GROUP_NAME_DEFAULT, Order = 3)]
        public LevelPriceTypeOption LevelPriceType { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Trend Classifier Period", Description = "Period for trend classifier. Just in case you want a different period.", GroupName = GROUP_NAME_DEFAULT, Order = 4)]
        public int TrendClassifierPeriod { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Positive Trend Score Color", Description = "The positive trend score color.", GroupName = GROUP_NAME_DEFAULT, Order = 5)]
        public Brush PositiveTrendScoreColor { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Negative Trend Score Color", Description = "The negative trend score color.", GroupName = GROUP_NAME_DEFAULT, Order = 6)]
        public Brush NegativeTrendScoreColor { get; set; }

        #region Serialization

        [Browsable(false)]
        public string PositiveTrendScoreColorSerialize
        {
            get => Serialize.BrushToString(PositiveTrendScoreColor);
            set => PositiveTrendScoreColor = Serialize.StringToBrush(value);
        }

        [Browsable(false)]
        public string NegativeTrendScoreColorSerialize
        {
            get => Serialize.BrushToString(NegativeTrendScoreColor);
            set => NegativeTrendScoreColor = Serialize.StringToBrush(value);
        }

        #endregion
    }
}
