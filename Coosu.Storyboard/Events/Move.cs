using System.Collections.Generic;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events;

public sealed class Move : BasicEvent, IPositionAdjustable
{
    public override EventType EventType => EventTypes.Move;

    public double StartX
    {
        get => GetValue(0);
        set => SetValue(0, value);
    }

    public double StartY
    {
        get => GetValue(1);
        set => SetValue(1, value);
    }

    public double EndX
    {
        get => GetValue(2);
        set => SetValue(2, value);
    }

    public double EndY
    {
        get => GetValue(3);
        set => SetValue(3, value);
    }

    public Move(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public Move()
    {
    }

    public void AdjustPosition(double x, double y)
    {
        StartX += x;
        StartY += y;
        EndX += x;
        EndY += y;
    }
}