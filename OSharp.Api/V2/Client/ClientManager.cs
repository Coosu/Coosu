using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OSharp.Api.HttpClient;

namespace OSharp.Api.V2.Client
{
    /// <summary>
    /// Manager of osu! api v2 Client.
    /// For more details, see: https://github.com/int-and-his-friends/osu-api-v2/wiki/Oauth-clients
    /// </summary>
    public class ClientManager
    {
        private static readonly HttpClientUtility HttpClient = new HttpClientUtility();

        /// <summary>
        /// Get the CSRF Token.
        /// </summary>
        public string CsrfToken { get; }

        /// <summary>
        /// Get the osu session.
        /// </summary>
        public string OsuSession { get; }

        private const string ClientLink = "https://osu.ppy.sh/oauth/clients";
        //private const string TokenKey = "XSRF-TOKEN";
        private const string TokenKey = "X-CSRF-TOKEN";

        /// <summary>
        /// Initialize a client manager.
        /// Grab the headers on the web page with user logged in: https://osu.ppy.sh/oauth/clients.
        /// </summary>
        /// <param name="csrfToken">CSRF token.</param>
        /// <param name="osuSession">Session in the cookie. This can be ignored after the first success request.</param>
        public ClientManager(string csrfToken, string osuSession)
        {
            CsrfToken = csrfToken;
            OsuSession = osuSession;
        }

        /// <summary>
        /// Initialize a client manager.
        /// </summary>
        /// <param name="csrfToken">CSRF token.</param>
        public ClientManager(string csrfToken)
        {
            CsrfToken = csrfToken;
        }

        /// <summary>
        /// Create a client.
        /// </summary>
        /// <param name="name">Client name.</param>
        /// <param name="redirectUri">Redirect URI.</param>
        /// <returns>OSU API V2 client.</returns>
        public Client CreateClient(string name, Uri redirectUri)
        {
            string json = HttpClient.HttpPostJson(
                url: ClientLink,
                args: new Dictionary<string, string>
                {
                    ["name"] = name,
                    ["redirect"] = redirectUri.AbsoluteUri
                },
                argsHeader: new Dictionary<string, string>
                {
                    [TokenKey] = CsrfToken,
                    ["Cookie"] = $"osu_session={OsuSession};",
                }
            );
            return JsonConvert.DeserializeObject<Client>(json);
        }

        /// <summary>
        /// Get clients which were created.
        /// </summary>
        public void GetClients()
        {
            string json = HttpClient.HttpGet(
                url: ClientLink,
                argsHeader: new Dictionary<string, string>
                {
                    [TokenKey] = CsrfToken,
                }
            );
        }

        /// <summary>
        /// Edit specified client which is created.
        /// </summary>
        /// <param name="clientId">Client ID.</param>
        /// <param name="newName">New client name.</param>
        /// <param name="newRedirectUri">New redirect URI.</param>
        public void EditClient(int clientId, string newName, Uri newRedirectUri)
        {
            string json = HttpClient.HttpPutJson(
                url: $"{ClientLink}/{clientId}",
                args: new Dictionary<string, string>
                {
                    ["name"] = newName,
                    ["redirect"] = newRedirectUri.AbsoluteUri
                },
                argsHeader: new Dictionary<string, string>
                {
                    [TokenKey] = CsrfToken,
                }
            );
        }

        /// <summary>
        /// Revoke specified client.
        /// </summary>
        /// <param name="clientId">Client ID.</param>
        public void RevokeClient(int clientId)
        {
            string json = HttpClient.HttpDelete(
                url: $"{ClientLink}/{clientId}",
                argsHeader: new Dictionary<string, string>
                {
                    [TokenKey] = CsrfToken,
                }
            );
        }
    }
}
