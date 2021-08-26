using System;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Common
{
    public interface IKeyEvent : IEvent, IScriptable
    {
        EasingFunctionBase Easing { get; set; }
        double[] Start { get; set; }
        double[] End { get; set; }
        bool IsStatic { get; }
    }
}
