using System;
using System.Collections.Generic;

namespace Coosu.Storyboard.Common
{
    public interface ISpriteHost : IScriptable, ICloneable, IEnumerable<Sprite>
    {
        float MaxTime { get; }
        float MinTime { get; }
        float MaxStartTime { get; }
        float MinEndTime { get; }
        IList<Sprite> Sprites { get; }
        IList<ISpriteHost> SubHosts { get; }
        Camera2 Camera2 { get; }
        void AddSprite(Sprite sprite);
        void AddSubHost(ISpriteHost spriteHost);
        ISpriteHost? BaseHost { get; }
    }
}