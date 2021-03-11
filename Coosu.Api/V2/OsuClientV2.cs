using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;

namespace Coosu.Api.V2
{
    public class OsuClientV2
    {
        internal const string BaseUri = "https://osu.ppy.sh/api/v2";

        private readonly TokenBase _token;
        private HttpClientUtility _httpClient;

        public OsuClientV2(TokenBase token, ClientOptions clientOptions = null)
        {
            clientOptions = clientOptions ?? new ClientOptions();

            _token = token;
            _httpClient = new HttpClientUtility(clientOptions.Socks5ProxyOptions);
            _httpClient.SetDefaultAuthorization(_token.TokenType, _token.AccessToken);
            User = new UserEndpoint(token, _httpClient);
            Beatmap = new BeatmapEndpoint(token, _httpClient);
        }

        public UserEndpoint User { get; }
        public BeatmapEndpoint Beatmap { get; }
    }
}
