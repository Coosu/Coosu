using System;

namespace OSharp.Storyboard
{
    public class CompressorEventArgs : StoryboardEventArgs
    {
        public CompressorEventArgs(Guid compressorGuid)
        {
            CompressorGuid = compressorGuid;
        }

        public Guid CompressorGuid { get; }
    }
}