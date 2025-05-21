using NinjaTrader.Gui;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class TrustMeBro : Indicator
    {
        public const string GROUP_NAME_PLOTS = "Plots";

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Trend Line Color", Description = "The trend line color.", GroupName = GROUP_NAME_PLOTS, Order = 0)]
        public Brush TrendLineColor { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Trend Line Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 1)]
        public DashStyleHelper TrendLineDashStyle { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Trend Line Width", GroupName = GROUP_NAME_PLOTS, Order = 2)]
        public int TrendLineWidth { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Level Color", Description = "The level color.", GroupName = GROUP_NAME_PLOTS, Order = 3)]
        public Brush LevelColor { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Level Dash Style", GroupName = GROUP_NAME_PLOTS, Order = 4)]
        public DashStyleHelper LevelDashStyle { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Level Width", GroupName = GROUP_NAME_PLOTS, Order = 5)]
        public int LevelWidth { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Trend Line Opacity", Description = "The opacity for the trend line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 6)]
        public byte TrendLineOpacity { get; set; }

        [NinjaScriptProperty, XmlIgnore]
        [Display(Name = "Level Opacity", Description = "The opacity for the levels. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 7)]
        public byte LevelOpacity { get; set; }
    }
}
