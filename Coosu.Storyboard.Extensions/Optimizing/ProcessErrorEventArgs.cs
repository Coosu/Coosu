using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class ProcessErrorEventArgs : StoryboardEventArgs
    {
        public ProcessErrorEventArgs(IEventHost sourceSprite)
        {
            SourceSprite = sourceSprite;
        }

        public IEventHost SourceSprite { get; set; }
        public override bool Continue { get; set; } = false;
    }
}