namespace Coosu.Storyboard.Advanced
{
    public class TextOptionsBuilder
    {
        public TextOptionsBuilder ScaleXBy(double ratio)
        {
            Options.XScale = ratio;
            return this;
        }

        public TextOptionsBuilder ScaleYBy(double ratio)
        {
            Options.YScale = ratio;
            return this;
        }

        public TextOptionsBuilder Reverse()
        {
            Options.RightToLeft = !Options.RightToLeft;
            return this;
        }

        public TextOptionsBuilder WithFontSize(int fontSize)
        {
            Options.FontSize = fontSize;
            return this;
        }

        public TextOptions Options { get; } = new();
    }
}