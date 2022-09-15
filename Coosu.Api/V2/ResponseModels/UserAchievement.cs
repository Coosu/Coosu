using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class UserAchievement
{
    [JsonProperty("achieved_at")]
    public DateTimeOffset AchievedAt { get; set; }

    [JsonProperty("achievement_id")]
    public int AchievementId { get; set; }
}