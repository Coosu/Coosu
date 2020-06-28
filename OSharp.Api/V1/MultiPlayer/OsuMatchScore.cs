using Newtonsoft.Json;
using OSharp.Api.V1.Internal;
using OSharp.Api.V1.Score;

namespace OSharp.Api.V1.MultiPlayer
{
    /// <summary>
    /// Match score API model of a game.
    /// </summary>
    public class OsuMatchScore : IScore
    {
        /// <summary>
        /// Index of player's slot (from 0).
        /// </summary>
        [JsonProperty("slot")]
        public int Slot { get; set; }

        /// <summary>
        /// 0 = No team, 1 = Blue, 2 = Red.
        /// </summary>
        [JsonProperty("team")]
        public int Team { get; set; }

        /// <inheritdoc />
        [JsonProperty("user_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        /// <inheritdoc />
        [JsonProperty("score")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Score { get; set; }

        /// <inheritdoc />
        [JsonProperty("maxcombo")]
        public int MaxCombo { get; set; }

        /// <inheritdoc />
        [JsonProperty("rank")]
        public string Rank { get; set; }

        /// <inheritdoc />
        [JsonProperty("count50")]
        public int Count50 { get; set; }

        /// <inheritdoc />
        [JsonProperty("count100")]
        public int Count100 { get; set; }

        /// <inheritdoc />
        [JsonProperty("count300")]
        public int Count300 { get; set; }

        /// <inheritdoc />
        [JsonProperty("countmiss")]
        public int CountMiss { get; set; }

        /// <inheritdoc />
        [JsonProperty("countgeki")]
        public int CountGeki { get; set; }

        /// <inheritdoc />
        [JsonProperty("countkatu")]
        public int CountKatu { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public bool IsPerfect => PerfectInt == 1;
        [JsonProperty("perfect")]
        private int PerfectInt { get; set; }

        /// <summary>
        /// Represents whether the player is failed at the end of the map.
        /// </summary>
        [JsonIgnore]
        public bool Pass => PassInt == 1;
        [JsonProperty("pass")]
        private int PassInt { get; set; }
    }
}
