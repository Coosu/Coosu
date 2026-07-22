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
    public async Task CompressAsync_DetectsTheShortestRepeatedMultiCommandLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,1000,1500,1,2
             R,0,1500,2000,0,1
             S,0,2000,2500,1,2
             R,0,2500,3000,0,1
             S,0,3000,3500,1,2
             R,0,3500,4000,0,1
             S,0,4000,4500,1,2
             R,0,4500,5000,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,4
              S,0,0,500,1,2
              R,0,500,1000,0,1
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_DetectsARepeatHiddenBySequentialShorthand()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             R,0,1000,1500,0,1
             R,0,1500,2000,1,0
             R,0,2000,2500,0,1
             R,0,2500,3000,1,0
             R,0,3000,3500,0,1
             R,0,3500,4000,1,0
             R,0,4000,4500,0,1
             R,0,4500,5000,1,0
             R,0,5000,5500,0,1
             R,0,5500,6000,1,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,5
              R,0,0,500,0,1,0
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_KeepsSequentialShorthandWhenItIsShorterThanLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             R,0,0,500,0,1
             R,0,500,1000,1,0
             R,0,1000,1500,0,1
             R,0,1500,2000,1,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             R,0,0,500,0,1,0,1,0
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_LeavesNonRepeatingPrefixAndSuffixOutsideLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             C,0,0,500,255,0,0
             S,0,1000,1500,1,2
             R,0,1500,2000,0,1
             S,0,2000,2500,1,2
             R,0,2500,3000,0,1
             S,0,3000,3500,1,2
             R,0,3500,4000,0,1
             C,0,4500,5000,0,255,0
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,3
              S,0,0,500,1,2
              R,0,500,1000,0,1
             C,0,0,500,255,0,0
             C,0,4500,5000,0,255,0
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_DetectsRepeatedTypesAmongUnrelatedInterleavedCommands()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,1000,1500,1,2
             C,0,1250,1350,255,0,0,0,255,0
             R,0,1500,2000,0,1
             S,0,2000,2500,1,2
             C,0,2300,2400,0,255,0,0,0,255
             R,0,2500,3000,0,1
             S,0,3000,3500,1,2
             C,0,3600,3700,0,0,255,255,0,0
             R,0,3500,4000,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,3
              S,0,0,500,1,2
              R,0,500,1000,0,1
             C,0,1250,1350,255,0,0,0,255,0
             C,0,2300,2400,0,255,0,0,0,255
             C,0,3600,3700,0,0,255,255,0,0
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_UsesTheMaximalRepeatedPrefixAndKeepsDifferentTail()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,2
             R,0,450,600,0,1
             S,0,600,750,1,2
             R,0,750,900,0,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,0,2
              S,0,0,150,1,2
              R,0,150,300,0,1
             S,0,600,750,1,2
             R,0,750,900,0,2
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_CreatesMultipleDisjointLoopsAcrossOptimizationRounds()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,2
             R,0,450,600,0,1
             S,0,600,750,1,2
             R,0,750,900,0,1
             S,0,900,1050,1,2
             R,0,1050,1200,0,1
             S,0,1200,1350,3,4
             R,0,1350,1500,2,3
             S,0,2000,2150,5,6
             R,0,2150,2300,4,5
             S,0,2300,2450,5,6
             R,0,2450,2600,4,5
             S,0,2600,2750,5,6
             R,0,2750,2900,4,5
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,0,4
              S,0,0,150,1,2
              R,0,150,300,0,1
             L,2000,3
              S,0,0,150,5,6
              R,0,150,300,4,5
             S,0,1200,1350,3,4
             R,0,1350,1500,2,3
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_DoesNotTreatDifferentValuesAsARepeatedLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,3
             R,0,450,600,0,2
             S,0,600,750,1,4
             R,0,750,900,0,3
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,3
             R,0,450,600,0,2
             S,0,600,750,1,4
             R,0,750,900,0,3
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_DoesNotCreateLoopWithTheWrongIterationDuration()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             R,0,0,100,0,1
             R,0,1000,1100,0,1
             R,0,2000,2100,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             R,0,0,100,0,1
             R,0,1000,1100,0,1
             R,0,2000,2100,0,1
            """,
            options => options.ConvertToLoop = true);
    }

    [Fact]
    public async Task CompressAsync_DoesNotMoveTriggerCommandTypesIntoLoop()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,2
             R,0,450,600,0,1
             S,0,600,750,1,2
             R,0,750,900,0,1
             T,HitSound,0,900
              S,0,0,150,1,2
              R,0,150,300,0,1
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             S,0,0,150,1,2
             R,0,150,300,0,1
             S,0,300,450,1,2
             R,0,450,600,0,1
             S,0,600,750,1,2
             R,0,750,900,0,1
             T,HitSound,0,900
              S,0,0,150,1,2
              R,0,150,300,0,1
            """,
            options => options.ConvertToLoop = true);
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
    public async Task CompressAsync_ConvertsUniformVectorsToScaleAcrossTheWholeSprite()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,5000,0.5
             V,0,0,500,1,1,2,2
             L,1000,2
              V,0,0,500,2,2,3,3
             T,HitSound,3000,4000
              V,0,0,500,3,3,4,4
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,2
              S,0,0,500,2,3
             F,0,-100,5000,0.5
             S,0,0,500,1,2
             T,HitSound,3000,4000
              S,0,0,500,3,4
            """);
    }

    [Fact]
    public async Task CompressAsync_UnifiesExistingScaleAndUniformVectorCommands()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2500,0.5
             S,0,0,1000,1,2
             V,0,1000,2000,2,2,3,3
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2500,0.5
             S,0,0,1000,1,2,3
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotPartiallyConvertVectorCommands()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2500,0.5
             V,0,0,1000,1,1,2,2
             L,1000,2
              V,0,0,500,2,1,3,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             L,1000,2
              V,0,0,500,2,1,3,2
             F,0,-100,2500,0.5
             V,0,0,1000,1,1,2,2
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotRewriteMixedScaleFamilyWithNonUniformVector()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2500,0.5
             S,0,0,1000,1,2
             V,0,1000,2000,2,1,3,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2500,0.5
             S,0,0,1000,1,2
             V,0,1000,2000,2,1,3,2
            """);
    }

    [Fact]
    public async Task CompressAsync_UsesRoundedValuesWhenReducingVectorDimensions()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1500,0.5
             V,0,0,1000,1.23441,1.23449,2.34541,2.34549
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1500,0.5
             S,0,0,1000,1.234,2.345
            """,
            options => options.RoundingDecimalPlaces = 3);
    }

    [Fact]
    public async Task CompressAsync_SplitsMovesWhenTheFinalTextIsShorter()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,5100,0.5
             M,0,0,1000,0,240,100,240
             M,0,1000,2000,100,240,200,240
             M,0,2000,3000,200,240,300,240
             M,0,3000,4000,300,240,400,240
             M,0,4000,5000,400,240,500,240
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,5100,0.5
             MX,0,0,1000,0,100,200,300,400,500
             MY,0,0,5000,240
            """);
    }

    [Fact]
    public async Task CompressAsync_SplitsMovesAlongTheYDimensionSymmetrically()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,5100,0.5
             M,0,0,1000,320,0,320,100
             M,0,1000,2000,320,100,320,200
             M,0,2000,3000,320,200,320,300
             M,0,3000,4000,320,300,320,400
             M,0,4000,5000,320,400,320,500
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,5100,0.5
             MY,0,0,1000,0,100,200,300,400,500
             MX,0,0,5000,320
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotSplitASingleOneDimensionalMove()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1100,0.5
             M,0,0,1000,0,240,100,240
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,1100,0.5
             M,0,0,1000,0,240,100,240
            """);
    }

    [Fact]
    public async Task CompressAsync_KeepsMovesWhenSplitTextIsNotShorter()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,3100,0.5
             M,0,0,1000,0,0,100,100
             M,0,1000,2000,100,100,200,200
             M,0,2000,3000,200,200,300,300
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,3100,0.5
             M,0,0,1000,0,0,100,100,200,200,300,300
            """);
    }

    [Fact]
    public async Task CompressAsync_DoesNotChangeAnAlreadyMixedPositionFamily()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,2100,0.5
             M,0,0,1000,0,240,100,240
             MX,0,1000,2000,100,200
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             F,0,-100,2100,0.5
             M,0,0,1000,0,240,100,240
             MX,0,1000,2000,100,200
            """);
    }

    [Fact]
    public async Task CompressAsync_DetectsPositionFamilyConflictsAcrossLoopsAndTriggers()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,5100,0.5
             M,0,0,1000,0,240,100,240
             M,0,1000,2000,100,240,200,240
             M,0,2000,3000,200,240,300,240
             M,0,3000,4000,300,240,400,240
             M,0,4000,5000,400,240,500,240
             L,1000,2
              MX,0,0,500,0,100
             T,HitSound,3000,4000
              MY,0,0,500,240,300
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             L,1000,2
              MX,0,0,500,0,100
             F,0,-100,5100,0.5
             M,0,0,1000,0,240,100,240,200,240,300,240,400,240,500,240
             T,HitSound,3000,4000
              MY,0,0,500,240,300
            """);
    }

    [Fact]
    public async Task CompressAsync_SplitsMovesAtomicallyAcrossLoopsAndTriggers()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,8100,0.5
             M,0,0,1000,0,123456.789,100,123456.789
             M,0,1000,2000,100,123456.789,200,123456.789
             M,0,2000,3000,200,123456.789,300,123456.789
             M,0,3000,4000,300,123456.789,400,123456.789
             M,0,4000,5000,400,123456.789,500,123456.789
             M,0,5000,6000,500,123456.789,600,123456.789
             M,0,6000,7000,600,123456.789,700,123456.789
             M,0,7000,8000,700,123456.789,800,123456.789
             L,1000,2
              M,0,0,500,0,123456.789,50,123456.789
             T,HitSound,3000,4000
              M,0,0,500,50,123456.789,100,123456.789
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",0,0
             L,1000,2
              MX,0,0,500,0,50
              MY,0,0,500,123456.789
             F,0,-100,8100,0.5
             MX,0,0,1000,0,100,200,300,400,500,600,700,800
             MY,0,0,8000,123456.789
             T,HitSound,3000,4000
              MX,0,0,500,50,100
              MY,0,0,500,123456.789
            """);
    }

    [Fact]
    public async Task CompressAsync_PreservesAnimationHeaderWhenReducingDimensions()
    {
        await AssertOptimizesToAsync(
            """
            Animation,Foreground,Centre,"animation.png",320,240,4,50,LoopOnce
             F,0,-100,1500,0.5
             V,0,0,1000,1,1,2,2
            """,
            """
            Animation,Foreground,Centre,"animation.png",320,240,4,50,LoopOnce
             F,0,-100,1500,0.5
             S,0,0,1000,1,2
            """);
    }

    [Fact]
    public async Task CompressAsync_CanDisableCommandDimensionReduction()
    {
        await AssertOptimizesToAsync(
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1500,0.5
             V,0,0,1000,1,1,2,2
            """,
            """
            Sprite,Foreground,Centre,"sprite.png",320,240
             F,0,-100,1500,0.5
             V,0,0,1000,1,1,2,2
            """,
            options => options.ReduceCommandDimensions = false);
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
