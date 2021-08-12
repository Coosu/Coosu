using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public delegate CommonEvent EventCreationDelegate(EventType e, EasingType easing,
        double startTime, double endTime,
        double[] start, double[] end);
}