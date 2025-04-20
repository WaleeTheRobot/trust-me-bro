using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using NinjaTrader.Gui;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using Brush = System.Windows.Media.Brush;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        public const string GROUP_NAME_GENERAL = "1. General";
        public const string GROUP_NAME_TRUST_ME_BRO = "2. Trust Me Bro";
        public const string GROUP_NAME_PLOTS = "Plots";

        private static readonly string[] _brushProps = {
            nameof(UpperFillColor),
            nameof(LowerFillColor),
            nameof(FormingFillColor),
            nameof(TrendLineColor),
            nameof(LevelColor)
        };

        #region Properties

        #region General Properties

        [NinjaScriptProperty]
        [Display(Name = "Version", Description = "Trust Me Bro Version", Order = 0, GroupName = GROUP_NAME_GENERAL)]
        [ReadOnly(true)]
        public string Version => "1.1.0";

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

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Pivot Limit", Description = "Limit the number of pivots to increase performance. This will stop extending levels after the pivot limit.", GroupName = GROUP_NAME_TRUST_ME_BRO, Order = 2)]
        public int PivotLimit
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Level Price Type", Description = "Price to use for pivot levels", GroupName = GROUP_NAME_TRUST_ME_BRO, Order = 3)]
        public LevelPriceType LevelPriceType { get; set; }

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
        [XmlIgnore]
        [Display(Name = "Upper Fill Color", Description = "Color for the upper fill and border", GroupName = GROUP_NAME_PLOTS, Order = 4)]
        public Brush UpperFillColor { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Lower Fill Color", Description = "Color for the lower fill and border", GroupName = GROUP_NAME_PLOTS, Order = 5)]
        public Brush LowerFillColor { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Forming Fill Color", Description = "Color for the upper and lower forming fill", GroupName = GROUP_NAME_PLOTS, Order = 6)]
        public Brush FormingFillColor { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Trend Line Color", Description = "The trend line color.", GroupName = GROUP_NAME_PLOTS, Order = 7)]
        public Brush TrendLineColor { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Trend Line Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 8)]
        public DashStyleHelper TrendLineDashStyle { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Trend Line Width", GroupName = GROUP_NAME_PLOTS, Order = 9)]
        public int TrendLineWidth { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Level Color", Description = "The level color.", GroupName = GROUP_NAME_PLOTS, Order = 10)]
        public Brush LevelColor { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Level Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 11)]
        public DashStyleHelper LevelDashStyle { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Level Width", GroupName = GROUP_NAME_PLOTS, Order = 12)]
        public int LevelWidth { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "ATR Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 13)]
        public byte ATRUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "ATR Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 14)]
        public byte ATRLowerOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "EMA Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 15)]
        public byte EMAUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "EMA Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 16)]
        public byte EMALowerOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Upper Fill Opacity", Description = "The opacity for the upper fill. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 17)]
        public byte UpperFillOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Lower Fill Opacity", Description = "The opacity for the lower fill. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 18)]
        public byte LowerFillOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Forming Fill Opacity", Description = "The opacity for the upper and lower forming fill. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 19)]
        public byte FormingFillOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Trend Line Opacity", Description = "The opacity for the trend line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 20)]
        public byte TrendLineOpacity { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Level Opacity", Description = "The opacity for the levels. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 21)]
        public byte LevelOpacity { get; set; }

        #endregion

        #region Serialization

        [Browsable(false)]
        [XmlIgnore]
        public string ColorsSerialize
        {
            get
            {
                var parts = _brushProps
                    .Select(p => (Brush)GetType().GetProperty(p).GetValue(this))
                    .Select(b => Serialize.BrushToString(b));
                return string.Join(";", parts);
            }
            set
            {
                var parts = value.Split(';');
                for (int i = 0; i < parts.Length && i < _brushProps.Length; i++)
                {
                    var prop = GetType().GetProperty(_brushProps[i]);
                    prop.SetValue(this, Serialize.StringToBrush(parts[i]));
                }
            }
        }

        #endregion

        #endregion
    }
}
