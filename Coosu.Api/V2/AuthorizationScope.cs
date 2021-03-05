using System;
using System.Collections.Generic;
using System.Linq;

namespace Coosu.Api.V2.Authorization
{
    /// <summary>
    /// Access scope option.
    /// </summary>
    [Flags]
    public enum AuthorizationScope
    {
        /// <summary>
        /// <a href="https://osu.ppy.sh/wiki/en/Bot_account">Chat Bot</a> and
        /// <a href="https://osu.ppy.sh/docs/index.html#client-credentials-grant">Client Credentials</a> Grant exclusive scope.
        /// </summary>
        Bot = 0xb1,

        /// <summary>
        /// Allows sending chat messages on a user's behalf;
        /// exclusive to <a href="https://osu.ppy.sh/wiki/en/Bot_account">Chat Bot</a>s
        /// </summary>
        Chat_Write = 0xb10,

        /// <summary>
        /// Allows reading of the user's friend list.
        /// </summary>
        Friends_Read = 0xb100,

        /// <summary>
        /// Allows reading of the public profile of the user (<code>/me</code>).
        /// </summary>
        Identify = 0xb1000,

        /// <summary>
        /// Allows reading of publicly available data on behalf of the user.
        /// </summary>
        Public = 0xb10000
    }

    /// <summary>
    /// AuthorizationScope enum extensions.
    /// </summary>
    public static class AuthorizationScopeExtension
    {
        /// <summary>
        /// Get the api-supported string array of the specified scope option.
        /// </summary>
        /// <param name="scopeOption">Access scope option.</param>
        /// <returns></returns>
        public static string[] GetRequestArray(this AuthorizationScope scopeOption)
        {
            return GetFlags(scopeOption).Select(GetScopeString).ToArray();
        }

        private static IEnumerable<T> GetFlags<T>(T input) where T : Enum
        {
            return Enum.GetValues(input.GetType())
                .Cast<Enum>()
                .Where(input.HasFlag)
                .Select(value => (T)value);
        }

        private static string GetScopeString(AuthorizationScope scope)
        {
            switch (scope)
            {
                case AuthorizationScope.Bot:
                    return "bot";
                case AuthorizationScope.Chat_Write:
                    return "chat.write";
                case AuthorizationScope.Friends_Read:
                    return "friends.read";
                case AuthorizationScope.Identify:
                    return "identify";
                case AuthorizationScope.Public:
                    return "public";
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
        }
    }
}