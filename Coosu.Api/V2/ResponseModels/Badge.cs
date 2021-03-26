using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Badge
    {
        [JsonProperty("awarded_at")]
        public DateTimeOffset AwardedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}