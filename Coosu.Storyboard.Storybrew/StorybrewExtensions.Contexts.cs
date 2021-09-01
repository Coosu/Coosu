using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Storyboard.Advanced;
using Coosu.Storyboard.Advanced.Text;
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
            var text = textContext.Text;
            var textOptions = textContext.TextOptions;
            var layer = textContext.Layer;
            var origin = textContext.Origin;
            var spriteGroup = textContext.SpriteGroup;
            var startTime = textContext.StartTime;

            textContext.BeatmapsetDir = storyboardObjectGenerator.MapsetPath;
            textContext.CachePath = storyboardObjectGenerator.GetProjectCachePath();
            var dict = TextHelper.ProcessText(textContext);
            var totalCalculateWidth = 0;
            var totalCalculateHeight = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var unicode = TextHelper.CharToUnicode(c);
                var fileName = unicode + "_" + textOptions.FileIdentifier;
                var path = Path.Combine(Directories.CoosuTextDir, fileName + ".png");

                var x = 0;
                var y = 0;
                spriteGroup.CreateSprite(path, layer, origin, x, y);
            }
        }
    }
}
