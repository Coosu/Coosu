namespace Coosu.Storyboard.Common
{
    public interface ICommonEvent : IEvent, IScriptable
    {
        EasingType Easing { get; set; }
        double[] Start { get; set; }
        double[] End { get; set; }
        int ParamLength { get; }
        bool IsStatic { get; }
    }
}
