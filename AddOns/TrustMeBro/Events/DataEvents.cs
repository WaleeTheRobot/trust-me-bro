using System;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Events
{
    public static class DataEvents
    {
        public static event Action OnKFSeriesUpdated;
        public static event Action OnFirstTickOfBarDone;

        public static void KFSeriesUpdated()
        {
            EventManager.InvokeEvent(OnKFSeriesUpdated);
        }

        public static void FirstTickOfBarDone()
        {
            EventManager.InvokeEvent(OnFirstTickOfBarDone);
        }
    }
}
