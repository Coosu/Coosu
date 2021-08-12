namespace Coosu.Storyboard.Common
{
    public interface ITimingAdjustable
    {
        void AdjustTiming(float offset);
    }

    public interface IAdjustable : ITimingAdjustable, IPositionAdjustable
    {
    }
}
