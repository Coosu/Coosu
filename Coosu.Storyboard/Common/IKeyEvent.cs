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
        IEnumerable<double> GetStarts();
        IEnumerable<double> GetEnds();
        void SetStarts(IEnumerable<double> startValues);
        void SetEnds(IEnumerable<double> endValues);
        bool IsHalfFilled { get; }
        bool IsFilled { get; }
        double GetValue(int index);
        void SetValue(int index, double value);
        void Fill();
        bool IsStartsEqualsEnds();
    }
}
