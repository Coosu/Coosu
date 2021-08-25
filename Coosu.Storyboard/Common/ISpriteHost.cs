using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface ISpriteHost : IScriptable, ICloneable, IEnumerable<Sprite>
    {
        double MaxTime { get; }
        double MinTime { get; }
        double MaxStartTime { get; }
        double MinEndTime { get; }
        ICollection<Sprite> Sprites { get; }
        Camera2 Camera2 { get; }
    }

    public struct State
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Scale { get; set; }
        public double Rotation { get; set; }
    }
}