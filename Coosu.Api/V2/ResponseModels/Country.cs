using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Country
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}