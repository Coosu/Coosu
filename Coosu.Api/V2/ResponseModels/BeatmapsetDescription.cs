using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class BeatmapsetDescription
    {
        [JsonProperty("description")]
        public string DescriptionDescription { get; set; }
    }
}