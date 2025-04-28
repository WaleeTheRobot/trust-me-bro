using System.Collections.Generic;
using TMB = NinjaTrader.NinjaScript.Indicators.TrustMeBro;

namespace NinjaTrader.Custom.AddOns.TrustMeBro
{
    public static class TrustMeBroService
    {
        private static Dictionary<string, TMB> _instances = new Dictionary<string, TMB>();

        public static void Register(string key, TMB instance)
        {
            _instances[key] = instance;
        }

        public static TMB Get(string key)
        {
            if (_instances.TryGetValue(key, out var instance))
                return instance;

            return null;
        }

        public static void Unregister(string key)
        {
            if (_instances.ContainsKey(key))
                _instances.Remove(key);
        }
    }
}
