using System;

namespace Coosu.Storyboard.Extensions.Optimizing
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