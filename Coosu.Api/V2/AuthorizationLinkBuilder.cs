using System;
using System.Text;
using System.Web;

namespace Coosu.Api.V2;

/// <summary>
/// Authorization URI builder.
/// For more details, see: https://github.com/int-and-his-friends/osu-api-v2/wiki/Authorization-code
/// </summary>
public class AuthorizationLinkBuilder
{
    private readonly int _clientId;
    private readonly Uri _registeredRedirectUri;
    private const string AuthorizationLink = "https://osu.ppy.sh/oauth/authorize";
    private const string TokenLink = "https://osu.ppy.sh/oauth/token";

    /// <summary>
    /// Initialize authorization link builder with client ID.
    /// </summary>
    /// <param name="clientId">Your API V2 client ID.</param>
    /// <param name="registeredRedirectUri">Registered Application Callback URL</param>
    public AuthorizationLinkBuilder(int clientId, Uri registeredRedirectUri)
    {
        _clientId = clientId;
        _registeredRedirectUri = registeredRedirectUri;
    }

    /// <summary>
    /// Build Authorization URI.
    /// </summary>
    /// <param name="state">Identifying tag. This tag will be transmitted both sent link and callback link.</param>
    /// <param name="scope">Access scope option.</param>
    /// <returns>Generated user authorization link.</returns>
    public Uri BuildAuthorizationLink(string state, AuthorizationScope scope)
    {
        var sb = new StringBuilder($"{AuthorizationLink}?");

        string responseType = "code";

        sb.Append($"client_id={_clientId}");
        sb.Append($"&redirect_uri={HttpUtils.UrlEncode(_registeredRedirectUri.OriginalString)}");
        sb.Append($"&response_type={responseType}");
        sb.Append($"&scope={HttpUtils.UrlEncode(string.Join(" ", scope.GetRequestArray()))}");
        sb.Append($"&state={HttpUtils.UrlEncode(state)}");

        return new Uri(sb.ToString());
    }

    /// <summary>
    /// Build GetUserToken() URI.
    /// </summary>
    /// <param name="clientSecret">The client secret of your application.</param>
    /// <param name="code">The code you received.</param>
    /// <returns>Generated GetUserToken() link.</returns>
    public Uri BuildAuthorizationTokenLink(string clientSecret, string code)
    {
        var sb = new StringBuilder($"{TokenLink}?");

        sb.Append($"&client_id={_clientId}");
        sb.Append($"&client_secret={HttpUtils.UrlEncode(clientSecret)}");
        sb.Append($"&code={HttpUtils.UrlEncode(code)}");
        sb.Append($"&grant_type=authorization_code");
        sb.Append($"&redirect_uri={HttpUtils.UrlEncode(_registeredRedirectUri.OriginalString)}");

        return new Uri(sb.ToString());
    }

    /// <summary>
    /// Build GetPublicToken() URI.
    /// </summary>
    /// <param name="clientSecret">The client secret of your application.</param>
    /// <returns>Generated GetUserToken() link.</returns>
    public Uri BuildPublicTokenLink(string clientSecret)
    {
        var sb = new StringBuilder($"{TokenLink}?");

        sb.Append($"&client_id={_clientId}");
        sb.Append($"&client_secret={HttpUtils.UrlEncode(clientSecret)}");
        sb.Append($"&grant_type=client_credentials");
        sb.Append($"&scope=public");

        return new Uri(sb.ToString());
    }

    /// <summary>
    /// Build RefreshToken() URI.
    /// </summary>
    /// <param name="clientSecret">The client secret of your application.</param>
    /// <returns>Generated RefreshToken() link.</returns>
    public Uri BuildRefreshUserTokenLink(string clientSecret, string refreshToken)
    {
        var sb = new StringBuilder($"{TokenLink}?");

        sb.Append($"&client_id={_clientId}");
        sb.Append($"&client_secret={HttpUtils.UrlEncode(clientSecret)}");
        //sb.Append($"&code={code}");
        sb.Append($"&grant_type=refresh_token");
        sb.Append($"&refresh_token={HttpUtils.UrlEncode(refreshToken)}");

        return new Uri(sb.ToString());
    }
}