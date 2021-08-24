using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.Storybrew;
using OpenTK;
using StorybrewCommon.Storyboarding;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class StorybrewExtension
    {
        public static void ExecuteBrew(this Layer layer, StoryboardLayer brewLayer, Action<CompressSettings>? configureSettings)
        {
            void EventHandler(object _, ProcessErrorEventArgs e) => throw new Exception(e.Message);

            var compressor = new SpriteCompressor(layer);
            configureSettings?.Invoke(compressor.Settings);

            compressor.ErrorOccured += EventHandler;
            compressor.CompressAsync().Wait();
            compressor.ErrorOccured -= EventHandler;
            if (layer.SceneObjects.Count == 0) return;

            foreach (var sprite in layer.SceneObjects.Where(k => k is Sprite).Cast<Sprite>())
            {
                InnerExecuteBrew(sprite, brewLayer, false, configureSettings);
            }
        }

        public static void ExecuteBrew(this Sprite sprite, StoryboardLayer brewLayer, Action<CompressSettings>? configureSettings)
        {
            InnerExecuteBrew(sprite, brewLayer, true, configureSettings);
        }

        private static void InnerExecuteBrew(Sprite sprite, StoryboardLayer brewLayer,
            bool optimize, Action<CompressSettings>? configureSettings)
        {
            if (optimize)
            {
                void EventHandler(object _, ProcessErrorEventArgs e) => throw new Exception(e.Message);

                var sceneObjects = new List<ISceneObject> { sprite };
                var compressor = new SpriteCompressor(sceneObjects);
                configureSettings?.Invoke(compressor.Settings);

                compressor.ErrorOccured += EventHandler;
                compressor.CompressAsync().Wait();
                compressor.ErrorOccured -= EventHandler;
                if (sceneObjects.Count == 0) return;
            }

            OsbSprite brewObj;
            if (sprite is Animation animation)
                brewObj = brewLayer.CreateAnimation(animation.ImagePath,
                    (int)animation.FrameCount,
                    (int)animation.FrameDelay,
                    StorybrewInteropHelper.ConvertLoopType(animation.LoopType),
                    StorybrewInteropHelper.ConvertOrigin(animation.OriginType),
                    new Vector2((float)animation.DefaultX,
                        (float)animation.DefaultY));
            else
                brewObj = brewLayer.CreateSprite(sprite.ImagePath,
                    StorybrewInteropHelper.ConvertOrigin(sprite.OriginType),
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
                StorybrewInteropHelper.ExecuteEvent(commonEvent, brewObj);
        }
    }
}