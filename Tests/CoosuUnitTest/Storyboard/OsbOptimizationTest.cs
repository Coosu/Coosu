using System;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Optimizing;
using Xunit;

namespace CoosuUnitTest.Storyboard;

public class OsbOptimizationTest
{
    [Fact]
    public async Task CompressAsync_CombinesTwoStaticFadesWithoutChangingSpriteLifetime()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             F,0,0,,1
             F,0,500,,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,1000,320,240,400,240
             F,0,500,,1,0
            """);
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             F,0,5,,1
             F,0,500,,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,1000,320,240,400,240
             F,0,500,,1,0
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotCombineStaticFadeThatDefinesSpriteLifetime()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             F,0,-5,,1
             F,0,500,,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-5,,1
             M,0,0,1000,320,240,400,240
             F,0,500,,0
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotApplyStaticFadeOptimizationToOtherCommandTypes()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             S,0,0,,1.5
             S,0,500,,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             S,0,0,,1.5
             M,0,0,1000,320,240,400,240
             S,0,500,,2
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotCombineAChainOfStaticFades()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1500,320,240,400,240
             F,0,0,,1
             F,0,500,,0
             F,0,1000,,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,0,,1
             M,0,0,1500,320,240,400,240
             F,0,500,,0
             F,0,1000,,1
            """);
    }

    [Fact]
    public async Task CompressAsync_PreservesDifferentValuesOfZeroDurationCommand()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             F,0,500,500,1,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,1000,320,240,400,240
             F,0,500,,1,0
            """);
    }

    [Fact]
    public async Task CompressAsync_NormalizesImagePathAndRoundsCommandValues()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"folder/image.png",320,240
             M,0,0,1000,320.123456,240.987654,400.123456,300.987654
            """,
            """
            Sprite,Foreground,Centre,"folder\image.png",320,240
             M,0,0,1000,320.123,240.988,400.123,300.988
            """,
            options => options.RoundingDecimalPlaces = 3);
    }

    [Fact]
    public async Task CompressAsync_CombinesAdjacentStaticCommandsWithTheSameValue()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,-100,2100,320,240,400,240
             F,0,0,1000,0.5
             F,0,1000,2000,0.5
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,-100,2100,320,240,400,240
             F,0,0,2000,0.5
            """);
    }

    [Fact]
    public async Task CompressAsync_RemovesAStaticCommandAlreadyRepresentedByPreviousTail()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,2000,320,240,400,240
             F,0,0,1000,0,1
             F,0,1000,1500,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,0,1000,0,1
             M,0,0,2000,320,240,400,240
            """);
    }

    [Fact]
    public async Task CompressAsync_RemovesInstantCommandOverwrittenAtTheSameTime()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,2000,320,240,400,240
             F,0,0,,1
             F,0,0,1000,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,0,1000,0,1
             M,0,0,2000,320,240,400,240
            """);
    }

    [Fact]
    public async Task CompressAsync_RemovesStaticDefaultCommandInsideSpriteLifetime()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,2000,320,240,400,240
             S,0,500,1000,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,2000,320,240,400,240
            """);
    }

    [Fact]
    public async Task CompressAsync_MovesStaticIntegerPositionIntoSpriteHeader()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,2000,0.5
             M,0,500,1000,100,200
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",100,200
             F,0,0,2000,0.5
            """);
    }

    [Fact]
    public async Task CompressAsync_ResetsHeaderPositionWhenAnimatedMoveMustRemain()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,1000,320,240,400,240
             F,0,0,1500,0.5
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,1000,320,240,400,240
             F,0,0,1500,0.5
            """);
    }

    [Fact]
    public async Task CompressAsync_RemovesCommandsHiddenByFadeOut()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,1000,1,0
             M,0,1100,1200,100,100,200,200
             M,0,1300,1400,200,200,300,300
             R,0,1000,2000,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",200,200
             F,0,0,1000,1,0
             R,0,1000,2000,0
            """);
    }

    [Fact]
    public async Task CompressAsync_ConvertsLargeAbsoluteTimingsToSingleLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,1000000,1000100,320,240,400,240
             F,0,1000100,1000200,0.5,0.8
             S,0,1000200,1000300,1,2
             R,0,1000300,1000400,0,1
             C,0,1000400,1000500,255,255,255,128,64,32
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             L,1000000,1
              M,0,0,100,320,240,400,240
              F,0,100,200,0.5,0.8
              S,0,200,300,1,2
              R,0,300,400,0,1
              C,0,400,500,255,255,255,128,64,32
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_RoundsCommandsInsideLoopsAndTriggers()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,5000,0.5
             L,1000,2
              M,0,0,500,320.12345,240.98765,400.12345,300.98765
             T,HitSound,2000,3000
              S,0,0,500,1.23456,2.34567
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,2
              M,0,0,500,320.12,240.99,400.12,300.99
             F,0,0,5000,0.5
             T,HitSound,2000,3000
              S,0,0,500,1.23,2.35
            """,
            options => options.RoundingDecimalPlaces = 2);
    }

    [Fact]
    public async Task CompressAsync_NormalizesEasingWhenNoInterpolationOccurs()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,34,0,2000,320,240,400,240
             S,34,100,500,2
             R,34,1000,1000,0,1
             L,1200,2
              C,34,0,200,128,64,32
             T,HitSound,1500,1800
              S,34,0,200,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1200,2
              C,0,0,200,128,64,32
             M,34,0,2000,320,240,400,240
             S,0,100,500,2
             R,0,1000,,0,1
             T,HitSound,1500,1800
              S,0,0,200,2
            """);
    }

    [Fact]
    public async Task CompressAsync_MergesOnlyTouchingOrOverlappingParametersOfTheSameType()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,0,2000,320,240,400,240
             P,0,100,500,A
             P,0,500,1000,A
             P,0,800,1200,A
             P,0,200,300,H
             P,0,400,600,H
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,0,2000,320,240,400,240
             P,0,100,1200,A
             P,0,200,300,H
             P,0,400,600,H
            """);
    }

    [Fact]
    public async Task CompressAsync_MergesParametersIndependentlyInsideLoopsAndTriggers()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,5000,0.5
             L,1000,2
              P,0,0,100,A
              P,0,100,200,A
             T,HitSound,2000,3000
              P,0,0,50,H
              P,0,40,100,H
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,2
              P,0,0,200,A
             F,0,0,5000,0.5
             T,HitSound,2000,3000
              P,0,0,100,H
            """);
    }

    [Fact]
    public async Task CompressAsync_OmitsOnlyTheDefaultAnimationLoopType()
    {
        await AssertOptimizesToAsync(
            """
            Animation,Foreground,Centre,"forever.png",320,240,4,50,LoopForever
             F,0,0,1000,0,1
            Animation,Foreground,Centre,"once.png",320,240,4,50,LoopOnce
             F,0,2000,3000,0,1
            """,
            """
            Animation,Foreground,Centre,"forever.png",320,240,4,50
             F,0,0,1000,0,1
            Animation,Foreground,Centre,"once.png",320,240,4,50,LoopOnce
             F,0,2000,3000,0,1
            """);
    }

    [Fact]
    public async Task CompressAsync_UsesShorthandForSequentialCommandsWithTheSameDuration()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             M,0,-100,3100,320,240,400,240
             F,0,0,1000,0.2,0.4
             F,0,1000,2000,0.4,0.6
             F,0,2000,3000,0.6,0.8
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             M,0,-100,3100,320,240,400,240
             F,0,0,1000,0.2,0.4,0.6,0.8
            """);
    }

    [Fact]
    public async Task CompressAsync_ParsesAndWritesMultiValueMoveShorthand()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1200,0.5
             M,1,0,500,0,0,100,100,200,50
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,1200,0.5
             M,1,0,500,0,0,100,100,200,50
            """);
    }

    [Fact]
    public async Task CompressAsync_UsesSequentialCommandShorthandInsideLoopsAndTriggers()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,5000,0.5
             L,1000,2
              S,0,0,100,1,2
              S,0,100,200,2,3
             T,HitSound,2000,3000
              R,0,0,100,0,1
              R,0,100,200,1,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,2
              S,0,0,100,1,2,3
             F,0,0,5000,0.5
             T,HitSound,2000,3000
              R,0,0,100,0,1,2
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotUseSequentialCommandShorthandWhenConditionsDiffer()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"duration.png",320,240
             M,0,-100,400,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,100,250,0.4,0.6
            Sprite,Foreground,Centre,"easing.png",320,240
             M,0,-100,300,320,240,400,240
             F,1,0,100,0.2,0.4
             F,2,100,200,0.4,0.6
            Sprite,Foreground,Centre,"gap.png",320,240
             M,0,-100,350,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,150,250,0.4,0.6
            Sprite,Foreground,Centre,"value.png",320,240
             M,0,-100,300,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,100,200,0.5,0.6
            """,
            """
            Sprite,Foreground,Centre,"duration.png",0,0
             M,0,-100,400,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,100,250,0.4,0.6
            Sprite,Foreground,Centre,"easing.png",0,0
             M,0,-100,300,320,240,400,240
             F,1,0,100,0.2,0.4
             F,2,100,200,0.4,0.6
            Sprite,Foreground,Centre,"gap.png",0,0
             M,0,-100,350,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,150,250,0.4,0.6
            Sprite,Foreground,Centre,"value.png",0,0
             M,0,-100,300,320,240,400,240
             F,0,0,100,0.2,0.4
             F,0,100,200,0.5,0.6
            """);
    }

    [Fact]
    public async Task SerializeAsync_DoesNotEnableSequentialCommandShorthandByDefault()
    {
        await AssertSerializesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,1000,0.2,0.4
             F,0,1000,2000,0.4,0.6
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,0,1000,0.2,0.4
             F,0,1000,2000,0.4,0.6
            """);
    }

    [Fact]
    public async Task CompressAsync_RemovesZeroDurationSpriteButKeepsOneDuplicateBackground()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"instant.png",320,240
             F,0,1000,,1
            Sprite,Background,Centre,"bg.jpg",320,240
            Sprite,Background,Centre,"bg.jpg",320,240
            Sprite,Foreground,Centre,"effective.png",320,240
             F,0,0,1000,0,1
            """,
            """
            Sprite,Background,Centre,"bg.jpg",320,240
            Sprite,Foreground,Centre,"effective.png",320,240
             F,0,0,1000,0,1
            """);
    }

    private static async Task AssertOptimizesToAsync(
        string inputEventText,
        string expectedEventText,
        Action<CompressOptions>? configure = null)
    {
        var osb = $"""
                   [Events]
                   //Storyboard Layer 0 (Background)
                   {inputEventText}
                   //Storyboard Sound Samples
                   """;
        var layer = Layer.ParseFromText(osb);
        using var compressor = new SpriteCompressor(layer, options =>
        {
            options.ThreadCount = 1;
            options.ConvertToLoop = false;
            configure?.Invoke(options);
        });

        Assert.True(await compressor.CompressAsync());

        var actualEventText = await layer.ToScriptStringAsync();
        Assert.Equal(NormalizeLineEndings(expectedEventText), NormalizeLineEndings(actualEventText));
    }

    private static async Task AssertSerializesToAsync(string inputEventText, string expectedEventText)
    {
        var osb = $"""
                   [Events]
                   //Storyboard Layer 0 (Background)
                   {inputEventText}
                   //Storyboard Sound Samples
                   """;
        var layer = Layer.ParseFromText(osb);

        var actualEventText = await layer.ToScriptStringAsync();
        Assert.Equal(NormalizeLineEndings(expectedEventText), NormalizeLineEndings(actualEventText));
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n").Trim();
}
