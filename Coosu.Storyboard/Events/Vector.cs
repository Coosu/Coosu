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

    public Vector(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public Vector()
    {
    }
}