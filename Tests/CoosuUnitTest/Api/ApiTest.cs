using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2;
using Coosu.Api.V2.RequestModels;
using Coosu.Api.V2.ResponseModels;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CoosuUnitTest.Api;

public class ApiTest
{
    private readonly int _clientId;
    private readonly string _clientSecret;
    private readonly IConfiguration _configuration;
    private readonly UserToken _publicToken;
    private readonly OsuClientV2 _client;

    public ApiTest()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Secretes>();

        var clientOptions = new ClientOptions()
        {
            ProxyUrl = "http://127.0.0.1:10801"
        };
        _configuration = builder.Build();
        _clientId = int.Parse(_configuration["ClientId"]);
        _clientSecret = _configuration["ClientSecret"];
        var client = new AuthorizationClient(clientOptions);
        _publicToken = client.GetPublicToken(_clientId, _clientSecret).Result;
        _client = new OsuClientV2(_publicToken, clientOptions);
    }

    [Fact]
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

    [Fact]
    public async Task SearchUser()
    {
        var v2 = new OsuClientV2(_publicToken);
        var beatmaps = await v2.User.GetUser("1243669", GameMode.Osu);
    }

    [Fact]
    public async Task GetUserScores()
    {
        var v2 = new OsuClientV2(_publicToken);
        var beatmaps = await v2.User.GetUserScores("1243669", ScoreType.Best, gameMode: GameMode.Mania);
    }
}

public class Secretes
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}