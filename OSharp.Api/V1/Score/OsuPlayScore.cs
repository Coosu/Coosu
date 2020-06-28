using System;
using Newtonsoft.Json;
using OSharp.Api.V1.Internal;

namespace OSharp.Api.V1.Score
{
    /// <summary>
    /// Score API Model.
    /// </summary>
    public class OsuPlayScore : IScore
    {
        /// <summary>
        /// Score ID.
        /// </summary>
        [JsonProperty("score_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ScoreId { get; set; }

        /// <inheritdoc />
        [JsonProperty("score")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Score { get; set; }

        /// <summary>
        /// User who played the score.
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        /// <inheritdoc />
        [JsonProperty("count300")]
        public int Count300 { get; set; }

        /// <inheritdoc />
        [JsonProperty("count100")]
        public int Count100 { get; set; }

        /// <inheritdoc />
        [JsonProperty("count50")]
        public int Count50 { get; set; }

        /// <inheritdoc />
        [JsonProperty("countmiss")]
        public int CountMiss { get; set; }

        /// <inheritdoc />
        [JsonProperty("maxcombo")]
        public int MaxCombo { get; set; }

        /// <inheritdoc />
        [JsonProperty("countkatu")]
        public int CountKatu { get; set; }

        /// <inheritdoc />
        [JsonProperty("countgeki")]
        public int CountGeki { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public bool IsPerfect => PerfectInt == 1;
        [JsonProperty("perfect")]
        internal int PerfectInt { get; set; }

        /// <summary>
        /// Enabled mods of the score.
        /// </summary>
        [JsonProperty("enabled_mods")]
        public Mod EnabledMods { get; set; }

        /// <inheritdoc />
        [JsonProperty("user_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// Score date. (UTC)
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset Date => DateTimeOffset.Parse(DateString);

        /// <summary>
        /// Score date string. (UTC)
        /// </summary>
        [JsonProperty("date")]
        public string DateString { get; set; }

        /// <inheritdoc />
        [JsonProperty("rank")]
        public string Rank { get; set; }

        /// <summary>
        /// Score performance-point.
        /// </summary>
        [JsonProperty("pp")]
        public float Pp { get; set; }

        /// <summary>
        /// Represents whether the score has replay.
        /// </summary>
        [JsonIgnore]
        public bool ReplayAvailable => ReplayAvailableInt == 1;
        [JsonProperty("replay_available")]
        private int ReplayAvailableInt { get; set; }
    }
}
