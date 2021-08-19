using System.Linq;
using System.Numerics;
using StorybrewCommon.Storyboarding;

namespace Coosu.Storyboard.Storybrew
{
    public static class StorybrewExtension
    {
        public static void ExecuteBrew(this Sprite sprite, StoryboardLayer brewLayer)
        {
            OsbSprite brewObj;
            if (sprite is Animation animation)
                brewObj = brewLayer.CreateAnimation(animation.ImagePath,
                    (int)animation.FrameCount,
                    (int)animation.FrameDelay,
                    BrewConvert.CvtLoopType(animation.LoopType),
                    BrewConvert.CvtOrigin(animation.OriginType),
                    new Vector2((float)animation.DefaultX,
                        (float)animation.DefaultY));
            else
                brewObj = brewLayer.CreateSprite(sprite.ImagePath,
                    BrewConvert.CvtOrigin(sprite.OriginType),
                    new Vector2((float)sprite.DefaultX, (float)sprite.DefaultY)
                );
        }

        public static void ExecuteBrew(this Layer layer, StoryboardLayer brewLayer)
        {
            foreach (var sprite in layer.SceneObjects.Where(k => k is Sprite).Cast<Sprite>())
            {
                sprite.ExecuteBrew(brewLayer);
            }
        }
    }
}
