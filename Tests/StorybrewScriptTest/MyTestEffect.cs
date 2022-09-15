using Coosu.Storyboard;
using Coosu.Storyboard.Easing;
using StorybrewCommon.Scripting;

namespace StorybrewScriptTest;

public class MyTestEffect : StoryboardObjectGenerator
{
    public override void Generate()
    {
        var layer = new Layer();

        var osu = layer.CreateSprite("SB/lazer.png");
        osu.Scale(EasingType.BackOut, 38264, 38639, 0, 0.3);
        osu.ScaleBy(new PowerEase() { EasingMode = EasingMode.EaseIn, Power = 5 }, 38639, 39014, 1.2);

        var interval = (38311 - 38264) / 2d;
        osu.StartLoop(38639, (int)((39014 - 38639) / (interval * 2)));
        osu.Fade(0, interval, 1);
        osu.Fade(interval, interval * 2, 0.7);



        //var osu2 = layer.CreateSprite("SB/lazer.png");
        //osu2.Scale(EasingType.CircInOut, 86264, 87014, 0.5, 0.205);
        //osu2.Fade(EasingType.CircInOut, 86264, 87014, 0, 1);
        ////osu2.FadeBy(EasingType.CircIn, 86824, 87012, -1);
        ////osu2.ScaleBy(EasingType.CircIn, 86824, 87012, -0.3);
        ////osu2.ScaleBy(new PowerEase() { EasingMode = EasingMode.EaseIn, Power = 5 }, 38639, 39014, 1.2);

        //var vig = layer.CreateSprite("SB/vignette.png");
        //vig.Scale(63012, 75012, 0.444);
        //vig.Fade(63012, 1);
        //vig.Fade(75012, 75199, 1, 0.6);
        //vig.Scale(75012, 86355, 0.444, 0.6);
        layer.ExecuteBrew(this, k => k.ThreadCount = 1);
    }
}