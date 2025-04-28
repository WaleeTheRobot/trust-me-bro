using NinjaTrader.Custom.AddOns.TrustMeBro;
using NinjaTrader.Custom.AddOns.TrustMeBro.Events;
using System.ComponentModel.DataAnnotations;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class SmartyPants : Indicator
    {
        private TrustMeBro _trustMeBro;
        private bool _linked = false;
        private string _key;

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Period", Order = 0, GroupName = "Parameters")]
        public int Period { get; set; }

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Looks smart, probably just nonsense.";
                Name = "_SmartyPants";
                Calculate = Calculate.OnEachTick;
                IsOverlay = false;
                DisplayInDataBox = true;
                DrawOnPricePanel = false;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 5;

            }
            if (State == State.DataLoaded)
            {
                _key = Instrument.FullName + "_" + BarsPeriod.Value + "_" + BarsPeriod.BarsPeriodType;

                DataEvents.OnFirstTickOfBarDone += HandleFirstTickOfBarDone;
                DataEvents.OnKFSeriesUpdated += HandleKFSeriesUpdated;
            }
        }

        protected override void OnBarUpdate()
        {

        }

        private void HandleFirstTickOfBarDone()
        {

        }

        private void HandleKFSeriesUpdated()
        {
            if (!_linked)
            {
                _trustMeBro = TrustMeBroService.Get(_key);

                if (_trustMeBro != null)
                {
                    _linked = true;
                }
                else
                {
                    return;
                }
            }

            if (_linked && _trustMeBro != null)
            {
                double latestKalmanClose = _trustMeBro.KfCloseSeries[0];
                //Print("SmartyPants Close: " + latestKalmanClose);
            }
        }

    }
}
