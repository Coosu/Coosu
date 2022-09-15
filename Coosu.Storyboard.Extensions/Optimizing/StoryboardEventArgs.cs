using System;

namespace Coosu.Storyboard.Extensions.Optimizing;

public class StoryboardEventArgs : EventArgs
{
    public string? Message { get; set; }
    public virtual bool Continue { get; set; }
}