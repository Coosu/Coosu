using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events;

public sealed class Vector : BasicEvent
{
    public override EventType EventType => EventTypes.Vector;
    public override double DefaultValue { get; } = 1;

    public double StartScaleX
    {
        get => GetValue(0);
        set => SetValue(0, value);
    }

    public double StartScaleY
    {
        get => GetValue(1);
        set => SetValue(1, value);
    }

    public double EndScaleX
    {
        get => GetValue(2);
        set => SetValue(2, value);
    }

    public double EndScaleY
    {
        get => GetValue(3);
        set => SetValue(3, value);
    }

    //public Vector(EasingFunctionBase easing, double startTime, double endTime, double vx1, double vy1, double vx2, double vy2) :
    //    base(easing, startTime, endTime, new[] { vx1, vy1 }, new[] { vx2, vy2 })
    //{
    //}

    public Vector(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public Vector()
    {
    }
}