using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public delegate CommonEvent EventCreationDelegate(EventType e, IEasingFunction easing,
        double startTime, double endTime,
        double[] start, double[] end);
}