using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Extensions.Optimizing;
using OpenTK;
using StorybrewCommon.Storyboarding;

namespace Coosu.Storyboard.Storybrew
{
    public static class StorybrewExtension
    {
        public static void ExecuteBrew(this Layer layer, StoryboardLayer brewLayer)
        {
            var compressor = new SpriteCompressor(layer);
            compressor.ErrorOccured = (_, e) => throw new Exception(e.Message);
            compressor.CompressAsync().Wait();
            if (layer.SceneObjects.Count == 0) return;

            foreach (var sprite in layer.SceneObjects.Where(k => k is Sprite).Cast<Sprite>())
            {
                InnerExecuteBrew(sprite, brewLayer, false);
            }
        }

        public static void ExecuteBrew(this Sprite sprite, StoryboardLayer brewLayer)
        {
            InnerExecuteBrew(sprite, brewLayer, true);
        }

        private static void InnerExecuteBrew(Sprite sprite, StoryboardLayer brewLayer, bool optimize)
        {
            if (optimize)
            {
                var sceneObjects = new List<ISceneObject> { sprite };
                var compressor = new SpriteCompressor(sceneObjects);
                compressor.ErrorOccured = (_, e) => throw new Exception(e.Message);
                compressor.CompressAsync().Wait();
                if (sceneObjects.Count == 0) return;
            }

            OsbSprite brewObj;
            if (sprite is Animation animation)
                brewObj = brewLayer.CreateAnimation(animation.ImagePath,
                    (int)animation.FrameCount,
                    (int)animation.FrameDelay,
                    ConvertHelper.ConvertLoopType(animation.LoopType),
                    ConvertHelper.ConvertOrigin(animation.OriginType),
                    new Vector2((float)animation.DefaultX,
                        (float)animation.DefaultY));
            else
                brewObj = brewLayer.CreateSprite(sprite.ImagePath,
                    ConvertHelper.ConvertOrigin(sprite.OriginType),
                    new Vector2((float)sprite.DefaultX, (float)sprite.DefaultY)
                );

            InnerExecuteBrew(sprite, brewObj);
            foreach (var l in sprite.LoopList)
            {
                brewObj.StartLoopGroup(l.StartTime, l.LoopCount);
                InnerExecuteBrew(l, brewObj);
                brewObj.EndGroup();
            }

            foreach (var t in sprite.TriggerList)
            {
                brewObj.StartTriggerGroup(t.TriggerName, t.StartTime, t.EndTime);
                InnerExecuteBrew(t, brewObj);
                brewObj.EndGroup();
            }
        }

        private static void InnerExecuteBrew(IEventHost eventHost, OsbSprite brewObj)
        {
            foreach (var commonEvent in eventHost.Events)
                ConvertHelper.ExecuteEvent(commonEvent, brewObj);
        }
    }
}