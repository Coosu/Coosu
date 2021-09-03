using System.Linq;
using System.Windows.Controls;
using Coosu.Shared.Numerics;
using Coosu.Storyboard.Storybrew.Text;
using StorybrewCommon.Scripting;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static partial class StorybrewExtensions
    {
        private static void DelayExecuteContexts(Layer layer, StoryboardObjectGenerator brewObjectGenerator)
        {
            foreach (var layerTag in layer.Tags)
            {
                if (layerTag.Key.StartsWith("text:"))
                {
                    DelayExecuteText((TextContext)layerTag.Value, brewObjectGenerator);
                }
            }
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
            var totalLen = textOptions.Orientation switch
            {
                Orientation.Horizontal => text.Select(k => dict[k].X).Sum(),
                Orientation.Vertical when textOptions.RotateBy90 => text.Select(k => dict[k].X).Sum(),
                _ => text.Select(k => dict[k].Y).Sum()
            };
            var actualLen = textOptions.Orientation switch
            {
                Orientation.Horizontal => (totalLen + (text.Length - 1) * textOptions.WordGap) * textOptions.XScale,
                _ => (totalLen + (text.Length - 1) * textOptions.WordGap) * textOptions.YScale
            }; ;

            Vector2D calOffset;
            int j = 0;
            calOffset = ResetOffset(spriteGroup, actualLen);
            if (textOptions.ShowShadow)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c].X;
                    if (c == ' ')
                    {
                        j--;
                        calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calOffset + width / 2;
                    calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                }

            calOffset = spriteGroup.DefaultX - actualLen / 2;
            if (textOptions.ShowStroke)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c].X;
                    if (c == ' ')
                    {
                        j--;
                        calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calOffset + width / 2;
                    calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                }

            calOffset = spriteGroup.DefaultX - actualLen / 2;
            if (textOptions.ShowBase)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c].X;
                    if (c == ' ')
                    {
                        j--;
                        calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calOffset + width / 2;
                    calOffset += (width + textOptions.WordGap) * textOptions.XScale;
                }
        }

        private static Vector2D ResetOffset(SpriteGroup spriteGroup, Vector2D actualSize)
        {
            var anchor = Anchors.FromOriginType(spriteGroup.Camera2.OriginType);
            return new Vector2D((float)(spriteGroup.DefaultX - actualSize.X * anchor.X),
                (float)(spriteGroup.DefaultY - actualSize.Y * anchor.Y));
        }
    }
}
