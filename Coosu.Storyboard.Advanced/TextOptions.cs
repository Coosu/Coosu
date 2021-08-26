using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Coosu.Storyboard.Advanced
{
    public class TextOptions
    {
        public string? FileIdentifier { get; set; }
        public bool RightToLeft { get; set; } = false;
        //xs
        public double XScale { get; set; } = 1;
        //ys
        public double YScale { get; set; } = 1;
        public double WordGap { get; set; }
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
        //fsnm;fsob;fsit
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        //fw400
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        //s36
        public int FontSize { get; set; } = 36;

        public Brush FillColor { get; set; } = Brushes.White;
        //ston;st
        public OptionType Stroke { get; set; }
        public Brush? StrokeColor { get; set; }
        //sdon;sd
        public OptionType Shadow { get; set; }
        public Brush? ShadowColor { get; set; }
        public List<FontFamilySource> FontFamilies { get; set; } = new() { "Arial" };

        internal static TextOptions Default { get; } = new TextOptions()
        {
            FileIdentifier = "default"
        };
    }
}