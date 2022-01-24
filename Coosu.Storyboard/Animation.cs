using System;
using System.IO;
using System.Threading.Tasks;
using Coosu.Shared;

namespace Coosu.Storyboard
{
    public sealed class Animation : Sprite
    {
        public override ObjectType ObjectType { get; } = ObjectTypes.Animation;


        public int FrameCount { get; set; }
        public double FrameDelay { get; set; }
        public LoopType LoopType { get; set; }

        /// <summary>
        /// Create a storyboard element by dynamic images.
        /// </summary>
        /// <param name="layerType">Set sprite layer.</param>
        /// <param name="originType">Set sprite origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        /// <param name="frameCount">Set frame count.</param>
        /// <param name="frameDelay">Set frame rate (frame delay).</param>
        /// <param name="loopType">Set loop type.</param>
        public Animation(LayerType layerType, OriginType originType, string imagePath, double defaultX,
            double defaultY, int frameCount, double frameDelay, LoopType loopType)
            : base(layerType, originType, imagePath, defaultX, defaultY)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }

        /// <summary>
        /// Create a storyboard element by dynamic images.
        /// </summary>
        /// <param name="layer">Set sprite layer.</param>
        /// <param name="origin">Set sprite origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        /// <param name="frameCount">Set frame count.</param>
        /// <param name="frameDelay">Set frame rate (frame delay).</param>
        /// <param name="loopType">Set loop type.</param>
        public Animation(ReadOnlySpan<char> layer, 
            ReadOnlySpan<char> origin,
            ReadOnlySpan<char> imagePath,
            double defaultX, double defaultY, 
            int frameCount, double frameDelay, ReadOnlySpan<char> loopType)
            : base(layer, origin, imagePath, defaultX, defaultY)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType.ToLoopType();
        }

        public override async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync(ObjectType.GetString(ObjectType));
            await writer.WriteAsync(',');
            await writer.WriteAsync(LayerType);
            await writer.WriteAsync(',');
            await writer.WriteAsync(OriginType);
            await writer.WriteAsync(",\"");
            await writer.WriteAsync(ImagePath);
            await writer.WriteAsync("\",");
            await writer.WriteAsync((int)DefaultX);
            await writer.WriteAsync(',');
            await writer.WriteAsync((int)DefaultY);
            await writer.WriteAsync(',');
            await writer.WriteAsync(FrameCount);
            await writer.WriteAsync(',');
            await writer.WriteAsync((int)FrameDelay);
            await writer.WriteAsync(',');
            await writer.WriteAsync(LoopType);
        }
    }
}
