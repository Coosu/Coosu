using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Storyboard.Advanced;
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
            var totalWidth = text.Select(k => dict[k]).Sum();
            var actualWidth = (totalWidth + (text.Length - 1) * textOptions.WordGap) * textOptions.XScale;

            var calculateX = spriteGroup.DefaultX - actualWidth / 2;
            int j = 0;
            if (textOptions.ShowBase)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c];
                    if (c == ' ')
                    {
                        calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calculateX + width / 2;
                    calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                }

            calculateX = spriteGroup.DefaultX - actualWidth / 2;
            if (textOptions.ShowStroke)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c] * textOptions.XScale;
                    if (c == ' ')
                    {
                        calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calculateX + width / 2;
                    calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                }

            calculateX = spriteGroup.DefaultX - actualWidth / 2;
            if (textOptions.ShowShadow)
                for (var i = 0; i < text.Length; i++, j++)
                {
                    var c = text[i];
                    var sprite = sprites[j];
                    var width = dict[c] * textOptions.XScale;
                    if (c == ' ')
                    {
                        calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                        continue;
                    }

                    sprite.DefaultY += spriteGroup.DefaultY;
                    sprite.DefaultX += calculateX + width / 2;
                    calculateX += (width + textOptions.WordGap) * textOptions.XScale;
                }
        }
    }
}
