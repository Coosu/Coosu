using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Coosu.Api.HttpClient;

internal class HttpClientUtility
{
    private enum RequestMethod
    {
        Get, Post, Put, Delete
    }

    static HttpClientUtility()
    {
        ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) => true;
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            foreach (var httpClient in HttpClients)
            {
                httpClient.Value?.Dispose();
            }
        };
    }

    private static readonly ConcurrentDictionary<string, System.Net.Http.HttpClient> HttpClients = new();

    private readonly ClientOptions _clientOptions;
    private readonly System.Net.Http.HttpClient _httpClient;
    private AuthenticationHeaderValue? _authorization;

    public HttpClientUtility(ClientOptions? clientCreationOptions = null)
    {
        _clientOptions = clientCreationOptions ??= new ClientOptions();
        var standardizedUri = clientCreationOptions.ProxyUrl == null
            ? ""
            : new Uri(clientCreationOptions.ProxyUrl).AbsoluteUri;

        _httpClient = HttpClients.GetOrAdd(standardizedUri, proxyUri =>
        {
            HttpMessageHandler handler;
            if (string.IsNullOrEmpty(proxyUri))
            {
                handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };
            }
            else
            {
#if NET6_0_OR_GREATER
                handler = new SocketsHttpHandler
                {
                    Proxy = new WebProxy(clientCreationOptions.ProxyUrl),
                    AutomaticDecompression = DecompressionMethods.GZip
                };
#else
                var httpClientHandler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip,
                };
                var uri = new Uri(proxyUri);
                httpClientHandler.Proxy = new MihaZupan.HttpToSocks5Proxy(uri.Host, uri.Port);
                handler = httpClientHandler;
#endif
            }

            return new System.Net.Http.HttpClient(handler) { Timeout = clientCreationOptions.Timeout };
        });
    }

    public void SetDefaultAuthorization(string scheme, string parameter)
    {
        _authorization = new AuthenticationHeaderValue(scheme, parameter);
    }

    public async Task<T> HttpGet<T>(
        string uri,
        IReadOnlyDictionary<string, string>? queries = null,
        IReadOnlyDictionary<string, string>? headers = null)
    {
        return await SendAsync<T>(uri, queries, null, headers, RequestMethod.Get);
    }

    /// <summary>
    /// DELETE with value-pairs.
    /// </summary>
    /// <param name="url">Http uri.</param>
    /// <param name="queries">Parameter dictionary.</param>
    /// <param name="headers">Header dictionary.</param>
    /// <returns></returns>
    public async Task<T> HttpDelete<T>(
        string url,
        IReadOnlyDictionary<string, string>? queries = null,
        IReadOnlyDictionary<string, string>? headers = null)
    {
        return await SendAsync<T>(url, queries, null, headers, RequestMethod.Delete);
    }

    /// <summary>
    /// POST with Json.
    /// </summary>
    /// <param name="url">Http uri.</param>
    /// <param name="obj">object</param>
    /// <param name="headers">Header dictionary.</param>
    /// <returns></returns>
    public async Task<T> HttpPost<T>(string url, object obj,
        IReadOnlyDictionary<string, string>? headers = null)
    {
        HttpContent content = new StringContent(JsonSerializer.Serialize(obj));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await SendAsync<T>(url, null, content, headers, RequestMethod.Post);
    }

    /// <summary>
    /// POST with Json.
    /// </summary>
    /// <param name="url">Http uri.</param>
    /// <param name="obj">object</param>
    /// <param name="headers">Header dictionary.</param>
    /// <returns></returns>
    public async Task<T> HttpPut<T>(string url, object obj,
        IReadOnlyDictionary<string, string>? headers = null)
    {
        HttpContent content = new StringContent(JsonSerializer.Serialize(obj));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await SendAsync<T>(url, null, content, headers, RequestMethod.Put);
    }

    private async Task<T> SendAsync<T>(
        string url,
        IReadOnlyDictionary<string, string>? args,
        HttpContent? content,
        IReadOnlyDictionary<string, string>? argsHeader,
        RequestMethod requestMethod)
    {
        var context = new RequestContext(url + BuildQueries(args));
        return await RunWithRetry(context, async () =>
        {
            var uri = context.RequestUri;
            var request = requestMethod switch
            {
                RequestMethod.Get => new HttpRequestMessage(HttpMethod.Get, uri),
                RequestMethod.Delete => new HttpRequestMessage(HttpMethod.Delete, uri),
                RequestMethod.Post => new HttpRequestMessage(HttpMethod.Post, uri),
                RequestMethod.Put => new HttpRequestMessage(HttpMethod.Put, uri),
                _ => throw new ArgumentOutOfRangeException(nameof(requestMethod), requestMethod, null)
            };

            if (content != null)
            {
                request.Content = content;
            }

            if (_authorization != null)
            {
                request.Headers.Authorization = _authorization;
            }

            if (argsHeader != null)
            {
                foreach (var kvp in argsHeader)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }

            HttpResponseMessage response;
            using (var cts = new CancellationTokenSource(_clientOptions.Timeout))
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    cts.Token);
            }

            try
            {
                if (response.RequestMessage is { RequestUri: { } })
                    context.RequestUri = response.RequestMessage.RequestUri.ToString();
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                return (await JsonSerializer.DeserializeAsync<T>(responseStream))!;
            }
            catch (Exception ex)
            {
#if DEBUG
                var text = await response.Content.ReadAsStringAsync();
#endif
                throw new Exception("Server responded: " + text, ex);
            }
            finally
            {
                response.Dispose();
            }
        });
    }

    private async Task<T> RunWithRetry<T>(RequestContext context, Func<Task<T>> func)
    {
        for (int i = 0; i < _clientOptions.RetryCount; i++)
        {
            var uri = context.RequestUri;
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                if (context.RequestUri != uri)
                {
                    i--;
                }
                else
                {
                    Debug.WriteLine(string.Format("Tried {0} time{1}. (>{2}ms): {3}",
                        i + 1,
                        i + 1 > 1 ? "s" : "",
                        _clientOptions.Timeout,
                        context.RequestUri)
                    );
                }

                if (ex is HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StackTrace?.Contains("EnsureSuccessStatusCode") == true)
                    {
                        throw;
                    }
                }

                if (i == _clientOptions.RetryCount - 1)
                    throw;
            }
        }

        throw new Exception("HttpRequest not success");
    }

    private static string? BuildQueries(IReadOnlyDictionary<string, string>? args)
    {
        if (args == null || args.Count < 1)
            return null;

        var sb = new StringBuilder("?");
        int i = 0;
        foreach (var kvp in args)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            if (i > 0) sb.Append('&');

            sb.Append(HttpUtils.UrlEncode(key));
            sb.Append('=');
            sb.Append(HttpUtils.UrlEncode(value));
            i++;
        }

        return sb.ToString();
    }
}