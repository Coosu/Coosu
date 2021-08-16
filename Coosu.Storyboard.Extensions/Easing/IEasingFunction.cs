namespace Coosu.Storyboard.Extensions.Easing
{
    public interface IEasingFunction
    {
        double Ease(double normalizedTime);
        string GetDescription();
    }
}