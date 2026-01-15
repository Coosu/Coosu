using System;
using System.Collections.Generic;
using System.Numerics;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;
using Xunit;
using Xunit.Abstractions;

namespace CoosuUnitTest.Beatmap;

public sealed class SliderDiscreteSamplingComparisonTest
{
    private readonly ITestOutputHelper _output;

    public SliderDiscreteSamplingComparisonTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CompareLegacyAndCurrentDiscreteSampling()
    {
        var cases = new[]
        {
            ("linear-repeat3", CreateSlider(SliderType.Linear, new Vector2(64, 192), new[]
            {
                new Vector2(448, 192),
            }, repeat: 3)),
            ("bezier-single", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(320, 64),
                new Vector2(448, 192),
            }, repeat: 1)),
            ("bezier-multi-repeat2", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(256, 64),
                new Vector2(256, 64),
                new Vector2(320, 256),
                new Vector2(448, 192),
            }, repeat: 2)),
            ("perfect-single", CreateSlider(SliderType.Perfect, new Vector2(64, 192), new[]
            {
                new Vector2(256, 64),
                new Vector2(448, 192),
            }, repeat: 1)),
        };

        const double interval = 50;

        foreach (var (name, slider) in cases)
        {
            var current = slider.ComputeDiscreteData(interval);
            var reference = SliderDiscreteSamplingReference.ComputeDiscreteData(slider, interval);

            _output.WriteLine($"case={name}");
            _output.WriteLine($"count current={current.Length} reference={reference.Length}");

            var maxOffsetDelta = 0d;
            var maxDistanceDelta = 0d;
            var mismatchCount = 0;

            var count = Math.Min(current.Length, reference.Length);
            for (var i = 0; i < count; i++)
            {
                var offsetDelta = Math.Abs(current[i].Offset - reference[i].Offset);
                if (offsetDelta > maxOffsetDelta) maxOffsetDelta = offsetDelta;

                var dx = current[i].Point.X - reference[i].Point.X;
                var dy = current[i].Point.Y - reference[i].Point.Y;
                var distDelta = Math.Sqrt(dx * dx + dy * dy);
                if (distDelta > maxDistanceDelta) maxDistanceDelta = distDelta;

                if (offsetDelta > 1e-7 || distDelta > 1e-2)
                {
                    mismatchCount++;
                    _output.WriteLine(
                        $"idx={i} off(cur={current[i].Offset:0.###} ref={reference[i].Offset:0.###} d={offsetDelta:0.########}) " +
                        $"pt(cur={current[i].Point.X:0.###},{current[i].Point.Y:0.###} ref={reference[i].Point.X:0.###},{reference[i].Point.Y:0.###} d={distDelta:0.######})");
                }
            }

            _output.WriteLine($"maxOffsetDelta={maxOffsetDelta:0.########} maxDistanceDelta={maxDistanceDelta:0.######} mismatches={mismatchCount}");

            Assert.Equal(reference.Length, current.Length);
            Assert.True(maxOffsetDelta <= 1e-7, $"Unexpected offset delta in case={name}: {maxOffsetDelta}");
            Assert.True(maxDistanceDelta <= 1.0, $"Unexpected point delta in case={name}: {maxDistanceDelta}");
        }
    }

    [Fact(Skip = "用于观察 current vs legacy 采样差异；需要时取消 Skip")]
    public void ObserveLegacyAndCurrentDiscreteSamplingDiff()
    {
        var cases = new[]
        {
            ("linear-repeat3", CreateSlider(SliderType.Linear, new Vector2(64, 192), new[]
            {
                new Vector2(448, 192),
            }, repeat: 3)),
            ("bezier-single", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(320, 64),
                new Vector2(448, 192),
            }, repeat: 1)),
            ("bezier-multi-repeat2", CreateSlider(SliderType.Bezier, new Vector2(64, 192), new[]
            {
                new Vector2(128, 64),
                new Vector2(256, 64),
                new Vector2(256, 64),
                new Vector2(320, 256),
                new Vector2(448, 192),
            }, repeat: 2)),
            ("perfect-single", CreateSlider(SliderType.Perfect, new Vector2(64, 192), new[]
            {
                new Vector2(256, 64),
                new Vector2(448, 192),
            }, repeat: 1)),
        };

        const double interval = 50;

        foreach (var (name, slider) in cases)
        {
            var current = slider.ComputeDiscreteData(interval);
            var legacy = SliderDiscreteSamplingLegacy.ComputeDiscreteData(slider, interval);

            _output.WriteLine($"case={name}");
            _output.WriteLine($"count current={current.Length} legacy={legacy.Length}");

            var maxOffsetDelta = 0d;
            var maxDistanceDelta = 0d;
            var mismatchCount = 0;

            var count = Math.Min(current.Length, legacy.Length);
            for (var i = 0; i < count; i++)
            {
                var offsetDelta = Math.Abs(current[i].Offset - legacy[i].Offset);
                if (offsetDelta > maxOffsetDelta) maxOffsetDelta = offsetDelta;

                var dx = current[i].Point.X - legacy[i].Point.X;
                var dy = current[i].Point.Y - legacy[i].Point.Y;
                var distDelta = Math.Sqrt(dx * dx + dy * dy);
                if (distDelta > maxDistanceDelta) maxDistanceDelta = distDelta;

                if (offsetDelta > 1e-7 || distDelta > 1e-2)
                {
                    mismatchCount++;
                    if (mismatchCount <= 10)
                    {
                        _output.WriteLine(
                            $"idx={i} off(cur={current[i].Offset:0.###} leg={legacy[i].Offset:0.###} d={offsetDelta:0.########}) " +
                            $"pt(cur={current[i].Point.X:0.###},{current[i].Point.Y:0.###} leg={legacy[i].Point.X:0.###},{legacy[i].Point.Y:0.###} d={distDelta:0.######})");
                    }
                }
            }

            _output.WriteLine($"maxOffsetDelta={maxOffsetDelta:0.########} maxDistanceDelta={maxDistanceDelta:0.######} mismatches={mismatchCount}");

            Assert.Equal(legacy.Length, current.Length);
            Assert.True(maxOffsetDelta <= 1e-7, $"Unexpected offset delta in case={name}: {maxOffsetDelta}");
        }
    }

    private static ExtendedSliderInfo CreateSlider(SliderType sliderType, Vector2 startPoint, IReadOnlyList<Vector2> controlPoints, int repeat)
    {
        var raw = new RawHitObject
        {
            X = (int)startPoint.X,
            Y = (int)startPoint.Y,
            Offset = 1000,
            RawType = RawObjectType.Slider,
            Hitsound = 0,
            SampleSet = 0,
            AdditionSet = 0,
        };

        var slider = new ExtendedSliderInfo(raw)
        {
            SliderType = sliderType,
            StartPoint = startPoint,
            StartTime = raw.Offset,
            ControlPoints = controlPoints,
            Repeat = repeat,
            PixelLength = 400,
        };

        slider.UpdateComputedValues(lastRedFactor: 500, lastLineMultiple: 1, diffSliderMultiplier: 1, diffTickRate: 1);
        return slider;
    }
}
