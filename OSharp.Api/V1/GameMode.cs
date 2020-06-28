using Newtonsoft.Json;

namespace OSharp.Api.V1
{
    /// <summary>
    /// Game mode.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// osu!standard mode.
        /// </summary>
        [JsonProperty("osu")]
        Standard = 0,
        /// <summary>
        /// osu!taiko mode.
        /// </summary>
        [JsonProperty("taiko")]
        Taiko,
        /// <summary>
        /// osu!catch mode.
        /// </summary>
        [JsonProperty("fruits")]
        CtB,
        /// <summary>
        /// osu!mania mode.
        /// </summary>
        [JsonProperty("mania")]
        OsuMania
    };
}