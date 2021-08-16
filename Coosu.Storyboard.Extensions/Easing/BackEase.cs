﻿using System;

namespace Coosu.Storyboard.Extensions.Easing
{
    /// <summary>
    /// This class implements an easing function that backs up before going to the destination.
    /// </summary>
    public class BackEase : EasingFunctionBase
    {
        public BackEase()
        {
        }

        /// <summary>
        /// Specifies how much the function will pull back
        /// </summary>
        public double Amplitude { get; set; } = 1;

        protected override double EaseInCore(double normalizedTime)
        {
            double amp = Math.Max(0.0, Amplitude);
            return Math.Pow(normalizedTime, 3.0) - normalizedTime * amp * Math.Sin(Math.PI * normalizedTime);
        }
    }
}