using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Availability
    {
        [JsonProperty("download_disabled")]
        public bool DownloadDisabled { get; set; }

        [JsonProperty("more_information")]
        public string MoreInformation { get; set; }
    }
}