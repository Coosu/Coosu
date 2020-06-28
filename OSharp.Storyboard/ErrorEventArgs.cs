namespace OSharp.Storyboard
{
    public class ErrorEventArgs : StoryboardEventArgs
    {
        public override bool Continue { get; set; } = false;
    }
}