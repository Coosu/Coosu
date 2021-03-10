using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class NominationsSummary
    {
        [JsonProperty("current")]
        public long Current { get; set; }

        [JsonProperty("required")]
        public long NominationsSummaryRequired { get; set; }
    }
}