using System;

namespace Coosu.Api.HttpClient;

public sealed class ClientOptions
{
    public string? ProxyUrl { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(8);
    public int RetryCount { get; set; } = 3;
}