using System;
using System.Collections.Generic;
using System.Linq;

namespace OSharp.Api.V2.Authorization
{
    /// <summary>
    /// Access scope option.
    /// </summary>
    [Flags]
    public enum AuthorizationScope
    {
        /// <summary>
        /// Access all scope.
        /// </summary>
        All = 0x0,
        /// <summary>
        /// Access friends-read scope.
        /// </summary>
        FriendsRead = 0x1,
        /// <summary>
        /// Access identify scope.
        /// </summary>
        Identify = 0x2
    }

    /// <summary>
    /// AuthorizationScope enum extensions.
    /// </summary>
    public static class AuthorizationScopeExtension
    {
        private static readonly IReadOnlyDictionary<AuthorizationScope, string> AuthDictionary =
            new Dictionary<AuthorizationScope, string>
            {
                [AuthorizationScope.FriendsRead] = "friends.read",
                [AuthorizationScope.Identify] = "identify",
            };

        /// <summary>
        /// Get the api-supported string array of the specified scope option.
        /// </summary>
        /// <param name="scopeOption">Access scope option.</param>
        /// <returns></returns>
        public static string[] GetRequestArray(this AuthorizationScope scopeOption)
        {
            return scopeOption.HasFlag(AuthorizationScope.All)
                ? AuthDictionary.Select(k => k.Value).ToArray()
                : GetFlags<AuthorizationScope>(scopeOption).Select(scope => AuthDictionary[scope]).ToArray();
        }

        private static IEnumerable<T> GetFlags<T>(Enum input) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return (T)value;
        }
    }
}
