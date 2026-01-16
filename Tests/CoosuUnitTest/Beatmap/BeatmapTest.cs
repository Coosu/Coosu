using System.IO;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Xunit;

namespace CoosuUnitTest.Beatmap;

public class BeatmapTest
{
    [Theory()]
    [InlineData("Test Artist - Test Title (Test Creator) [New Difficulty].osu")]
    [InlineData("Tachiiri Kinshi - Zi Fu Ling (Momoyaya) [goldfish].osu")]
    public async Task ParseV128Case(string file)
    {
        var folder = @"files";
        var osuFile = await OsuFile.ReadFromFileAsync(Path.Combine(folder, file));
        var parsed = osuFile.HitObjects.ToSerializedString(osuFile.Version).Trim();

        //var expected = """
        //               [HitObjects]
        //               0,31.999996,1042,6,2,B|96.000015:31.999996|96.000015:128.00002|P|192:128.00002|192:224.00003|L|320.00003:224.00003|B4|416.00003:128|416.00003:31.999996|512.00006:31.999996,2,746.5360107421875,2|12|2,1:2|2:1|3:2,3:0:0:0:
        //               0,32,5714.285714285712,6,2,B|96.000015:32|96.000015:128.00002|P|192:128.00002|192:224.00003|L|320.00003:224.00003|B4|416.00003:128|416.00003:32|512.00006:32,1,746.5360107421875,2|12,1:2|2:1,3:0:0:0:
        //               168,88,10890,54,0,B|200:72|200:72|232:88|232:88|264:72|264:72|296:88|296:88|328:72|328:72|360:88,1,180,4|0,0:0|0:0,0:0:0:0:
        //               """;
        //Assert.Equal(expected, parsed);
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
}