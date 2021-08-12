namespace Coosu.Storyboard.Common
{
    public interface ICommonEvent : IEvent, IScriptable
    {
        EasingType Easing { get; set; }
        float[] Start { get; set; }
        float[] End { get; set; }
        int ParamLength { get; }
        bool IsStatic { get; }
    }
}
