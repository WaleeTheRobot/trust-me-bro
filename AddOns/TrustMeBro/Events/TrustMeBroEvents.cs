using System;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Events
{
    public static class TrustMeBroEvents
    {
        /// <summary>
        /// Event triggered when the pivot points are updated.
        /// </summary>
        public static event Action OnPivotPointsUpdated;

        public static void NotifyPivotPointsUpdated()
        {
            EventManager.InvokeEvent(OnPivotPointsUpdated);
        }
    }
}

