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
    [Fact(Skip = "Requires osu! API credentials and network access.")]
    public async Task SearchBeatmapset()
    {
        var v2 = new OsuClientV2(await GetPublicToken());
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

    [Fact(Skip = "Requires osu! API credentials and network access.")]
    public async Task SearchUser()
    {
        var v2 = new OsuClientV2(await GetPublicToken());
        var beatmaps = await v2.User.GetUser("1243669", GameMode.Osu);
    }

    [Fact(Skip = "Requires osu! API credentials and network access.")]
    public async Task GetUserScores()
    {
        var v2 = new OsuClientV2(await GetPublicToken());
        var beatmaps = await v2.User.GetUserScores("1243669", ScoreType.Best, gameMode: GameMode.Mania);
    }

    private static async Task<UserToken> GetPublicToken()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Secretes>();

        var clientOptions = new ClientOptions()
        {
            ProxyUrl = "http://127.0.0.1:7897"
        };

        var configuration = builder.Build();
        var clientId = int.Parse(configuration["ClientId"]);
        var clientSecret = configuration["ClientSecret"];
        var client = new AuthorizationClient(clientOptions);
        return await client.GetPublicToken(clientId, clientSecret);
    }
}

public class Secretes
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}