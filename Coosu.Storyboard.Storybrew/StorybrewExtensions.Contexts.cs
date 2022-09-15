using System;
using System.Linq;
using System.Windows.Controls;
using Coosu.Shared.Numerics;
using Coosu.Storyboard.Storybrew.Text;
using StorybrewCommon.Scripting;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard;

public static partial class StorybrewExtensions
{
    private static void DelayExecuteContexts(Layer layer, StoryboardObjectGenerator brewObjectGenerator)
    {
        layer.Tags
            .AsParallel()
            //.WithDegreeOfParallelism(1)
            .ForAll(layerTag =>
            {
                if (layerTag.Key.StartsWith("text:", StringComparison.Ordinal))
                {
                    DelayExecuteText((TextContext)layerTag.Value, brewObjectGenerator);
                }
            });

        //foreach (var layerTag in layer.Tags)
        //{
        //    if (layerTag.Key.StartsWith("text:"))
        //    {
        //        DelayExecuteText((TextContext)layerTag.Value, brewObjectGenerator);
        //    }
        //}
    }

    private static void DelayExecuteText(TextContext textContext, StoryboardObjectGenerator storyboardObjectGenerator)
    {
        var text = textContext.Text.Where(k => k >= 32 && k != 127).ToArray();
        var textOptions = textContext.TextOptions;
        var layer = textContext.Layer;
        var spriteGroup = textContext.SpriteGroup;
        var startTime = textContext.StartTime;
        var unitOrigin = textContext.TextOptions.Origin;

        textContext.BeatmapsetDir = storyboardObjectGenerator.MapsetPath;
        textContext.CachePath = storyboardObjectGenerator.GetProjectCachePath();
        var dict = TextHelper.ProcessText(textContext);

        var sprites = spriteGroup.ToArray();
        var totalW = textOptions.Orientation switch
        {
            Orientation.Horizontal => text.Select(k => dict[k].X).Sum(),
            Orientation.Vertical when textOptions.RotateBy90 => text.Select(k => dict[k].X).Sum(),
            _ => text.Select(k => dict[k].Y).Sum()
        };
        var actualW = textOptions.Orientation switch
        {
            Orientation.Horizontal => (totalW + (text.Length - 1) * textOptions.WordGap) * textOptions.XScale,
            _ => (totalW + (text.Length - 1) * textOptions.WordGap) * textOptions.YScale
        };
        var actualH = dict.Values.Max(k => k.Y);

        int j = 0;
        var calOffset = ResetOffset(spriteGroup, textOptions.Orientation, new Vector2D(actualW, actualH));
        if (textOptions.ShowShadow)
            for (var i = 0; i < text.Length; i++, j++)
                RepositionSprite(i);

        calOffset = ResetOffset(spriteGroup, textOptions.Orientation, new Vector2D(actualW, actualH));
        if (textOptions.ShowStroke)
            for (var i = 0; i < text.Length; i++, j++)
                RepositionSprite(i);

        calOffset = ResetOffset(spriteGroup, textOptions.Orientation, new Vector2D(actualW, actualH));
        if (textOptions.ShowBase)
            for (var i = 0; i < text.Length; i++, j++)
                RepositionSprite(i);

        void RepositionSprite(int i)
        {
            var c = text[i];
            var sprite = sprites[j];

            var isVertical = textOptions.Orientation == Orientation.Vertical;
            var useYAdd = isVertical && textOptions.RotateBy90 == false;

            var addition = useYAdd ? dict[c].Y : dict[c].X;
            if (c == ' ')
            {
                j--;
                calOffset.X += (addition + textOptions.WordGap) *
                               (isVertical ? textOptions.YScale : textOptions.XScale);
                return;
            }

            if (isVertical)
            {
                sprite.DefaultY += (float)(calOffset.X + addition / 2);
                sprite.DefaultX += (float)(calOffset.Y + addition / 2);
            }
            else
            {
                sprite.DefaultX += (float)(calOffset.X + addition / 2);
                sprite.DefaultY = (float)calOffset.Y;
                //sprite.DefaultY += calOffset.Y + addition / 2;
            }

            calOffset.X += (addition + textOptions.WordGap) *
                           (isVertical ? textOptions.YScale : textOptions.XScale);
        }
    }

    private static Vector2D ResetOffset(SpriteGroup spriteGroup,
        Orientation orientation,
        Vector2D actualSize)
    {
        var anchor = Anchors.FromOriginType(spriteGroup.Camera2.OriginType);
        if (orientation == Orientation.Horizontal)
            return new Vector2D(spriteGroup.DefaultX - actualSize.X * anchor.X,
                spriteGroup.DefaultY/* - actualSize.Y * anchor.Y*/);

        return new Vector2D(spriteGroup.DefaultX - actualSize.Y * anchor.X,
            spriteGroup.DefaultY - actualSize.X * anchor.Y);
    }
}