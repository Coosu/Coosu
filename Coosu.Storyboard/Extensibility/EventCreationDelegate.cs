using System.Collections.Generic;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility;

public delegate BasicEvent EventCreationDelegate(EventType e, EasingFunctionBase easing,
    double startTime, double endTime, List<double> values);