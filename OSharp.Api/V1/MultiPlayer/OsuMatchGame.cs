using System;
using Newtonsoft.Json;
using OSharp.Api.V1.Internal;

namespace OSharp.Api.V1.MultiPlayer
{
    /// <summary>
    /// Game API model of the match.
    /// </summary>
    public class OsuMatchGame
    {
        /// <summary>
        /// Game ID.
        /// </summary>
        [JsonProperty("game_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long GameId { get; set; }

        /// <summary>
        /// Game start time. (UTC)
        /// </summary>
        [JsonProperty("start_time")]
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Game end time. (UTC)
        /// </summary>
        [JsonProperty("end_time")]
        public DateTimeOffset? EndTime { get; set; }

        /// <summary>
        /// Beatmap ID of the game.
        /// </summary>
        [JsonProperty("beatmap_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BeatmapId { get; set; }

        /// <summary>
        /// Play mode of the game.
        /// </summary>
        [JsonProperty("play_mode")]
        public GameMode PlayMode { get; set; }

        /// <summary>
        /// Couldn't find.
        /// </summary>
        [JsonProperty("match_type")]
        public string MatchType { get; set; }

        /// <summary>
        /// Winning condition of the game.
        /// </summary>
        [JsonProperty("scoring_type")]
        public MatchScoringType ScoringType { get; set; }

        /// <summary>
        /// Team type of the game.
        /// </summary>
        [JsonProperty("team_type")]
        public MatchTeamType TeamType { get; set; }

        /// <summary>
        /// Mod of the game.
        /// </summary>
        [JsonProperty("mods")]
        public Mod Mods { get; set; }

        /// <summary>
        /// Scores of the game.
        /// </summary>
        [JsonProperty("scores")]
        public OsuMatchScore[] Scores { get; set; }
    }
}