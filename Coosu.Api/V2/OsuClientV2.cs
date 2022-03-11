using System;
using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;

namespace Coosu.Api.V2
{
    public class OsuClientV2
    {
        internal const string BaseUri = "https://osu.ppy.sh/api/v2";

        public OsuClientV2(TokenBase token, ClientOptions? clientOptions = null)
        {
            var httpClient = new HttpClientUtility(clientOptions);
            httpClient.SetDefaultAuthorization(token.TokenType, token.AccessToken);
            User = new UserEndpoint(token, httpClient);
            Beatmap = new BeatmapEndpoint(token, httpClient);
        }

        public UserEndpoint User { get; }
        public BeatmapEndpoint Beatmap { get; }
    }
}
