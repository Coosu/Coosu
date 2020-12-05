using System;

namespace Coosu.Storyboard
{
    public class StoryboardEventArgs : EventArgs
    {
        public string Message { get; set; }
        public virtual bool Continue { get; set; }
    }
}