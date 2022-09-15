using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Storybrew;
using Coosu.Storyboard.Storybrew.Text;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard;

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
        var enumerable = text.Where(k => k >= 32 && k != 127);
        if (textOptions.RightToLeft) enumerable = enumerable.Reverse();
        var textArr = enumerable.ToArray();
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
            StartTime = startTime,
            Text = textArr,
            TextOptions = textOptions,
            SpriteGroup = spriteGroup
        };
        //var dict = TextHelper.ProcessText(new TextContext()).Result;
        //var totalCalculateWidth = 0;
        //var totalCalculateHeight = 0;
        bool scale = !textOptions.XScale.Equals(1) || !textOptions.YScale.Equals(1);
        if (textOptions.ShowShadow)
        {
            for (var i = 0; i < textArr.Length; i++)
            {
                var c = textArr[i];
                if (c == ' ') continue;
                var fileName = TextHelper.ConvertToFileName(c, textOptions.FileIdentifier + "_", "_bl");
                var filePath = Path.Combine(Directories.CoosuTextDir, fileName);
                var r = textOptions.ShadowDepth;
                var deg = textOptions.ShadowDirection;
                var x = r * Math.Cos(deg / 180d * Math.PI);
                var y = r * Math.Sin(deg / 180d * Math.PI);
                var sprite = spriteGroup.CreateSprite(filePath, layer, textOptions.Origin, (double)x, (double)y);
                if (scale) AdjustScale(sprite, startTime, textOptions);
                sprite.Tag = i;
            }
        }

        if (textOptions.ShowStroke)
        {
            for (var i = 0; i < textArr.Length; i++)
            {
                var c = textArr[i];
                if (c == ' ') continue;
                var fileName = TextHelper.ConvertToFileName(c, textOptions.FileIdentifier + "_", "_st");
                var filePath = Path.Combine(Directories.CoosuTextDir, fileName);

                var sprite = spriteGroup.CreateSprite(filePath, layer, textOptions.Origin, 0, 0);
                if (scale) AdjustScale(sprite, startTime, textOptions);
                sprite.Tag = i;
            }
        }

        if (textOptions.ShowBase)
        {
            for (var i = 0; i < textArr.Length; i++)
            {
                var c = textArr[i];
                if (c == ' ') continue;
                var fileName = TextHelper.ConvertToFileName(c, textOptions.FileIdentifier + "_", "");
                var filePath = Path.Combine(Directories.CoosuTextDir, fileName);

                var sprite = spriteGroup.CreateSprite(filePath, layer, textOptions.Origin, 0, 0);
                if (scale) AdjustScale(sprite, startTime, textOptions);
                sprite.Tag = i;
            }
        }

        spriteHost.AddSubHost(spriteGroup);
        return spriteGroup;
    }

    private static void AdjustScale(IEventHost sprite, double startTime, CoosuTextOptions textOptions)
    {
        var isVertical = textOptions.Orientation == Orientation.Vertical;
        var useYAdd = isVertical && textOptions.RotateBy90 == false;
        if (!isVertical)
        {
            sprite.Vector(startTime, (double)textOptions.XScale, (double)textOptions.YScale);
        }
        else if (!useYAdd)
        {
            sprite.Vector(startTime, (double)textOptions.YScale, (double)textOptions.XScale);
            sprite.Rotate(startTime, (double)(Math.PI / 2));
        }
        else
        {
            sprite.Vector(startTime, (double)textOptions.XScale, (double)textOptions.YScale);
        }
    }
}