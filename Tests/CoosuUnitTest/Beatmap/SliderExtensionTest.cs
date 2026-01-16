using System.Collections.Generic;
using System.Numerics;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;
using Xunit;

namespace CoosuUnitTest.Beatmap;

public class SliderExtensionTest
{
    [Fact]
    public void TestSliderExtension()
    {
        var rawObj = new RawHitObject { X = 0, Y = 0, Offset = 0, RawType = RawObjectType.Slider };

        // 1. Setup SliderInfo
        var sliderInfo = new SliderInfo(rawObj)
        {
            SliderType = SliderType.Linear,
            StartPoint = new Vector3(0, 0, 0),
            ControlPoints = new List<Vector3> { new Vector3(100, 0, 0) },
            PixelLength = 200, // Desired length > Actual length (100)
            Repeat = 1,
            StartTime = 0
        };
        
        // 2. Wrap in ExtendedSliderInfo
        var extended = new ExtendedSliderInfo(sliderInfo, rawObj);

        // 3. Compute values
        // UpdateComputedValues(double lastRedFactor, double lastLineMultiple, double diffSliderMultiplier, float diffTickRate)
        // lastRedFactor = 1000ms.
        // CurrentSingleDuration = PixelLength / (100 * CurrentSliderMultiplier) * lastRedFactor
        // 200 / 100 * 1000 = 2000ms.
        
        extended.UpdateComputedValues(1000, 1, 1, 10); // TickRate 10 -> Interval 100ms
        
        // 4. Get Slider Ticks
        var ticks = extended.GetSliderTicks();
        
        // 5. Verify
        // We expect ticks up to 1900ms.
        // Tick at 1500ms should be at X=150.
        
        bool foundExtendedTick = false;
        foreach(var tick in ticks)
        {
            if (System.Math.Abs(tick.Offset - 1500) < 0.1)
            {
                // Expected point (150, 0, 0)
                // If extension is not working, it would be (100, 0, 0)
                Assert.Equal(150, tick.Point.X, 0.1);
                foundExtendedTick = true;
            }
        }
        
        Assert.True(foundExtendedTick, "Tick at 1500ms not found");
    }
}
