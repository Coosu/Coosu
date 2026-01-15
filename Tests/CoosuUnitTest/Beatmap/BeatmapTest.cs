using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Beatmap.Extensions.Playback;
using Xunit;

namespace CoosuUnitTest.Beatmap;

public class BeatmapTest
{
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