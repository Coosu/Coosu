using Newtonsoft.Json;

namespace OSharp.Api.V1.Replay
{
    /// <summary>
    /// Replay API Model.
    /// </summary>
    public class OsuReplay
    {
        /// <summary>
        /// Base64-encoded replay.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Encoding type. Always be base64.
        /// </summary>
        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        /// <summary>
        /// Represents if the fetched replay is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Content != null && Encoding != null;
    }
}
