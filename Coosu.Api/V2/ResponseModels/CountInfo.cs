using Newtonsoft.Json;
using System;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class CountInfo
    {
        [JsonProperty("start_date")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}