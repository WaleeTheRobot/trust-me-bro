using NinjaTrader.Custom.AddOns.TrustMeBro.Common;
using System;
using System.Collections.Generic;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Pivots
{
    public class PivotManager
    {
        private BarData _previousBar = null;
        private BarData _currentBar = null;
        private readonly List<PivotPoint> _pivots = null;
        private bool _hasFirstPivot = false;
        private bool _isLookingForHigh = false;
        private double _currentMaxHigh, _currentMinLow;
        private int _maxHighBar, _minLowBar;
        private readonly double _threshold;

        public PivotManager(int pivotLimit, double threshold)
        {
            _threshold = threshold;

            _pivots = new List<PivotPoint>(pivotLimit);
            _currentMaxHigh = 0;
            _maxHighBar = 0;
            _currentMinLow = 0;
            _minLowBar = 0;
        }

        public void ProcessPivotPoint(BarData previousBar, BarData currentBar, Func<int, BarData> barLookup)
        {
            _previousBar = previousBar;
            _currentBar = currentBar;

            if (IsFirstPivot())
                return;

            // Skip inside bar
            if (_currentBar.High < _previousBar.High && _currentBar.Low > _previousBar.Low)
                return;

            if (_isLookingForHigh)
                IsLookingForHigh(barLookup);
            else
                IsLookingForLow(barLookup);
        }

        private bool IsFirstPivot()
        {
            if (_hasFirstPivot)
                return false;

            bool isBullish = _previousBar.Open <= _previousBar.Close;
            var pivot = new PivotPoint(_previousBar, !isBullish);
            _pivots.Add(pivot);

            _isLookingForHigh = isBullish;
            if (isBullish)
            {
                _currentMaxHigh = _currentBar.High;
                _maxHighBar = _currentBar.BarNumber;
            }
            else
            {
                _currentMinLow = _previousBar.Low;
                _minLowBar = _currentBar.BarNumber;
            }

            _hasFirstPivot = true;
            return true;
        }

        private void IsLookingForHigh(Func<int, BarData> lookup)
        {
            // Still making new highs
            if (_currentBar.High > _currentMaxHigh)
            {
                _currentMaxHigh = _currentBar.High;
                _maxHighBar = _currentBar.BarNumber;

                return;
            }

            // New high found
            var swingBar = lookup(_maxHighBar);
            var swing = new PivotPoint(swingBar, true);
            _pivots.Add(swing);

            _isLookingForHigh = false;
            _currentMinLow = _currentBar.Low;
            _minLowBar = _currentBar.BarNumber;
        }

        private void IsLookingForLow(Func<int, BarData> lookup)
        {
            // Still making new lows
            if (_currentBar.Low < _currentMinLow)
            {
                _currentMinLow = _currentBar.Low;
                _minLowBar = _currentBar.BarNumber;

                return;
            }

            // New low found
            var swingBar = lookup(_minLowBar);
            var swing = new PivotPoint(swingBar, false);
            _pivots.Add(swing);

            _isLookingForHigh = true;
            _currentMaxHigh = _currentBar.High;
            _maxHighBar = _currentBar.BarNumber;
        }

        public void UpdatePivotStatus()
        {
            foreach (var pivot in _pivots)
            {
                if (!pivot.DisplayLevel)
                    continue;

                double pivotClose = pivot.BarData.Close;

                if (pivot.IsHigh)
                {
                    if (_currentBar.Open > pivotClose + _threshold)
                        pivot.DisplayLevel = false;
                }
                else
                {
                    if (_currentBar.Open < pivotClose - _threshold)
                        pivot.DisplayLevel = false;
                }
            }
        }
    }
}
