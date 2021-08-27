using System;
using System.Numerics;
using Coosu.Storyboard.Advanced.Texting;
using Coosu.Storyboard.Common;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class LayerExtensions
    {
        public static SpriteGroup CreateText(this ISpriteHost layer,
            string text,
            double startTime,
            double initialX,
            double initialY,
            Action<CoosuTextOptionsBuilder> configure)
        {
            var builder = new CoosuTextOptionsBuilder();
            configure.Invoke(builder);
            return CreateText(layer, text, startTime, initialX, initialY, OriginType.Centre, builder.Options);
        }

        public static SpriteGroup CreateText(this ISpriteHost layer,
            string text,
            double startTime,
            double initialX,
            double initialY,
            OriginType origin,
            Action<CoosuTextOptionsBuilder> configure)
        {
            var builder = new CoosuTextOptionsBuilder();
            configure.Invoke(builder);
            return CreateText(layer, text, startTime, initialX, initialY, origin, builder.Options);
        }

        public static SpriteGroup CreateText(this ISpriteHost layer,
            string text,
            double startTime,
            double initialX,
            double initialY,
            OriginType origin = OriginType.Centre,
            CoosuTextOptions? textOptions = null)
        {
            textOptions ??= CoosuTextOptions.Default;
            if (textOptions.FileIdentifier == null)
                throw new ArgumentNullException("textOptions.FileIdentifier",
                    "The text's FileIdentifier shouldn't be null.");

            throw new NotImplementedException();
        }
    }
}