using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.Storybrew;
using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    /// <summary>
    /// Storybrew extension methods for Coosu.
    /// </summary>
    public static class StorybrewExtensions
    {
        /// <summary>
        /// Executing Coosu commands and optimizing automatically in storybrew.
        /// </summary>
        /// <param name="layer">Specific Coosu <see cref="Layer"/>.</param>
        /// <param name="brewObjectGenerator">Specific storybrew <see cref="StoryboardObjectGenerator"/>.</param>
        /// <param name="configureSettings">Configure compressing options.</param>
        public static void ExecuteBrew(this Layer layer, StoryboardObjectGenerator brewObjectGenerator,
            Action<CompressSettings>? configureSettings = null)
        {
            ExecuteBrew(layer, brewObjectGenerator.GetLayer(layer.Name), configureSettings);
        }

        /// <summary>
        /// Executing Coosu commands and optimizing automatically in storybrew.
        /// </summary>
        /// <param name="layer">Specific Coosu <see cref="Layer"/>.</param>
        /// <param name="brewLayer">Specific storybrew <see cref="StoryboardLayer"/>.</param>
        /// <param name="configureSettings">Configure compressing options.</param>
        public static void ExecuteBrew(this Layer layer, StoryboardLayer brewLayer,
            Action<CompressSettings>? configureSettings = null)
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

        /// <summary>
        /// Executing Coosu commands and optimizing automatically in storybrew.
        /// </summary>
        /// <param name="sprite">Specific Coosu <see cref="Sprite"/>.</param>
        /// <param name="brewLayer">Specific storybrew <see cref="StoryboardLayer"/>.</param>
        /// <param name="configureSettings">Configure compressing options.</param>
        public static void ExecuteBrew(this Sprite sprite, StoryboardLayer brewLayer,
            Action<CompressSettings>? configureSettings = null)
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