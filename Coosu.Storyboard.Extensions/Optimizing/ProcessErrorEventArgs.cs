using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Optimizing;

public class ProcessErrorEventArgs : StoryboardEventArgs
{
    public ProcessErrorEventArgs(IDetailedEventHost sourceSprite)
    {
        SourceSprite = sourceSprite;
    }

    public override bool Continue { get; set; } = false;
    public IDetailedEventHost SourceSprite { get; set; }
}