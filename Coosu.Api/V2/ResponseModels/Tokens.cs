using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public class PublicToken : TokenBase
    {
    }

    public class UserToken : TokenBase
    {
        /// <summary>
        /// The refresh token.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public abstract class TokenBase
    {
        /// <summary>
        /// The type of token, this should always be <c>Bearer</c>
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The number of seconds the token will be valid for.
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// The access token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Create time of access token.
        /// </summary>
        [JsonProperty("create_time")]
        public DateTimeOffset? CreateTime { get; set; }
    }
}
