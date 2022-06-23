using System.Diagnostics;
using System.IO;

namespace Coosu.Beatmap.Extensions.Playback
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ControlNode : HitsoundNode
    {
        public SlideChannel SlideChannel { get; internal set; }
        public ControlType ControlType { get; internal set; }

        public string DebuggerDisplay => $"CT{(UseUserSkin ? "D" : "")}:{Offset}: " +
                                         $"O{Offset}: " +
                                         $"T{(int)ControlType}{(ControlType is ControlType.StartSliding or ControlType.StopSliding ? (int)SlideChannel : "")}: " +
                                         $"V{(Volume * 10):#.#}: " +
                                         $"B{(Balance * 10):#.#}: " +
                                         $"{(Filename == null ? "" : Path.GetFileNameWithoutExtension(Filename))}";
    }
}