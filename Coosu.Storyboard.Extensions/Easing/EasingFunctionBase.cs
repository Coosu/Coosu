namespace Coosu.Storyboard.Extensions.Easing
{
    public abstract class EasingFunctionBase : IEasingFunction
    {
        public EasingMode EasingMode
        {
            get;
            set;
        }

        public double Ease(double normalizedTime)
        {
            switch (this.EasingMode)
            {
                case EasingMode.EaseIn:
                    return this.EaseInCore(normalizedTime);
                case EasingMode.EaseOut:
                    return 1.0 - this.EaseInCore(1.0 - normalizedTime);
                default:
                    return normalizedTime >= 0.5 ? (1.0 - this.EaseInCore((1.0 - normalizedTime) * 2.0)) * 0.5 + 0.5 : this.EaseInCore(normalizedTime * 2.0) * 0.5;
            }
        }

        public string GetDescription()
        {
            return GetType().Name + EasingMode.ToString().Substring(4);
        }

        protected abstract double EaseInCore(double normalizedTime);
    }
}
