using System;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public delegate CommonEvent EventCreationDelegate(EventType e, EasingFunctionBase easing,
        double startTime, double endTime,
        Span<double> start, Span<double> end);
}