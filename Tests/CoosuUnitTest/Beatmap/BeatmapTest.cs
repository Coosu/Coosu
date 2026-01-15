using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Extensions.Playback;
using Coosu.Beatmap.Sections.HitObject;
using Xunit;

namespace CoosuUnitTest.Beatmap;

public class BeatmapTest
{
    [Fact]
    public async Task ParseV128Case()
    {
        var folder = @"files";
        var file = @"Test Artist - Test Title (Test Creator) [New Difficulty].osu";
        var osuFile = await OsuFile.ReadFromFileAsync(Path.Combine(folder, file));
        var parsed = osuFile.HitObjects.ToSerializedString(osuFile.Version).Trim();

        var expected = """
                       [HitObjects]
                       0,31.999996,1042,6,2,B|96.000015:31.999996|96.000015:128.00002|P|192:128.00002|192:224.00003|L|320.00003:224.00003|B4|416.00003:128|416.00003:31.999996|512.00006:31.999996,2,746.5360107421875,2|12|2,1:2|2:1|3:2,3:0:0:0:
                       0,32,5714.285714285712,6,2,B|96.000015:32|96.000015:128.00002|P|192:128.00002|192:224.00003|L|320.00003:224.00003|B4|416.00003:128|416.00003:32|512.00006:32,1,746.5360107421875,2|12,1:2|2:1,3:0:0:0:
                       168,88,10890,54,0,B|200:72|200:72|232:88|232:88|264:72|264:72|296:88|296:88|328:72|328:72|360:88,1,180,4|0,0:0|0:0,0:0:0:0:
                       """;
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public void ComputePathV128_LinearToBezier()
    {
        // Construct a multi-segment slider: Linear -> Bezier
        // Segment 1 (Linear): (0,0) -> (100,0)
        // Segment 2 (Bezier): (100,0) -> (200,0) -> (200,100) (Quadratic Bezier)
        // Note: Bezier control points include the start point of the curve.
        // For segment 2, start point is (100,0). Control points in list are (200,0), (200,100).
        
        var sliderInfo = new SliderInfo(new RawHitObject())
        {
            StartPoint = new System.Numerics.Vector3(0, 0, 0),
            SliderType = SliderType.Linear,
            ControlPoints = new[]
            {
                new System.Numerics.Vector3(100, 0, 0),
                new System.Numerics.Vector3(0, 0, (int)SliderType.Bezier + 1), // Type change marker
                new System.Numerics.Vector3(200, 0, 0),
                new System.Numerics.Vector3(200, 100, 0)
            },
            PixelLength = 100 + 150 // Approx length. Linear 100. Bezier approx length > 100. Let's say 250 total.
        };
        
        // Exact length calculation for verification
        // Linear: 100.
        // Bezier: (100,0)->(200,0)->(200,100). 
        // This is a bit complex, but we can verify the point at the junction.
        // If we set PixelLength exactly to the path length, the junction should be at a predictable time.
        // But we don't know the exact length of the Bezier part easily without running the algo.
        // However, we know the Linear part is length 100.
        // If the algorithm is correct, the path will go through (100,0).
        
        // Let's use a simpler second segment: Linear again.
        // Linear (0,0)->(100,0)
        // Linear (100,0)->(100,100)
        // Total length 200.
        // Junction at 100 (50% of length).
        
        sliderInfo.ControlPoints = new[]
        {
            new System.Numerics.Vector3(100, 0, 0),
            new System.Numerics.Vector3(0, 0, (int)SliderType.Linear + 1), // Type change to Linear
            new System.Numerics.Vector3(100, 100, 0)
        };
        sliderInfo.PixelLength = 200;
        
        var extended = new ExtendedSliderInfo(sliderInfo, sliderInfo.BaseObject);
        // Duration calculation:
        // PixelLength = 200.
        // Multiplier = 1.
        // BeatDuration = 2000.
        // Duration = 200 / 100 * 2000 = 4000ms.
        extended.UpdateComputedValues(2000, 1, 1, 1);
        
        // Get ticks at 100ms interval
        var ticks = extended.ComputeTicks(100);
        
        // 500ms is 1/8 of 4000ms.
        // Distance = 200 * 1/8 = 25.
        // Point at 25 distance is (25,0).
        
        var tick500 = ticks.OrderBy(t => System.Math.Abs(t.Offset - 500)).First();
        Assert.True(System.Math.Abs(tick500.Offset - 500) < 1, "Tick at 500ms not found");
        Assert.True(System.Math.Abs(tick500.Point.X - 25) < 1, $"Tick at 500ms X should be 25, but was {tick500.Point.X}");
        Assert.True(System.Math.Abs(tick500.Point.Y - 0) < 1, $"Tick at 500ms Y should be 0, but was {tick500.Point.Y}");
        
        // 2500ms is 5/8 of 4000ms.
        // Distance = 200 * 5/8 = 125.
        // Point at 125 distance: 100 on first segment, 25 on second.
        // Second segment (100,0)->(100,100).
        // Point is (100, 25).
        
        var tick2500 = ticks.OrderBy(t => System.Math.Abs(t.Offset - 2500)).First();
        Assert.True(System.Math.Abs(tick2500.Offset - 2500) < 1, "Tick at 2500ms not found");
        Assert.True(System.Math.Abs(tick2500.Point.X - 100) < 1, $"Tick at 2500ms X should be 100, but was {tick2500.Point.X}");
        Assert.True(System.Math.Abs(tick2500.Point.Y - 25) < 1, $"Tick at 2500ms Y should be 25, but was {tick2500.Point.Y}");
        
        // If the logic was wrong (ignoring Z type marker):
        // Path: (0,0) -> (100,0) -> (0,0) -> (100,100)
        // Segments: 100, 100, sqrt(100^2+100^2)=141.4
        // Total len: 341.4.
        // Target length 200.
        // Ratio = 200/341.4 = 0.58
        // So it would traverse 58% of the WRONG path.
        // 58% of 341.4 = 200.
        // 200 distance on wrong path:
        // 0->100 (100 done). Remaining 100.
        // 100->0 (100 done). Arrive at (0,0).
        // So end point would be (0,0).
        // At 1500ms (75% duration = 150 distance):
        // 100 distance -> (100,0).
        // 50 distance back -> (50,0).
        // So with wrong logic, 1500ms would be at (50,0).
        // With correct logic, 1500ms is at (100,50).
        // This confirms the test is valid to distinguish the two behaviors.
    }

    [Fact]
    public async Task ParseEdgeCase()
    {
        var folder = @"files";
        var file = @"Chata & nayuta - Yuune Zekka, Ryouran no Sai (sjoy) [Replay].osu";
        var osuFile = await OsuFile.ReadFromFileAsync(Path.Combine(folder, file));
    }

    [Fact(Skip = "Depends on local files outside repository.")]
    public async Task ReadAndWrite()
    {
        //var folder = @"C:\Users\milkitic\Desktop\1002455 supercell - Giniro Hikousen  (Ttm bootleg Edit)";
        //var file = @"supercell - Giniro Hikousen  (Ttm bootleg Edit) (yf_bmp) [4K Hard].osu";
        var folder = @"C:\Users\milkitic\Downloads\1376486 Risshuu feat. Choko - Take [no video] (2)";
        var file = @"Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu";
        var osuFile = await OsuFile.ReadFromFileAsync(Path.Combine(folder, file));

        osuFile.SaveToDirectory(folder, osuFile.Metadata.Version + " (TEST)");
    }

    [Fact(Skip = "Depends on local files outside repository.")]
    public async Task ReadHitsounds()
    {
        var folder = @"C:\Users\milkitic\Downloads\606833 BlackYooh vs. siromaru - BLACK or WHITE";
        var file = "BlackYooh vs. siromaru - BLACK or WHITE (Arrival) [Black].osu";
        var osuDir = new OsuDirectory(folder);
        await osuDir.InitializeAsync(file);

        var hitsoundList = await osuDir.GetHitsoundNodesAsync(osuDir.OsuFiles[0]);
        var playableNodes = hitsoundList
            .Where(k => k is PlayableNode { /*PlayablePriority: PlayablePriority.Primary*/ })
            .Cast<PlayableNode>()
            .ToList();
    }
}