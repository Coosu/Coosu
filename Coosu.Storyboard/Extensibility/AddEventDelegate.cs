using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public delegate CommonEvent AddEventDelegate(EventType e, EasingType easing, float startTime, float endTime, float[] start, float[] end);
}