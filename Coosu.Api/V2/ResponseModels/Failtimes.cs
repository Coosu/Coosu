using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Failtimes
    {
        [JsonProperty("fail")]
        public long[] Fail { get; set; }

        [JsonProperty("exit")]
        public long[] Exit { get; set; }
    }
}