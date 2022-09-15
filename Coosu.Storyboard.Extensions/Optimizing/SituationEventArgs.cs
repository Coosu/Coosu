using System;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Optimizing;

public class SituationEventArgs : CompressorEventArgs
{
    public SituationEventArgs(Guid compressorGuid, SituationType situationType) : base(compressorGuid)
    {
        SituationType = situationType;
        Message = situationType.GetDescription();
    }

    public override bool Continue { get; set; } = true;
    public IKeyEvent[] Events { get; set; }
    public IDetailedEventHost Host { get; set; }
    public SituationType SituationType { get; }
    public Sprite? Sprite { get; set; }
}