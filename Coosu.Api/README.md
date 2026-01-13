# Coosu.Api

[![NuGet](https://img.shields.io/nuget/v/Coosu.Api.svg)](https://www.nuget.org/packages/Coosu.Api/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Api.svg)](https://www.nuget.org/packages/Coosu.Api/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

**Coosu.Api** is a lightweight and easy-to-use .NET Standard library for accessing both **osu! API v1** and **osu! API v2**. It simplifies the process of fetching data such as user profiles, beatmap information, scores, replays, and more.

## Features

*   **Supports osu! API v1 & v2**: Comprehensive coverage for both API versions.
*   **Typed Client and Models**: Strongly-typed classes for API requests and responses, making it easier to work with data.
*   **Asynchronous Operations**: All API calls are asynchronous (`async`/`await`) for non-blocking operations.
*   **Helper Classes and Extensions**: Includes utility classes like `UserExtra`, `BeatmapExtra`, and `ReplayExtra` (for v1) to generate metadata (e.g., profile links, flag URIs, replay file generation) and provide useful extension methods.
*   **OAuth 2.0 for API v2**: Handles authenticationフロー for API v2 using OAuth 2.0 (Authorization Code Grant and Client Credentials Grant).
*   **Customizable HTTP Client**: Allows configuration of the underlying HTTP client, including timeout and proxy settings.
*   **Clear Abstractions**: Provides clear separation for API v1 (`OsuClient`) and API v2 (`OsuClientV2`) functionalities.

## Installation

You can install Coosu.Api via NuGet Package Manager:

```bash
Install-Package Coosu.Api
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Api
```

## Usage

### osu! API v1

To use API v1, you need an API key, which you can obtain from [here](https://osu.ppy.sh/p/api).

```csharp
using Coosu.Api.V1;
using Coosu.Api.V1.Beatmap;
using Coosu.Api.V1.Score;
using Coosu.Api.V1.User;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ApiV1Example
{
    private readonly OsuClient _client;

    public ApiV1Example(string apiKey)
    {
        _client = new OsuClient(apiKey);
    }

    public async Task GetUserDataAsync(string userName)
    {
        // Get user data
        OsuUser[] users = await _client.GetUsers(UserComponent.FromUserName(userName));
        OsuUser user = users.FirstOrDefault();

        if (user != null)
        {
            Console.WriteLine($"User: {user.Username} (ID: {user.UserId})");
            Console.WriteLine($"PP: {user.PpRaw}, Rank: {user.PpRank}");

            // Using UserExtra for more info
            var userExtra = new UserExtra(user);
            Console.WriteLine($"Profile URL: {userExtra.UserPageUri}");
            Console.WriteLine($"Avatar URL: {userExtra.AvatarUri}");
        }
    }

    public async Task GetBeatmapDataAsync(int beatmapId)
    {
        // Get beatmap data
        OsuBeatmap beatmap = await _client.GetBeatmap(new BeatmapId(beatmapId));
        if (beatmap != null)
        {
            Console.WriteLine($"Beatmap: {beatmap.Artist} - {beatmap.Title} [{beatmap.Version}]");
            Console.WriteLine($"Creator: {beatmap.Creator}, BPM: {beatmap.Bpm}");

            var beatmapExtra = new BeatmapExtra(beatmap);
            Console.WriteLine($"Thumbnail: {beatmapExtra.ThumbnailUri}");
        }
    }

    public async Task GetScoresAsync(int beatmapId, string userName)
    {
        // Get top scores for a beatmap
        OsuPlayScore[] scores = await _client.GetScores(new BeatmapId(beatmapId),
                                                      UserComponent.FromUserName(userName),
                                                      limitCount: 5);
        Console.WriteLine($"\nTop 5 scores for beatmap {beatmapId} by {userName}:");
        foreach (var score in scores)
        {
            Console.WriteLine($"  Score: {score.Score}, Combo: {score.MaxCombo}, Mods: {score.EnabledMods}");
        }
    }

    // ... other examples for GetUserBestScores, GetUserRecentScores, GetMatch, GetReplay ...
}
```

*(Refer to the original README content for more API v1 examples, including `ReplayExtra` usage.)*

### osu! API v2

API v2 uses OAuth 2.0 for authentication. You'll need to register an application [here](https://osu.ppy.sh/home/account/edit#oauth) to get a Client ID and Client Secret.

**1. Authorization (User-based access):**

```csharp
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using System;
using System.Threading.Tasks; // For Task

public class ApiV2AuthorizationExample
{
    private readonly int _clientId = YOUR_CLIENT_ID; // Replace with your client ID
    private readonly string _clientSecret = "YOUR_CLIENT_SECRET"; // Replace with your client secret
    private readonly Uri _redirectUri = new Uri("YOUR_REDIRECT_URI"); // Replace with your redirect URI

    public string GetAuthorizationUrl()
    {
        var authBuilder = new AuthorizationLinkBuilder(_clientId, _redirectUri);
        // Request necessary scopes
        authBuilder.AddScope(AuthorizationScope.Identify | AuthorizationScope.Public | AuthorizationScope.Friends_Read);
        return authBuilder.Build();
    }

    public async Task ExchangeCodeForTokenAsync(string authorizationCode)
    {
        UserToken token = await TokenClient.RequestTokenAsync(new AuthorizationCodeTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            RedirectUri = _redirectUri.OriginalString,
            Code = authorizationCode
        });

        Console.WriteLine($"Access Token: {token.AccessToken}");
        Console.WriteLine($"Refresh Token: {token.RefreshToken}");
        Console.WriteLine($"Expires In: {token.ExpiresIn} seconds");

        // Store the token securely and use it for API calls
        OsuClientV2 client = new OsuClientV2(token);
        // ... make API calls using client.User, client.Beatmap etc.
        var me = await client.User.GetOwnData();
        Console.WriteLine($"My username: {me.Username}");
    }

    public async Task RefreshTokenAsync(string refreshTokenValue)
    {
        UserToken newToken = await TokenClient.RefreshTokenAsync(new RefreshTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            RefreshToken = refreshTokenValue
        });
        Console.WriteLine($"New Access Token: {newToken.AccessToken}");
        // Update your stored token
    }
}
```

**2. Client Credentials Grant (Application-level access):**

```csharp
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using System.Threading.Tasks;

public class ApiV2ClientCredentialsExample
{
    private readonly int _clientId = YOUR_CLIENT_ID;
    private readonly string _clientSecret = "YOUR_CLIENT_SECRET";

    public async Task GetPublicTokenAsync()
    {
        ClientCredentialsToken token = await TokenClient.RequestTokenAsync(new ClientCredentialsTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            Scope = AuthorizationScope.Public // Or other scopes like Bot
        });

        Console.WriteLine($"Client Access Token: {token.AccessToken}");

        OsuClientV2 client = new OsuClientV2(token);
        // Now you can make API calls that require public access or specific grant types
        // Example: Get beatmap details
        var beatmap = await client.Beatmap.GetBeatmap(129891); // Example beatmap ID
        if (beatmap != null)
        {
            Console.WriteLine($"Beatmap Title: {beatmap.Beatmapset.Title}");
        }
    }
}
```

**Making API v2 Calls:**

Once you have an `OsuClientV2` instance initialized with a valid token:

```csharp
// Assuming 'client' is an initialized OsuClientV2 instance

// Get own user data (requires 'identify' scope)
var myData = await client.User.GetOwnData(Coosu.Api.V2.GameMode.Osu);
Console.WriteLine($"My username is: {myData.Username}, from {myData.Country.Name}");

// Get another user's data
var userData = await client.User.GetUser(7276846); // Example user ID (ppy)
if (userData != null)
{
    Console.WriteLine($"User: {userData.Username}, Last Visit: {userData.LastVisit}");
}

// Get beatmap set discussions
var discussions = await client.Beatmap.GetBeatmapsetDiscussions(beatmapsetId: 1001546); // Example beatmapset ID
if (discussions != null)
{
    Console.WriteLine($"Found {discussions.Discussions.Length} discussions for the beatmapset.");
}

// ... many other endpoints are available under client.User, client.Beatmap, etc.
```

## Contributing

Contributions are welcome! If you find any bugs, have feature requests, or want to contribute to the code, please feel free to open an issue or submit a pull request on the GitHub repository.

## License

Coosu.Api is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
