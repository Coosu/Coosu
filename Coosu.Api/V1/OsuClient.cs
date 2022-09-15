using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V1.Beatmap;
using Coosu.Api.V1.Internal;
using Coosu.Api.V1.MultiPlayer;
using Coosu.Api.V1.Replay;
using Coosu.Api.V1.Score;
using Coosu.Api.V1.User;

[assembly: CLSCompliant(true)]

namespace Coosu.Api.V1;

/// <summary>
/// Client to access osu!API via HTTP connection.
/// </summary>
public class OsuClient
{
    /// <summary>
    /// Osu api key.
    /// </summary>
    public Key Key { get; }

    private static readonly HttpClientUtility HttpClientUtility = new HttpClientUtility();

    /// <summary>
    /// Create a client to handle api connections.
    /// </summary>
    /// <param name="key">Osu api key. See: https://osu.ppy.sh/p/api. </param>
    public OsuClient(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Retrieve general beatmap information.
    /// </summary>
    /// <param name="since">Return all beatmaps ranked since this date.</param>
    /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
    /// <returns>Fetched beatmaps.</returns>
    public async Task<OsuBeatmap[]> GetBeatmaps(
        DateTimeOffset since,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateBeatmapLink());
        sb.Append($"&since={since.ToUniversalTime():yyyy-MM-dd HH:mm:ss}");
        if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuBeatmap[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve general beatmap information.
    /// </summary>
    /// <param name="beatmapSetId">Specify a beatmap set id.</param>
    /// <param name="gameMode">Specify the game mode.</param>
    /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
    /// <returns>Fetched beatmaps.</returns>
    public async Task<OsuBeatmap[]> GetBeatmaps(
        BeatmapSetId beatmapSetId,
        ConvertibleGameMode gameMode = null,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateBeatmapLink());
        sb.Append($"&s={beatmapSetId}");

        if (gameMode != null)
            AppendConvertibleGameMode(gameMode, sb);
        if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuBeatmap[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve general beatmap set information.
    /// </summary>
    /// <param name="beatmapSetId">Specify a beatmap set id.</param>
    /// <returns>Fetched beatmaps.</returns>
    public async Task<OsuBeatmapSet> GetBeatmapSet(
        BeatmapSetId beatmapSetId)
    {
        var maps = await GetBeatmaps(beatmapSetId);
        return new OsuBeatmapSet(maps);
    }

    /// <summary>
    /// Retrieve general beatmap set information.
    /// </summary>
    /// <param name="beatmapId">Specify a beatmap set id.</param>
    /// <returns>Fetched beatmaps.</returns>
    public async Task<OsuBeatmapSet> GetBeatmapSet(
        BeatmapId beatmapId)
    {
        var map = await GetBeatmap(beatmapId);
        return await GetBeatmapSet(new BeatmapSetId(map.BeatmapSetId));
    }

    /// <summary>
    /// Retrieve general beatmap information.
    /// </summary>
    /// <param name="beatmapId">Specify a beatmap id.</param>
    /// <returns>Fetched beatmap.</returns>
    public async Task<OsuBeatmap?> GetBeatmap(
        BeatmapId beatmapId)
    {
        var sb = new StringBuilder(Key.CreateBeatmapLink());
        sb.Append($"&b={beatmapId}");

        var obj = await HttpClientUtility.HttpGet<OsuBeatmap[]>(sb.ToString());
        return obj.FirstOrDefault();
    }

    /// <summary>
    /// Retrieve general beatmap information.
    /// </summary>
    /// <param name="nameOrId">Specify a user or a username to return metadata from.</param>
    /// <param name="gameMode">Specify the game mode.</param>
    /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
    /// <returns>Fetched beatmaps.</returns>
    public async Task<OsuBeatmap[]> GetBeatmaps(
        UserComponent? nameOrId = null,
        ConvertibleGameMode? gameMode = null,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateBeatmapLink());
        if (nameOrId != null)
            AppendUser(nameOrId, sb);
        if (gameMode != null)
            AppendConvertibleGameMode(gameMode, sb);
        if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuBeatmap[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve general beatmap information.
    /// </summary>
    /// <param name="hash">The beatmap hash. It can be used, for instance, if you're trying to get what beatmap has a replay played in, as .osr replays only provide beatmap hashes (example of hash: a5b99395a42bd55bc5eb1d2411cbdf8b).</param>
    /// <returns>Fetched beatmap.</returns>
    public async Task<OsuBeatmap?> GetBeatmap(
        string hash)
    {
        var sb = new StringBuilder(Key.CreateBeatmapLink());
        sb.Append($"&h={hash}");

        var obj = await HttpClientUtility.HttpGet<OsuBeatmap[]>(sb.ToString());
        return obj.FirstOrDefault();
    }

    /// <summary>
    /// Retrieve general user information.
    /// </summary>
    /// <param name="nameOrId">Specify a user id or a username to return metadata from.</param>
    /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
    /// <param name="eventDays">Max number of days between now and last event date (1 - 31). Default value is 1.</param>
    /// <returns>Fetched users.</returns>
    public async Task<OsuUser[]> GetUsers(
        UserComponent nameOrId,
        GameMode? gameMode = null,
        int? eventDays = null)
    {
        var sb = new StringBuilder(Key.CreateUserLink(nameOrId));

        if (gameMode != null)
            AppendGameMode(gameMode, sb);
        if (eventDays != null)
            sb.Append($"&event_days={eventDays}");

        var obj = await HttpClientUtility.HttpGet<OsuUser[]>(sb.ToString());
        return obj;
    }


    /// <summary>
    /// Retrieve information about the top 100 scores of a specified beatmap.
    /// </summary>
    /// <param name="beatmapId">Specify a beatmap id to return score information from.</param>
    /// <param name="nameOrId">Specify a user id or a user name to return score information for.</param>
    /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
    /// <param name="mods">Specify a mod or mod combination.</param>
    /// <param name="limitCount">Amount of results from the top (1 - 100). Default value is 50.</param>
    /// <returns>Fetched scores.</returns>
    public async Task<OsuPlayScore[]> GetScores(
        BeatmapId beatmapId,
        UserComponent? nameOrId = null,
        GameMode? gameMode = null,
        Mod? mods = null,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateScoreLink(beatmapId));
        if (nameOrId != null)
            AppendUser(nameOrId, sb);
        if (gameMode != null)
            AppendGameMode(gameMode, sb);
        if (mods != null)
            sb.Append($"&mods={(int)mods.Value}");
        if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuPlayScore[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve information about top scores for a specified user.
    /// </summary>
    /// <param name="nameOrId">Specify a user_id or a username to return score information for.</param>
    /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
    /// <param name="limitCount">Amount of results (1 - 100). Default value is 10.</param>
    /// <returns>Fetched user's best scores.</returns>
    public async Task<OsuUserBest[]> GetUserBestScores(
        UserComponent nameOrId,
        GameMode? gameMode = null,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateUserBestLink(nameOrId));
        if (gameMode != null)
            AppendGameMode(gameMode, sb);
        else if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuUserBest[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve information about recent plays over the last 24 hours for a specified user.
    /// </summary>
    /// <param name="nameOrId">Specify a user_id or a username to return score information for.</param>
    /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
    /// <param name="limitCount">Amount of results (1 - 50). Default value is 10.</param>
    /// <returns>Fetched user's recent scores.</returns>
    public async Task<OsuUserRecent[]> GetUserRecentScores(
        UserComponent nameOrId,
        GameMode? gameMode = null,
        int? limitCount = null)
    {
        var sb = new StringBuilder(Key.CreateUserRecentLink(nameOrId));
        if (gameMode != null)
            AppendGameMode(gameMode, sb);
        if (limitCount != null)
            AppendLimit(limitCount, sb);

        var obj = await HttpClientUtility.HttpGet<OsuUserRecent[]>(sb.ToString());
        return obj;
    }

    /// <summary>
    /// Retrieve information about multi-player match.
    /// </summary>
    /// <param name="matchId">Match id to get information from.</param>
    /// <returns>Fetched multi-player lobby.</returns>
    public async Task<OsuMatch> GetMatch(int matchId)
    {
        string match = Key.CreateMatchLink(matchId);

        var obj = await HttpClientUtility.HttpGet<OsuMatch>(match);
        return obj;
    }

    /// <summary>
    /// Retrieve information about replay data of the specified user's score on a map.
    /// </summary>
    /// <param name="gameMode">The mode the score was played in.</param>
    /// <param name="nameOrId">The user that has played the beatmap.</param>
    /// <param name="beatmapId">The beatmap ID in which the replay was played.</param>
    /// <returns>Fetched replay data.</returns>
    public async Task<OsuReplay> GetReplay(
        GameMode gameMode,
        UserComponent nameOrId,
        BeatmapId beatmapId)
    {
        string replay = Key.CreateReplayLink(gameMode, beatmapId, nameOrId);
        var obj = await HttpClientUtility.HttpGet<OsuReplay>(replay);
        return obj;
        // Note that the binary data you get when you decode above base64-string is not the contents of an .osr-file.
        // It is the LZMA stream referred to by the osu-wiki here:
        // The remaining data contains information about mouse movement and key presses in an wikipedia:LZMA stream
        // https://osu.ppy.sh/wiki/Osr_(file_format)#Format
    }


    private static void AppendGameMode(GameMode? gameMode, StringBuilder sb)
    {
        if (gameMode == null)
            throw new NullReferenceException();
        sb.Append($"&m={(int)gameMode.Value}");
    }

    private static void AppendConvertibleGameMode(ConvertibleGameMode gameMode, StringBuilder sb)
    {
        sb.Append(gameMode.ConvertOption == BeatmapConvertOption.Included
            ? $"&m={(int)gameMode.GameMode}&a=1"
            : $"&m={(int)gameMode.GameMode}");
    }

    private static void AppendLimit(int? limitCount, StringBuilder sb)
    {
        sb.Append($"&limit={limitCount}");
    }

    private static void AppendUser(UserComponent nameOrId, StringBuilder sb)
    {
        sb.Append($"&{nameOrId.CreateUserParameter()}");
    }
}