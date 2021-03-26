using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class UserCover
    {
        [JsonProperty("custom_url")]
        public string CustomUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; } // nullable for custom
    }
}