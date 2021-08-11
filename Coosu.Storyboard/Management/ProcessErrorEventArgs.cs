namespace Coosu.Storyboard.Management
{
    public class ProcessErrorEventArgs : StoryboardEventArgs
    {
        public override bool Continue { get; set; } = false;
    }
}