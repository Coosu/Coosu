using System;

namespace Coosu.Storyboard
{
    public class ProgressEventArgs : CompressorEventArgs
    {
        public int Progress { get; set; }
        public int TotalCount { get; set; }

        public ProgressEventArgs(Guid compressorGuid) : base(compressorGuid)
        {
        }
    }
}