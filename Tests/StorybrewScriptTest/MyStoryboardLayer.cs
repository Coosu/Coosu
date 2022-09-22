using OpenTK.Mathematics;
using StorybrewCommon.Storyboarding;
using StorybrewEditor.Storyboarding;

namespace StorybrewScriptTest;

internal class MyStoryboardLayer : StoryboardLayer
{
    public MyStoryboardLayer(string identifier) : base(identifier)
    {
    }

    public override OsbSprite CreateSprite(string path, OsbOrigin origin, Vector2 initialPosition)
    {
        var storyboardObject = new EditorOsbSprite()
        {
            TexturePath = path,
            Origin = origin,
            InitialPosition = initialPosition,
        };
        return storyboardObject;
    }

    public override OsbSprite CreateSprite(string path, OsbOrigin origin)
        => CreateSprite(path, origin, OsbSprite.DefaultPosition);

    public override OsbAnimation CreateAnimation(string path, int frameCount, double frameDelay, OsbLoopType loopType, OsbOrigin origin,
        Vector2 initialPosition)
    {
        var storyboardObject = new EditorOsbAnimation
        {
            TexturePath = path,
            Origin = origin,
            FrameCount = frameCount,
            FrameDelay = frameDelay,
            LoopType = loopType,
            InitialPosition = initialPosition,
        };
        return storyboardObject;
    }

    public override OsbAnimation CreateAnimation(string path, int frameCount, double frameDelay, OsbLoopType loopType, OsbOrigin origin = OsbOrigin.Centre)
        => CreateAnimation(path, frameCount, frameDelay, loopType, origin, OsbSprite.DefaultPosition);

    public override OsbSample CreateSample(string path, double time, double volume = 100)
    {
        var storyboardObject = new EditorOsbSample()
        {
            AudioPath = path,
            Time = time,
            Volume = volume,
        };
        return storyboardObject;
    }

    public override void Discard(StoryboardObject storyboardObject)
    {

    }
}