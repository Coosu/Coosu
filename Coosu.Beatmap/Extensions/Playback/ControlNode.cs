namespace Coosu.Beatmap.Extensions.Playback
{
    public class ControlNode : HitsoundNode
    {
        public SlideChannel SlideChannel { get; internal set; }
        public ControlType ControlType { get; internal set; }
    }
}