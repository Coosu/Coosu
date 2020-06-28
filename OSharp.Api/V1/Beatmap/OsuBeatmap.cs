using System;
using Newtonsoft.Json;
using OSharp.Api.V1.Internal;

namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Beatmap API model.
    /// </summary>
    public class OsuBeatmap
    {
        /// <summary>
        /// Beatmap approved state.
        /// </summary>
        [JsonProperty("approved")]
        public BeatmapApprovedState? Approved { get; set; }

        /// <summary>
        /// Beatmap approved date. (UTC)
        /// </summary>
        [JsonProperty("approved_date")]
        public DateTimeOffset? ApprovedDate { get; set; }

        /// <summary>
        /// Beatmap updated date. (UTC)
        /// </summary>
        [JsonProperty("last_update")]
        public DateTimeOffset LastUpdate { get; set; }

        /// <summary>
        /// Beatmap artist.
        /// </summary>
        [JsonProperty("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// Beatmap ID.
        /// </summary>
        [JsonProperty("beatmap_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BeatmapId { get; set; }

        /// <summary>
        /// Beatmap-set ID.
        /// </summary>
        [JsonProperty("beatmapset_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BeatmapSetId { get; set; }

        /// <summary>
        /// Beat-per-minute of the beatmap.
        /// </summary>
        [JsonProperty("bpm")]
        public double Bpm { get; set; }

        /// <summary>
        /// Beatmap creator.
        /// </summary>
        [JsonProperty("creator")]
        public string Creator { get; set; }

        /// <summary>
        /// Beatmap creator's ID.
        /// </summary>
        [JsonProperty("creator_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CreatorId { get; set; }

        /// <summary>
        /// The amount of stars the map would have in-game and on the website.
        /// </summary>
        [JsonProperty("difficultyrating")]
        public double? DifficultyRating { get; set; }

        /// <summary>
        /// Circle size value. (CS)
        /// </summary>
        [JsonProperty("diff_size")]
        public float? CircleSize { get; set; }

        /// <summary>
        /// Overall difficulty. (OD)
        /// </summary>
        [JsonProperty("diff_overall")]
        public float? OverallDifficulty { get; set; }

        /// <summary>
        /// Approach Rate. (AR)
        /// </summary>
        [JsonProperty("diff_approach")]
        public float? ApproachRate { get; set; }

        /// <summary>
        /// Health drain. (HP)
        /// </summary>
        [JsonProperty("diff_drain")]
        public float? HealthDrain { get; set; }

        /// <summary>
        /// Seconds from first note to last note not including breaks.
        /// </summary>
        [JsonProperty("hit_length")]
        public int? HitLength { get; set; }

        /// <summary>
        /// Beatmap source.
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Beatmap genre.
        /// </summary>
        [JsonProperty("genre_id")]
        public BeatmapGenre Genre { get; set; }

        /// <summary>
        /// Beatmap language.
        /// </summary>
        [JsonProperty("language_id")]
        public BeatmapLanguage Language { get; set; }

        /// <summary>
        /// Beatmap title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Seconds from first note to last note including breaks.
        /// </summary>
        [JsonProperty("total_length")]
        public int? TotalLength { get; set; }

        /// <summary>
        /// Beatmap difficulty name.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Beatmap md5 hash.
        /// </summary>
        [JsonProperty("file_md5")]
        public string FileMd5 { get; set; }

        /// <summary>
        /// Beatmap game mode.
        /// </summary>
        [JsonProperty("mode")]
        public GameMode? GameMode { get; set; }

        /// <summary>
        /// Beatmap tags. (separated by spaces)
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Beatmap favourite count.
        /// </summary>
        [JsonProperty("favourite_count")]
        public int? FavouriteCount { get; set; }

        /// <summary>
        /// Beatmap play count.
        /// </summary>
        [JsonProperty("playcount")]
        public int? PlayCount { get; set; }

        /// <summary>
        /// Beatmap pass count.
        /// </summary>
        [JsonProperty("passcount")]
        public int? PassCount { get; set; }

        /// <summary>
        /// Beatmap max combo.
        /// </summary>
        [JsonProperty("max_combo")]
        public int? MaxCombo { get; set; }
    }
}
