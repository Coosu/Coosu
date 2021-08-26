using System;
using Coosu.Storyboard.Advanced;

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
}