using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Page
    {
        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }
    }
}