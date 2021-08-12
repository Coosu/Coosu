namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class ProcessErrorEventArgs : StoryboardEventArgs
    {
        public override bool Continue { get; set; } = false;
    }
}