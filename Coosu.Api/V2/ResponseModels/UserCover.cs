using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class UserCover
    {
        [JsonProperty("custom_url")]
        public Uri CustomUrl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; } // nullable for custom
    }
}