using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSharp.Api.HttpClient;
using OSharp.Api.V1.Beatmap;
using OSharp.Api.V1.Internal;
using OSharp.Api.V1.MultiPlayer;
using OSharp.Api.V1.Replay;
using OSharp.Api.V1.Score;
using OSharp.Api.V1.User;

[assembly: CLSCompliant(true)]

namespace OSharp.Api.V1
{
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

            var link = Key.CreateBeatmapLink();
            string json = GetJson(link + "&h=\"-1\"");
            object obj = JsonConvert.DeserializeObject(json);

            if (!(obj is JObject jObj)) return;
            if (jObj.Children().Count() != 1) return;
            var error = jObj.Children().FirstOrDefault();
            if (error?.Path == "error")
            {
                throw new InvalidOperationException(error.FirstOrDefault()?.ToString());
            }
        }

        /// <summary>
        /// Retrieve general beatmap information.
        /// </summary>
        /// <param name="since">Return all beatmaps ranked since this date.</param>
        /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
        /// <returns>Fetched beatmaps.</returns>
        public OsuBeatmap[] GetBeatmaps(
            DateTimeOffset since,
            int? limitCount = null)
        {
            var sb = new StringBuilder(Key.CreateBeatmapLink());
            sb.Append($"&since={since.ToUniversalTime():yyyy-MM-dd HH:mm:ss}");
            if (limitCount != null)
                AppendLimit(limitCount, sb);

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve general beatmap information.
        /// </summary>
        /// <param name="beatmapSetId">Specify a beatmap set id.</param>
        /// <param name="gameMode">Specify the game mode.</param>
        /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
        /// <returns>Fetched beatmaps.</returns>
        public OsuBeatmap[] GetBeatmaps(
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

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve general beatmap set information.
        /// </summary>
        /// <param name="beatmapSetId">Specify a beatmap set id.</param>
        /// <returns>Fetched beatmaps.</returns>
        public OsuBeatmapSet GetBeatmapSet(
            BeatmapSetId beatmapSetId)
        {
            var maps = GetBeatmaps(beatmapSetId);
            return new OsuBeatmapSet(maps);
        }

        /// <summary>
        /// Retrieve general beatmap set information.
        /// </summary>
        /// <param name="beatmapId">Specify a beatmap set id.</param>
        /// <returns>Fetched beatmaps.</returns>
        public OsuBeatmapSet GetBeatmapSet(
            BeatmapId beatmapId)
        {
            var map = GetBeatmap(beatmapId);
            return GetBeatmapSet(new BeatmapSetId(map.BeatmapSetId));
        }

        /// <summary>
        /// Retrieve general beatmap information.
        /// </summary>
        /// <param name="beatmapId">Specify a beatmap id.</param>
        /// <returns>Fetched beatmap.</returns>
        public OsuBeatmap GetBeatmap(
            BeatmapId beatmapId)
        {
            var sb = new StringBuilder(Key.CreateBeatmapLink());
            sb.Append($"&b={beatmapId}");

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(json);
            return obj.FirstOrDefault();
        }

        /// <summary>
        /// Retrieve general beatmap information.
        /// </summary>
        /// <param name="nameOrId">Specify a user or a username to return metadata from.</param>
        /// <param name="gameMode">Specify the game mode.</param>
        /// <param name="limitCount">Amount of results (1 - 500). Default value is 500.</param>
        /// <returns>Fetched beatmaps.</returns>
        public OsuBeatmap[] GetBeatmaps(
            UserComponent nameOrId = null,
            ConvertibleGameMode gameMode = null,
            int? limitCount = null)
        {
            var sb = new StringBuilder(Key.CreateBeatmapLink());
            if (nameOrId != null)
                AppendUser(nameOrId, sb);
            if (gameMode != null)
                AppendConvertibleGameMode(gameMode, sb);
            if (limitCount != null)
                AppendLimit(limitCount, sb);

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve general beatmap information.
        /// </summary>
        /// <param name="hash">The beatmap hash. It can be used, for instance, if you're trying to get what beatmap has a replay played in, as .osr replays only provide beatmap hashes (example of hash: a5b99395a42bd55bc5eb1d2411cbdf8b).</param>
        /// <returns>Fetched beatmap.</returns>
        public OsuBeatmap GetBeatmap(
            string hash)
        {
            var sb = new StringBuilder(Key.CreateBeatmapLink());
            sb.Append($"&h={hash}");

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuBeatmap[]>(json);
            return obj.FirstOrDefault();
        }

        /// <summary>
        /// Retrieve general user information.
        /// </summary>
        /// <param name="nameOrId">Specify a user id or a username to return metadata from.</param>
        /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
        /// <param name="eventDays">Max number of days between now and last event date (1 - 31). Default value is 1.</param>
        /// <returns>Fetched users.</returns>
        public OsuUser[] GetUsers(
            UserComponent nameOrId,
            GameMode? gameMode = null,
            int? eventDays = null)
        {
            var sb = new StringBuilder(Key.CreateUserLink(nameOrId));

            if (gameMode != null)
                AppendGameMode(gameMode, sb);
            if (eventDays != null)
                sb.Append($"&event_days={eventDays}");

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuUser[]>(json);
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
        public OsuPlayScore[] GetScores(
            BeatmapId beatmapId,
            UserComponent nameOrId = null,
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

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuPlayScore[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve information about top scores for a specified user.
        /// </summary>
        /// <param name="nameOrId">Specify a user_id or a username to return score information for.</param>
        /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
        /// <param name="limitCount">Amount of results (1 - 100). Default value is 10.</param>
        /// <returns>Fetched user's best scores.</returns>
        public OsuUserBest[] GetUserBestScores(
            UserComponent nameOrId,
            GameMode? gameMode = null,
            int? limitCount = null)
        {
            var sb = new StringBuilder(Key.CreateUserBestLink(nameOrId));
            if (gameMode != null)
                AppendGameMode(gameMode, sb);
            else if (limitCount != null)
                AppendLimit(limitCount, sb);

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuUserBest[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve information about recent plays over the last 24 hours for a specified user.
        /// </summary>
        /// <param name="nameOrId">Specify a user_id or a username to return score information for.</param>
        /// <param name="gameMode">Specify the game mode. Default value is 0.</param>
        /// <param name="limitCount">Amount of results (1 - 50). Default value is 10.</param>
        /// <returns>Fetched user's recent scores.</returns>
        public OsuUserRecent[] GetUserRecentScores(
            UserComponent nameOrId,
            GameMode? gameMode = null,
            int? limitCount = null)
        {
            var sb = new StringBuilder(Key.CreateUserRecentLink(nameOrId));
            if (gameMode != null)
                AppendGameMode(gameMode, sb);
            if (limitCount != null)
                AppendLimit(limitCount, sb);

            string json = GetJson(sb);
            var obj = JsonConvert.DeserializeObject<OsuUserRecent[]>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve information about multi-player match.
        /// </summary>
        /// <param name="matchId">Match id to get information from.</param>
        /// <returns>Fetched multi-player lobby.</returns>
        public OsuMatch GetMatch(int matchId)
        {
            string match = Key.CreateMatchLink(matchId);

            string json = GetJson(match);
            var obj = JsonConvert.DeserializeObject<OsuMatch>(json);
            return obj;
        }

        /// <summary>
        /// Retrieve information about replay data of the specified user's score on a map.
        /// </summary>
        /// <param name="gameMode">The mode the score was played in.</param>
        /// <param name="nameOrId">The user that has played the beatmap.</param>
        /// <param name="beatmapId">The beatmap ID in which the replay was played.</param>
        /// <returns>Fetched replay data.</returns>
        public OsuReplay GetReplay(
            GameMode gameMode,
            UserComponent nameOrId,
            BeatmapId beatmapId)
        {
            string replay = Key.CreateReplayLink(gameMode, beatmapId, nameOrId);
            string json = GetJson(replay);
            OsuReplay obj = JsonConvert.DeserializeObject<OsuReplay>(json);
            return obj;
            // Note that the binary data you get when you decode above base64-string is not the contents of an .osr-file.
            // It is the LZMA stream referred to by the osu-wiki here:
            // The remaining data contains information about mouse movement and key presses in an wikipedia:LZMA stream
            // https://osu.ppy.sh/wiki/Osr_(file_format)#Format
        }

        private static string GetJson(StringBuilder sb)
        {
            return GetJson(sb.ToString());
        }

        private static string GetJson(string url)
        {
            return HttpClientUtility.HttpGet(url);
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
}
