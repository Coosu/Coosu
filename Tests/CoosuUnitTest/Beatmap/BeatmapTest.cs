using System.IO;
using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2;
using Coosu.Api.V2.RequestModels;
using Coosu.Api.V2.ResponseModels;
using Coosu.Beatmap;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoosuUnitTest.Beatmap
{
    [TestClass]
    public class BeatmapTest
    {

        [TestMethod]
        public async Task ReadAndWrite()
        {
            //var folder = @"C:\Users\milkitic\Desktop\1002455 supercell - Giniro Hikousen  (Ttm bootleg Edit)";
            //var file = @"supercell - Giniro Hikousen  (Ttm bootleg Edit) (yf_bmp) [4K Hard].osu";
            var folder = @"C:\Users\milkitic\Downloads\1376486 Risshuu feat. Choko - Take [no video] (2)";
            var file = @"Risshuu feat. Choko - Take (yf_bmp) [Ta~ke take take take take take tatata~].osu";
            var osuFile = await OsuFile.ReadFromFileAsync(Path.Combine(folder, file));

            osuFile.Metadata.Version += " (TEST)";
            osuFile.WriteOsuFile(Path.Combine(folder, osuFile.GetPath(osuFile.Metadata.Version)));
        }
    }
}
