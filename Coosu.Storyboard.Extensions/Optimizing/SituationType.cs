namespace Coosu.Storyboard.Extensions.Optimizing;

public enum SituationType
{
    ThisLastSingleInLastInvisibleToFixTail,
    ThisLastSingleInLastInvisibleToFixEndTime,
    ThisLastInLastInvisibleToRemove,
    NextHeadAndThisInInvisibleToRemove,
    ThisFirstSingleIsStaticAndDefaultToRemove,
    ThisFirstIsStaticAndSequentWithNextHeadToRemove,
    MoveSingleIsStaticToRemoveAndChangeInitial,
    MoveSingleEqualsInitialToRemove,
    InitialToZero,
    ThisPrevIsStaticAndSequentToCombine,
    ThisIsStaticAndSequentWithPrevToCombine,
    PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove,
}