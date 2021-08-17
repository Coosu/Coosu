using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Common
{
    public interface ICommonEvent : IEvent, IScriptable
    {
        IEasingFunction Easing { get; set; }
        double[] Start { get; set; }
        double[] End { get; set; }
        int ParamLength { get; }
        bool IsStatic { get; }
    }
}
