using System;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class SituationEventArgs : CompressorEventArgs
    {
        public SituationType SituationType { get; }
        public Sprite? Sprite { get; set; }
        public IDetailedEventHost Host { get; set; }
        public ICommonEvent[] Events { get; set; }
        public override bool Continue { get; set; } = true;

        public SituationEventArgs(Guid compressorGuid, SituationType situationType) : base(compressorGuid)
        {
            SituationType = situationType;
            Message = situationType.GetDescription();
        }
    }

    public enum SituationType
    {
        ThisLastSingleInLastObsoleteToFixTail,
        ThisLastSingleInLastObsoleteToFixEndTime,
        ThisLastInLastObsoleteToRemove,
        NextHeadAndThisInObsoleteToRemove,
        ThisFirstSingleIsStaticAndDefaultToRemove,
        ThisFirstIsStaticAndSequentWithNextHeadToRemove,
        MoveSingleIsStaticToRemoveAndChangeInitial,
        MoveSingleEqualsInitialToRemove,
        InitialToZero,
        ThisPrevIsStaticAndSequentToCombine,
        ThisIsStaticAndSequentWithPrevToCombine,
        PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove,
    }

    public static class SituationExtension
    {
        public static string GetDescription(this SituationType situationType)
        {
            switch (situationType)
            {
                case SituationType.ThisLastSingleInLastObsoleteToFixTail:
                    return "The only event for its category in the obsolete range which range equals to the sprite's end time " +
                           "can be optimized when string value of end parameters is longer than the start parameters. ";
                case SituationType.ThisLastSingleInLastObsoleteToFixEndTime:
                    return "The only event for its category in the obsolete range which range equals to the sprite's end time " +
                           "can be optimized when string value of end time is longer than the start time. ";
                case SituationType.ThisLastInLastObsoleteToRemove:
                    return "The event in the obsolete range which range's end time equals to the sprite's end time " +
                           "is useless and can be safely removed.";
                case SituationType.NextHeadAndThisInObsoleteToRemove:
                    return "When one event is in an obsolete range and the next event's start time is in the same obsolete range, " +
                           "the event is useless and can be safely removed.";
                case SituationType.ThisFirstSingleIsStaticAndDefaultToRemove:
                    return "ThisFirstSingleIsStaticAndDefaultToRemove.";
                case SituationType.ThisFirstIsStaticAndSequentWithNextHeadToRemove:
                    return "ThisFirstIsStaticAndSequentWithNextHeadToRemove.";
                case SituationType.MoveSingleIsStaticToRemoveAndChangeInitial:
                    return "MoveSingleIsStaticToRemoveAndChangeInitial.";
                case SituationType.MoveSingleEqualsInitialToRemove:
                    return "MoveSingleEqualsInitialToRemove.";
                case SituationType.InitialToZero:
                    return "InitialToZero.";
                case SituationType.ThisPrevIsStaticAndSequentToCombine:
                    return "ThisPrevIsStaticAndSequentToCombine.";
                case SituationType.ThisIsStaticAndSequentWithPrevToCombine:
                    return "ThisIsStaticAndSequentWithPrevToCombine.";
                case SituationType.PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove:
                    return "PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove.";
                default:
                    throw new ArgumentOutOfRangeException(nameof(situationType), situationType, null);
            }
        }
    }
}