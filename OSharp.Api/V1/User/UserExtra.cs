using System;
using OSharp.Api.V1.Internal;

namespace OSharp.Api.V1.User
{ 
    /// <summary>
    /// User extra metadata and methods.
    /// </summary>
    public class UserExtra
    {
        private readonly OsuUser _user;

        /// <summary>
        /// Initialize user extra class.
        /// </summary>
        /// <param name="user">Specified user.</param>
        public UserExtra(OsuUser user)
        {
            _user = user;
        }

        /// <summary>
        /// Get URI of the user page.
        /// </summary>
        public Uri UserPageUri => new Uri($"{Link.UserPageUri}{_user.UserId}");

        /// <summary>
        /// Get avatar image URI of the user.
        /// </summary>
        public Uri AvatarUri => new Uri($"{Link.AvatarUri}{_user.UserId}");

        /// <summary>
        /// Get flag image URI of the user.
        /// </summary>
        public Uri FlagUri => new Uri($"{Link.FlagUri}{_user.Country.ToLower()}.gif");

        /// <summary>
        /// Get URI of the user's osu!status page.
        /// </summary>
        public Uri OsuStatsUri => new Uri($"{Link.OsuStatsUri}{_user.UserName}");

        /// <summary>
        /// Get URI of the user's osu!track page.
        /// </summary>
        public Uri OsuTrackUri => new Uri($"{Link.OsuTrackUri}{_user.UserName}n");

        /// <summary>
        /// Get URI of the user's osu!status page.
        /// </summary>
        public Uri OsuSkillsUri => new Uri($"{Link.OsuSkillsUri}{_user.UserName}");

        /// <summary>
        /// Get URI of the user's osu!chan page.
        /// </summary>
        public Uri OsuChanUri => new Uri($"{Link.OsuChanUri}{_user.UserId}");

        /// <summary>
        /// Get URI of the user's PP Plus page.
        /// </summary>
        public Uri PpPlusUri => new Uri($"{Link.PpPlusUri}{_user.UserId}");
    }
}