using System;

namespace Coosu.Storyboard.Extensions.Optimizing;

public class ProgressEventArgs : CompressorEventArgs
{
    public ProgressEventArgs(Guid compressorGuid) : base(compressorGuid)
    {
    }

    public int Progress { get; set; }
    public int TotalCount { get; set; }
}