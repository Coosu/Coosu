using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace OSharp.Api.HttpClient
{
    /// <summary>
    /// Helper class of HttpClient.
    /// To increase the efficiency, please consider to initialize this class infrequently.
    /// </summary>
    internal class HttpClientUtility
    {
        public static readonly string CacheImagePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "imageCache");

        public TimeSpan Timeout { get; set; }

        public int RetryCount { get; set; }

        private readonly System.Net.Http.HttpClient _httpClient;

        public HttpClientUtility() : this(TimeSpan.FromSeconds(8), 3)
        {
        }

        public HttpClientUtility(TimeSpan timeout) : this(timeout, 3)
        {
        }

        public HttpClientUtility(TimeSpan timeout, int retryCount)
        {
            Timeout = timeout;
            RetryCount = retryCount;
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            _httpClient = new System.Net.Http.HttpClient(handler)
            {
                Timeout = Timeout
            };
        }

        /// <summary>
        /// GET with value-pairs.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <param name="args">Parameter dictionary.</param>
        /// <param name="argsHeader">Header dictionary.</param>
        /// <returns></returns>
        public string HttpGet(
            string url,
            IDictionary<string, string> args = null,
            IDictionary<string, string> argsHeader = null)
        {
            return GetResult(url, args, argsHeader, RequestMethod.Get);
        }

        /// <summary>
        /// DELETE with value-pairs.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <param name="args">Parameter dictionary.</param>
        /// <param name="argsHeader">Header dictionary.</param>
        /// <returns></returns>
        public string HttpDelete(
            string url,
            IDictionary<string, string> args = null,
            IDictionary<string, string> argsHeader = null)
        {
            return GetResult(url, args, argsHeader, RequestMethod.Delete);
        }

        /// <summary>
        /// POST with nothing.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <returns></returns>
        public string HttpPost(string url)
        {
            HttpContent content = new StringContent("");
            content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Form.GetContentType());
            return HttpRequest(url, content, RequestMethod.Post);
        }

        /// <summary>
        /// POST with Json.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <param name="postJson">json string.</param>
        /// <returns></returns>
        public string HttpPostJson(string url, string postJson)
        {
            HttpContent content = new StringContent(postJson);
            content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Json.GetContentType());
            return HttpRequest(url, content, RequestMethod.Post);
        }

        /// <summary>
        /// POST with Json.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <param name="args">Parameter dictionary.</param>
        /// <param name="argsHeader">Header dictionary.</param>
        /// <returns></returns>
        public string HttpPostJson(string url,
            IDictionary<string, string> args = null,
            IDictionary<string, string> argsHeader = null)
        {
            HttpContent content = GetHttpContent(HttpContentType.Json, args, argsHeader, true);
            return HttpRequest(url, content, RequestMethod.Post);
        }

        /// <summary>
        /// PUT with Json.
        /// </summary>
        /// <param name="url">Http uri.</param>
        /// <param name="args">Parameter dictionary.</param>
        /// <param name="argsHeader">Header dictionary.</param>
        /// <returns></returns>
        public string HttpPutJson(string url,
            IDictionary<string, string> args = null,
            IDictionary<string, string> argsHeader = null)
        {
            HttpContent content = GetHttpContent(HttpContentType.Json, args, argsHeader, true);
            return HttpRequest(url, content, RequestMethod.Put);
        }

        private static HttpContent GetHttpContent(
            HttpContentType contentType,
            IDictionary<string, string> args,
            IDictionary<string, string> argsHeader,
            bool json)
        {
            HttpContent content;
            if (args != null)
            {
                if (json)
                {
                    var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(args);
                    content = new StringContent(jsonStr);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType.GetContentType());
                }
                else
                {
                    content = new StringContent(string.Join("&", args.Select(k => $"{k.Key}={k.Value}")));
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType.GetContentType());
                }
            }
            else
            {
                content = new StringContent("");
                content.Headers.ContentType = new MediaTypeHeaderValue(HttpContentType.Form.GetContentType());
            }

            if (argsHeader != null)
            {
                foreach (var item in argsHeader)
                    content.Headers.Add(item.Key, item.Value);
            }

            return content;
        }

        private string GetResult(
            string url,
            IDictionary<string, string> args,
            IDictionary<string, string> argsHeader,
            RequestMethod requestMethod)
        {
            string responseStr = null;
            string fullUrl = url + args?.ToUrlParamString();

            for (int i = 0; i < RetryCount; i++)
            {
                HttpRequestMessage message = null;
                try
                {
                    switch (requestMethod)
                    {
                        case RequestMethod.Get:
                            message = new HttpRequestMessage(HttpMethod.Get, fullUrl);
                            break;
                        case RequestMethod.Delete:
                            message = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
                            break;
                        case RequestMethod.Post:
                        case RequestMethod.Put:
                            throw new NotSupportedException();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(requestMethod), requestMethod, null);
                    }

                    if (argsHeader != null)
                    {
                        foreach (var item in argsHeader)
                        {
                            message.Headers.Add(item.Key, item.Value);
                        }
                    }

                    HttpResponseMessage response;
                    using (var cts = new CancellationTokenSource(Timeout))
                    {
                        response = _httpClient.SendAsync(message, cts.Token).Result;
                    }

                    responseStr = response.Content.ReadAsStringAsync().Result;
                    return responseStr;
                }
                catch (Exception)
                {
                    Debug.WriteLine(string.Format("Tried {0} time{1} for timed out. (>{2}ms): {3}",
                        i + 1,
                        i + 1 > 1 ? "s" : "",
                        Timeout,
                        fullUrl)
                    );
                    if (i == RetryCount - 1)
                        throw;
                }
                finally
                {
                    message?.Dispose();
                }
            }

            return responseStr;
        }

        private string HttpRequest(string url, HttpContent content, RequestMethod requestMethod)
        {
            string responseStr = null;
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    HttpResponseMessage response;
                    switch (requestMethod)
                    {
                        case RequestMethod.Post:
                            response = _httpClient.PostAsync(url, content).Result;
                            break;
                        case RequestMethod.Put:
                            response = _httpClient.PutAsync(url, content).Result;
                            break;
                        case RequestMethod.Get:
                        case RequestMethod.Delete:
                            throw new NotSupportedException();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(requestMethod), requestMethod, null);
                    }
                    // ensure if the request is success.
                    response.EnsureSuccessStatusCode();
                    // read the Json asynchronously.

                    // notice currently was auto decompressed with GZip,
                    // because AutomaticDecompression was set to DecompressionMethods.GZip
                    responseStr = response.Content.ReadAsStringAsync().Result;
                    return responseStr;
                }
                catch (Exception)
                {
                    Debug.WriteLine(string.Format("Tried {0} time{1} for timed out. (>{2}ms): {3}",
                        i + 1,
                        i + 1 > 1 ? "s" : "",
                        Timeout,
                        url)
                    );
                    if (i == RetryCount - 1)
                        throw;
                }
            }

            return responseStr;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; // always accept.  
        }
    }
}
