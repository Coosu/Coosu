using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2;
using Coosu.Api.V2.RequestModels;
using Coosu.Api.V2.ResponseModels;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoosuUnitTest.Api
{
    [TestClass]
    public class ApiTest
    {
        private readonly int _clientId;
        private readonly string _clientSecret;
        private readonly IConfiguration _configuration;
        private readonly UserToken _publicToken;

        public ApiTest()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Secretes>();

            _configuration = builder.Build();
            _clientId = int.Parse(_configuration["ClientId"]);
            _clientSecret = _configuration["ClientSecret"];
            var client = new AuthorizationClient();
            _publicToken = client.GetPublicToken(_clientId, _clientSecret).Result;
        }

        [TestMethod]
        public async Task SearchBeatmapset()
        {
            var v2 = new OsuClientV2(_publicToken);
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

        [TestMethod]
        public async Task SearchUser()
        {
            var v2 = new OsuClientV2(_publicToken);
            var beatmaps = await v2.User.GetUser("1243669", GameMode.Osu);
        }
    }

    public class Secretes
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
