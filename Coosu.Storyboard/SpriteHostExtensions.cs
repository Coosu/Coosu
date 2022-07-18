using System.Numerics;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public static class SpriteHostExtensions
    {
        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath)
        {
            var obj = new Sprite(LayerType.Foreground, OriginType.Centre, filePath, 320, 240);
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath,
            OriginType originType)
        {
            var obj = new Sprite(LayerType.Foreground, originType, filePath, 320, 240);
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath,
            LayerType layerType)
        {
            var obj = new Sprite(layerType, OriginType.Centre, filePath, 320, 240);
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath,
            LayerType layerType,
            OriginType originType)
        {
            var obj = new Sprite(layerType, originType, filePath, 320, 240);
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <param name="defaultLocation">The sprite's default location.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath,
            LayerType layerType,
            OriginType originType,
            Vector2 defaultLocation)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultLocation.X, defaultLocation.Y);
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard sprite.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The sprite's layer.</param>
        /// <param name="originType">The sprite's origin.</param>
        /// <param name="defaultX">The sprite's default x.</param>
        /// <param name="defaultY">The sprite's default y.</param>
        /// <returns>Created sprite.</returns>
        public static Sprite CreateSprite(
            this ISpriteHost spriteHost,
            string filePath,
            LayerType layerType,
            OriginType originType,
            float defaultX, float defaultY)
        {
            var obj = new Sprite(layerType, originType, filePath, defaultX, defaultY);
            spriteHost.AddSprite(obj);
            return obj;
        }
        
        /// <summary>
        /// Create a storyboard animation.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="frameCount">The animation's total frame count.</param>
        /// <param name="frameDelay">The animation's frame delay between each frames.</param>
        /// <param name="loopType">The animation's loop type.</param>
        /// <param name="layerType">The animation's layer.</param>
        /// <param name="originType">The animation's origin.</param>
        /// <param name="defaultX">The animation's default x.</param>
        /// <param name="defaultY">The animation's default y.</param>
        /// <returns>Created animation.</returns>
        public static Animation CreateAnimation(
            this ISpriteHost spriteHost,
            string filePath,
            int frameCount, float frameDelay, LoopType loopType,
            LayerType layerType = LayerType.Foreground,
            OriginType originType = OriginType.Centre,
            int defaultX = 320, int defaultY = 240)
        {
            var obj = new Animation(
                layerType,
                originType,
                filePath,
                defaultX,
                defaultY,
                frameCount,
                frameDelay,
                loopType
            );
            spriteHost.AddSprite(obj);
            return obj;
        }

        /// <summary>
        /// Create a storyboard animation.
        /// </summary>
        /// <param name="filePath">Image file path.</param>
        /// <param name="layerType">The animation's layer.</param>
        /// <param name="originType">The animation's origin.</param>
        /// <param name="defaultX">The animation's default x.</param>
        /// <param name="defaultY">The animation's default y.</param>
        /// <param name="frameCount">The animation's total frame count.</param>
        /// <param name="frameDelay">The animation's frame delay between each frames.</param>
        /// <param name="loopType">The animation's loop type.</param>
        /// <returns>Created animation.</returns>
        public static Animation CreateAnimation(
            this ISpriteHost spriteHost,
            string filePath,
            LayerType layerType,
            OriginType originType,
            int defaultX, int defaultY,
            int frameCount, float frameDelay, LoopType loopType)
        {
            var obj = new Animation(
                layerType,
                originType,
                filePath,
                defaultX,
                defaultY,
                frameCount,
                frameDelay,
                loopType
            );
            spriteHost.AddSprite(obj);
            return obj;
        }
    }
}
