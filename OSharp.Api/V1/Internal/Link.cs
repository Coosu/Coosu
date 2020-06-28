using OSharp.Api.V1.Beatmap;
using OSharp.Api.V1.User;

namespace OSharp.Api.V1.Internal
{
    internal static class Link
    {
        public const string BeatmapDownloadUri = "https://osu.ppy.sh/d/";
        public const string BloodcatUri = "https://bloodcat.com/osu/s/";
        public const string BeatmapSetThumbUri = "https://b.ppy.sh/thumb/";
        public const string BeatmapUri = "https://osu.ppy.sh/b/";
        public const string BeatmapSetUri = "https://osu.ppy.sh/s/";
        public const string OsuDirect = "osu://s/";

        public const string UserPageUri = "https://osu.ppy.sh/u/";
        public const string AvatarUri = "https://a.ppy.sh/";
        public const string FlagUri = "https://new.ppy.sh/images/flags/";
        public const string OsuStatsUri = "https://osustats.ppy.sh/u/";
        public const string OsuTrackUri = "https://ameobea.me/osutrack/user/";
        public const string OsuSkillsUri = "http://osuskills.tk/user/";
        public const string OsuChanUri = "https://syrin.me/osuchan/u/";
        public const string PpPlusUri = "https://syrin.me/pp+/u/";

    }

    internal static class LinkBuildExtension
    {
        private const string OsuApiUri = "https://osu.ppy.sh/api/";
        private const string OsuBeatmap = "get_beatmaps?";
        private const string OsuScores = "get_scores?";
        private const string OsuUser = "get_user?";
        private const string OsuUserBest = "get_user_best?";
        private const string OsuUserRecent = "get_user_recent?";
        private const string OsuMatch = "get_match?";
        private const string OsuReplay = "get_replay?";

        public static string CreateBeatmapLink(this Key key)
        {
            return $"{OsuApiUri}{OsuBeatmap}k={key}";
        }

        public static string CreateUserLink(this Key key, UserComponent user)
        {
            return $"{OsuApiUri}{OsuUser}k={key}&{CreateUserParameter(user)}";
        }

        public static string CreateScoreLink(this Key key, BeatmapId id)
        {
            return $"{OsuApiUri}{OsuScores}k={key}&b={id.Id}";
        }

        public static string CreateUserBestLink(this Key key, UserComponent user)
        {
            return $"{OsuApiUri}{OsuUserBest}k={key}&{CreateUserParameter(user)}";
        }

        public static string CreateUserRecentLink(this Key key, UserComponent user)
        {
            return $"{OsuApiUri}{OsuUserRecent}k={key}&{CreateUserParameter(user)}";
        }

        public static string CreateMatchLink(this Key key, int match)
        {
            return $"{OsuApiUri}{OsuMatch}k={key}&mp={match}";
        }

        public static string CreateReplayLink(this Key key, GameMode gameMode, BeatmapId id, UserComponent user)
        {
            return string.Format("{0}{1}k={2}&m={3}&b={4}&u={5}",
                OsuApiUri,
                OsuReplay,
                key,
                gameMode,
                id.Id,
                user.IdOrName);
        }

        public static string CreateUserParameter(this UserComponent user)
        {
            if (user.IdType != UserComponent.Type.Auto)
            {
                return string.Format("u={0}&type={1}",
                    user.IdOrName,
                    user.IdType == UserComponent.Type.Id ? "id" : "string");
            }
            else
            {
                return $"u={user.IdOrName}";
            }
        }
    }
}
