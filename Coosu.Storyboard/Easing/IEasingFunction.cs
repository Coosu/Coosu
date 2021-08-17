namespace Coosu.Storyboard.Easing
{
    public interface IEasingFunction
    {
        double Ease(double normalizedTime);
        string GetDescription();
        EasingType GetEasingType();
        EasingType? TryGetEasingType();
    }
}