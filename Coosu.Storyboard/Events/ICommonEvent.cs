namespace Coosu.Storyboard.Events
{
    public interface ICommonEvent : IEvent
    {
        EasingType Easing { get; set; }
        float[] Start { get; set; }
        float[] End { get; set; }
        int ParamLength { get; }
        bool IsStatic { get; }
    }
}
