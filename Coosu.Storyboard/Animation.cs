using System;

namespace Coosu.Storyboard
{
    public sealed class Animation : Sprite
    {
        public override ObjectType ObjectType { get; } = ObjectTypes.Animation;

        protected override string Header =>
            $"{ObjectTypeManager.GetString(ObjectType)},{LayerType},{OriginType},\"{ImagePath}\"," +
            $"{DefaultX},{DefaultY},{FrameCount},{FrameDelay},{LoopType}";

        public int FrameCount { get; set; }
        public float FrameDelay { get; set; }
        public LoopType LoopType { get; set; }

        /// <summary>
        /// Create a storyboard element by dynamic images.
        /// </summary>
        /// <param name="type">Set element type.</param>
        /// <param name="layerType">Set element layer.</param>
        /// <param name="originType">Set element origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        /// <param name="frameCount">Set frame count.</param>
        /// <param name="frameDelay">Set frame rate (frame delay).</param>
        /// <param name="loopType">Set loop type.</param>
        public Animation(LayerType layerType, OriginType originType, string imagePath, float defaultX,
            float defaultY, int frameCount, float frameDelay, LoopType loopType)
            : base(layerType, originType, imagePath, defaultX, defaultY)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = loopType;
        }

        /// <summary>
        /// Create a storyboard element by dynamic images.
        /// </summary>
        /// <param name="type">Set element type.</param>
        /// <param name="layer">Set element layer.</param>
        /// <param name="origin">Set element origin.</param>
        /// <param name="imagePath">Set image path.</param>
        /// <param name="defaultX">Set default x-coordinate of location.</param>
        /// <param name="defaultY">Set default x-coordinate of location.</param>
        /// <param name="frameCount">Set frame count.</param>
        /// <param name="frameDelay">Set frame rate (frame delay).</param>
        /// <param name="loopType">Set loop type.</param>
        public Animation(string layer, string origin, string imagePath, float defaultX,
            float defaultY, int frameCount, float frameDelay, string loopType)
            : base(layer, origin, imagePath, defaultX, defaultY)
        {
            FrameCount = frameCount;
            FrameDelay = frameDelay;
            LoopType = (LoopType)Enum.Parse(typeof(LoopType), loopType);
        }
    }
}
