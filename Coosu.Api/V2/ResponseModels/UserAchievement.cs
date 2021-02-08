using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class UserAchievement
    {
        [JsonProperty("achieved_at")]
        public DateTimeOffset AchievedAt { get; set; }

        [JsonProperty("achievement_id")]
        public long AchievementId { get; set; }
    }
}