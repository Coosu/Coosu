using System.Threading.Tasks;
using Coosu.Api.V2;
using Coosu.Api.V2.RequestModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoosuUnitTest.Api
{
    [TestClass]
    public class ApiTest
    {
        [TestMethod]
        public async Task SearchBeatmapset()
        {
            var client = new AuthorizationClient();
            var publicToken = await client.GetPublicToken(5044, "SwbQi6CeSs13gE01302Qpp8BrqEADVj5DQadtdbD");
            var v2 = new OsuClientV2(publicToken);
            var beatmaps = await v2.Beatmap.SearchBeatmapset(new SearchOptions
            {
                BeatmapsetStatus = BeatmapsetStatus.Any,
                Gamemode = GameMode.Osu,
                //Genre = GenreType.Electronic,
                Sort = BeatmapsetSearchSort.Updated,
                MustHasStoryboard = true,
                MustHasVideo = true,
                //Language = LanguageType.Instrumental
            });
        }
    }
}
