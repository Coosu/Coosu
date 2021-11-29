using Coosu.Api.V2;
using Coosu.Api.V2.RequestModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoosuUnitTest.Api
{
    [TestClass]
    public class ApiTest
    {
        [TestMethod]
        public void SearchBeatmapset()
        {
            var client = new AuthorizationClient();
            var publicToken = client.GetPublicToken(11169, "");
            var v2 = new OsuClientV2(publicToken);
            var beatmaps = v2.Beatmap.SearchBeatmapset(new SearchOptions
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
