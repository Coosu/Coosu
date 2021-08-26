using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class LayerExtensions
    {
        public static void CreateText(this Layer layer, string text, Action<TextOptionsBuilder> configure)
        {
            var builder = new TextOptionsBuilder();
            configure.Invoke(builder);
            CreateText(layer, text, builder.Options);
        }

        public static void CreateText(this Layer layer, string text, TextOptions? textOptions = null)
        {
            textOptions ??= TextOptions.Default;
            if (textOptions.FileIdentifier == null)
                throw new ArgumentNullException("textOptions.FileIdentifier",
                    "The text's FileIdentifier shouldn't be null.");
        }
    }

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

    public enum OptionType
    {
        None, With, Only
    }

    public class FontFamilySource
    {
        public string Name { get; }
        public string? Path { get; private set; }

        public FontFamilySource(string name)
        {
            Name = name;
        }

        public static FontFamilySource FromFiles(string path, string name)
        {
            return new FontFamilySource(name) { Path = path };
        }

        public static implicit operator FontFamilySource(string s)
        {
            return new FontFamilySource(s);
        }
    }
}