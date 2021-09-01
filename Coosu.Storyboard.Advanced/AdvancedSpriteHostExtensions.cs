using System;
using System.IO;
using Coosu.Storyboard.Advanced;
using Coosu.Storyboard.Advanced.Text;
using Coosu.Storyboard.Common;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class AdvancedSpriteHostExtensions
    {
        public static SpriteGroup CreateText(this ISpriteHost spriteHost,
            string text,
            double startTime,
            double initialX,
            double initialY,
            Action<CoosuTextOptionsBuilder> configure)
        {
            var builder = new CoosuTextOptionsBuilder();
            configure.Invoke(builder);
            return CreateText(spriteHost, text, startTime, initialX, initialY, LayerType.Foreground, OriginType.Centre, builder.Options);
        }

        public static SpriteGroup CreateText(this ISpriteHost spriteHost,
            string text,
            double startTime,
            double initialX,
            double initialY,
            OriginType origin,
            Action<CoosuTextOptionsBuilder> configure)
        {
            var builder = new CoosuTextOptionsBuilder();
            configure.Invoke(builder);
            return CreateText(spriteHost, text, startTime, initialX, initialY, LayerType.Foreground, origin, builder.Options);
        }

        public static SpriteGroup CreateText(this ISpriteHost spriteHost,
            string text,
            double startTime,
            double initialX,
            double initialY,
            LayerType layer = LayerType.Foreground,
            OriginType origin = OriginType.Centre,
            CoosuTextOptions? textOptions = null)
        {
            textOptions ??= CoosuTextOptions.Default;
            if (textOptions.FileIdentifier == null)
                throw new ArgumentNullException("textOptions.FileIdentifier",
                    "The text's FileIdentifier shouldn't be null.");

            var spriteGroup = new SpriteGroup(initialX, initialY, spriteHost.Camera2.DefaultZ, origin)
            {
                Camera2 =
                {
                    CameraIdentifier = Guid.NewGuid().ToString()
                },
                BaseHost = spriteHost,
            };
            ISpriteHost @base = spriteHost;
            ISpriteHost tempHost = spriteHost;
            while (tempHost.BaseHost != null)
            {
                @base = tempHost.BaseHost;
                tempHost = tempHost.BaseHost;
            }

            var layer1 = (Layer)@base;

            layer1.Tags["text:" + spriteGroup.Camera2.CameraIdentifier] = new TextContext
            {
                Layer = layer,
                Origin = origin,
                StartTime = startTime,
                Text = text,
                TextOptions = textOptions,
                SpriteGroup = spriteGroup
            };
            //var dict = TextHelper.ProcessText(new TextContext()).Result;
            //var totalCalculateWidth = 0;
            //var totalCalculateHeight = 0;
            //for (var i = 0; i < text.Length; i++)
            //{
            //    var c = text[i];
            //    var unicode = TextHelper.CharToUnicode(c);
            //    var fileName = unicode + "_" + textOptions.FileIdentifier;
            //    var path = Path.Combine(Directories.CoosuTextDir, fileName + ".png");

            //    var x = 0;
            //    var y = 0;
            //    spriteGroup.CreateSprite(path, layer, origin, x, y);
            //}

            spriteHost.AddSubHost(spriteGroup);
            return spriteGroup;
        }
    }
}