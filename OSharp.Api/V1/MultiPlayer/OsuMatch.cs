using Newtonsoft.Json;

namespace OSharp.Api.V1.MultiPlayer
{
    /// <summary>
    /// Match API model.
    /// </summary>
    public class OsuMatch
    {
        /// <summary>
        /// Match information.
        /// </summary>
        [JsonProperty("match")]
        public OsuMatchMetadata Match { get; set; }

        /// <summary>
        /// Games' information.
        /// </summary>
        [JsonProperty("games")]
        public OsuMatchGame[] Games { get; set; }
    }
}