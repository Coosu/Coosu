using Coosu.Api.HttpClient;

namespace Coosu.Api.V2
{
    public class ClientOptions
    {
        public Socks5ProxyOptions Socks5ProxyOptions { get; set; } = new Socks5ProxyOptions();
    }
}