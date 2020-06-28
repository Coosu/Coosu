using System;
using System.Collections.Generic;
using System.Text;

namespace OSharp.Api.HttpClient
{
    internal static class HttpUtility
    {
        public static string ToUrlParamString(this IDictionary<string, string> args)
        {
            if (args == null || args.Count <= 1)
                return "";
            StringBuilder sb = new StringBuilder("?");
            foreach (var item in args)
                sb.Append(item.Key + "=" + item.Value + "&");
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static string GetContentType(this HttpContentType type)
        {
            switch (type)
            {
                case HttpContentType.Json:
                    return "application/json";
                case HttpContentType.Form:
                    return "application/x-www-form-urlencoded";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
