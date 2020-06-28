using System;

namespace OSharp.Storyboard
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