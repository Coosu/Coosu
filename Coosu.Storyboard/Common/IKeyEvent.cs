using System;
using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Common
{
    public interface IKeyEvent : IEvent, IScriptable
    {
        float DefaultValue { get; }
        EasingFunctionBase Easing { get; set; }
        IReadOnlyList<float> Values { get; }

#if NET5_0_OR_GREATER
        Span<float> GetStartsSpan();
        Span<float> GetEndsSpan();
#endif

        bool IsHalfFilled { get; }
        bool IsFilled { get; }
        float GetValue(int index);
        void SetValue(int index, float value);
        void Fill();
        bool IsStartsEqualsEnds();
    }
}
