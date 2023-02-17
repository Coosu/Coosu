using System.Collections.Generic;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions;

public sealed class MoveZ : BasicEvent
{
    public override EventType EventType { get; } = new("MZ", 1, 11);

    public double StartZ
    {
        get => GetValue(0);
        set => SetValue(0, value);
    }

    public double EndZ
    {
        get => GetValue(1);
        set => SetValue(1, value);
    }

    public MoveZ(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public MoveZ()
    {
    }
    
}