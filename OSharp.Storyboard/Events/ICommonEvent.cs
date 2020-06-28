namespace OSharp.Storyboard.Events
{
    public interface ICommonEvent : IEvent
    {
        EventType EventType { get; }
        EasingType Easing { get; set; }
        float[] Start { get; }
        float[] End { get; }
        int ParamLength { get; }
        bool IsStatic { get; }
    }
}
