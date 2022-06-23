using System.Diagnostics;
using System.IO;

namespace Coosu.Beatmap.Extensions.Playback
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class PlayableNode : HitsoundNode
    {
        public PlayablePriority PlayablePriority { get; set; }

        public string DebuggerDisplay => $"PL{(UseUserSkin ? "D" : "")}:{Offset}: " +
                                         $"P{(int)PlayablePriority}: " +
                                         $"V{(Volume * 10):#.#}: " +
                                         $"B{(Balance * 10):#.#}: " +
                                         $"{(Filename)}";
    }
}