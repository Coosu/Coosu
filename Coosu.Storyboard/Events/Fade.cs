﻿using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events;

public sealed class Fade : BasicEvent
{
    public override EventType EventType => EventTypes.Fade;
    public override double DefaultValue { get; } = 1;

    public double StartOpacity
    {
        get => GetValue(0);
        set => SetValue(0, value);
    }

    public double EndOpacity
    {
        get => GetValue(1);
        set => SetValue(1, value);
    }

    public Fade(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
        : base(easing, startTime, endTime, values)
    {
    }

    public Fade() { }
}