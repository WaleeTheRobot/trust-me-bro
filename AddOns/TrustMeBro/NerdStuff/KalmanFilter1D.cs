using System;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.NerdStuff
{
    // Kalman Filter adapts smoothing strength based on esstimated noise and drift.
    // When raw values swings more wildly, it leans on its prior estiamte more, damping noise.
    // When raw values shift more persistently, its estimates catches up so it doesn't lag forever like a moving average.
    public class KalmanFilter1D
    {
        // Estimate covariance. Uncertainty of state estimate.
        private double _P;
        // Process noise variance. Smaller Q makes filter more sluggish and trusts prior state more and resists sudden shifts.
        private double _Q;
        // Measurement noise variance. Larger R means filter trusts measurement less and smooths more aggressively.
        private double _R;
        // Current state estimate
        public double X { get; private set; }

        public KalmanFilter1D(double initialEstimate, double initialP, double processNoise, double measurementNoise)
        {
            X = initialEstimate;
            _P = initialP;
            _Q = processNoise;
            _R = measurementNoise;
        }

        public void SetNoiseParameters(double processNoise, double measurementNoise)
        {
            _Q = Math.Max(1e-6, processNoise);
            _R = Math.Max(1e-5, measurementNoise);
        }

        public double Update(double measurement)
        {
            // Predict
            _P += _Q;
            // Compute Kalman gain
            double K = _P / (_P + _R);
            // Update estimate with measurement residual
            X = X + K * (measurement - X);
            // Update covariance
            _P = (1 - K) * _P;

            return X;
        }
    }
}
