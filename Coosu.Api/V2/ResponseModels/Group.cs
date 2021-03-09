using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Group
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("colour")]
        public string Colour { get; set; }

        [JsonProperty("playmodes")]
        public string[] PlayModes { get; set; }

        [JsonProperty("is_probationary")]
        public bool IsProbationary { get; set; }
    }
}