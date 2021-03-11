namespace Coosu.Api.HttpClient
{
    public class Socks5ProxyOptions
    {
        public bool UseSocks5Proxy { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}