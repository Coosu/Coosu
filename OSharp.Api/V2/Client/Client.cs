using System;
using Newtonsoft.Json;

namespace OSharp.Api.V2.Client
{
    /// <summary>
    /// OSU API V2 client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client ID.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Owner of the client.
        /// </summary>
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// Client name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Client secret.
        /// </summary>
        [JsonProperty("secret")]
        public string Secret { get; set; }

        /// <summary>
        /// Specified redirect URI after authorization. (URL encoded)
        /// </summary>
        [JsonProperty("redirect")]
        public Uri Redirect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("personal_access_client")]
        public bool PersonalAccessClient { get; set; }

        /// <summary>
        /// Whether the client uses password.
        /// </summary>
        [JsonProperty("password_client")]
        public bool PasswordClient { get; set; }

        /// <summary>
        /// Whether the client is revoked.
        /// </summary>
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }

        /// <summary>
        /// Latest updating time of the client.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        /// <summary>
        /// Creation time of the client.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }
}
