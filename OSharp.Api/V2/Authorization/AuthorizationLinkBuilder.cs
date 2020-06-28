using System;
using System.Text;

namespace OSharp.Api.V2.Authorization
{
    /// <summary>
    /// Authorization URI builder.
    /// For more details, see: https://github.com/int-and-his-friends/osu-api-v2/wiki/Authorization-code
    /// </summary>
    public class AuthorizationLinkBuilder
    {
        private readonly int _clientId;
        private const string RequestLink = "https://osu.ppy.sh/oauth/authorize";

        /// <summary>
        /// Initialize authorization link builder with client ID.
        /// </summary>
        /// <param name="clientId">Your API V2 client ID.</param>
        public AuthorizationLinkBuilder(int clientId)
        {
            _clientId = clientId;
        }

        /// <summary>
        /// Build Authorization URI.
        /// </summary>
        /// <param name="redirectUri">Client redirect URI (URL encoded)</param>
        /// <param name="tag">Identifying tag. This tag will be transmitted both sent link and callback link.</param>
        /// <param name="scope">Access scope option.</param>
        /// <returns>Generated user authorization link.</returns>
        public Uri BuildLink(Uri redirectUri, string tag, AuthorizationScope scope)
        {
            var sb = new StringBuilder($"{RequestLink}?");

            string responseType = "code";

            sb.Append($"response_type={responseType}");
            sb.Append($"&client_id={_clientId}");
            sb.Append($"&redirect_uri={redirectUri.AbsoluteUri}");
            sb.Append($"&state={tag}");
            sb.Append($"&scope={string.Join(" ", scope.GetRequestArray())}");

            return new Uri(sb.ToString());
        }
    }
}
