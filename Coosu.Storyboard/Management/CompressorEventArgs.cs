using System;

namespace Coosu.Storyboard.Management
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