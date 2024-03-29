﻿using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V1;

/// <summary>
/// Game mode.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// osu!standard mode.
    /// </summary>
    [JsonProperty("osu")]
    Standard = 0,
    /// <summary>
    /// osu!taiko mode.
    /// </summary>
    [JsonProperty("taiko")]
    Taiko,
    /// <summary>
    /// osu!catch mode.
    /// </summary>
    [JsonProperty("fruits")]
    CtB,
    /// <summary>
    /// osu!mania mode.
    /// </summary>
    [JsonProperty("mania")]
    OsuMania
};