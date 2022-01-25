using System;
using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Common
{
    public interface IKeyEvent : IEvent, IScriptable
    {
        double DefaultValue { get; }
        EasingFunctionBase Easing { get; set; }
        IReadOnlyList<double> Values { get; }

#if NET5_0_OR_GREATER
        Span<double> GetStartsSpan();
        Span<double> GetEndsSpan();
#endif

        bool IsHalfFilled { get; }
        bool IsFilled { get; }
        double GetValue(int index);
        void SetValue(int index, double value);
        void Fill();
        bool IsStartsEqualsEnds();
    }
}
