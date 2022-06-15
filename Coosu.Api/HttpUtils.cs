using System;
using System.Web;

namespace Coosu.Api;

internal class HttpUtils
{
    public static string UrlEncode(string key)
    {
        return key.Length < 65520 ? Uri.EscapeDataString(key) : HttpUtility.UrlEncode(key);
    }
}