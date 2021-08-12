using System;
using Coosu.Storyboard.Management;

namespace Coosu.Storyboard.Extensions.Optimizing
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