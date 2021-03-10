using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Genre
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}