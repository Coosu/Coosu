using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events;

public sealed class Rotate : BasicEvent
{
    public override EventType EventType => EventTypes.Rotate;

    public double StartRotate
    {
        get => GetValue(0);
        set => SetValue(0, value);
    }

    public double EndRotate
    {
        get => GetValue(1);
        set => SetValue(1, value);
    }

    //public Rotate(EasingFunctionBase easing, double startTime, double endTime, double r1, double r2) :
    //    base(easing, startTime, endTime, new[] { r1 }, new[] { r2 })
    //{
    //}

    public Rotate(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public Rotate()
    {
    }
}