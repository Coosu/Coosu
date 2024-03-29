﻿using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V1.MultiPlayer;

/// <summary>
/// Match API model.
/// </summary>
public class OsuMatch
{
    /// <summary>
    /// Match information.
    /// </summary>
    [JsonProperty("match")]
    public OsuMatchMetadata Match { get; set; }

    /// <summary>
    /// Games' information.
    /// </summary>
    [JsonProperty("games")]
    public OsuMatchGame[] Games { get; set; }
}