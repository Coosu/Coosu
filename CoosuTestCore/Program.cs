using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;

namespace CoosuTestCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var redirectUri = new Uri("");
            //var sb = new AuthorizationLinkBuilder(5044, redirectUri);
            //var uri = sb.BuildAuthorizationLink("2241521134", AuthorizationScope.Public
            //                                              | AuthorizationScope.Identify);
            //var o = uri.ToString();

            //var code = "";
            //var clientSecret = "";
            //var uri2 = sb.BuildAuthorizationTokenLink(clientSecret, code).ToString();

            //var result = AuthorizationClient.GetUserToken(5044, redirectUri, clientSecret, code);

            var authClient = new AuthorizationClient(new ClientOptions()
            {
                Socks5ProxyOptions = new Socks5ProxyOptions()
                {
                    HostName = "localhost",
                    UseSocks5Proxy = true,
                    Port = 10801
                }
            });
            var publicAuth = authClient.GetPublicToken(5044, "SwbQi6CeSs13gE01302Qpp8BrqEADVj5DQadtdbD");
            var g = new OsuClientV2(publicAuth);
            try
            {
                //var b = g.User.GetOwnData();
                var a = g.User.GetUserBeatmap("1243669", UserBeatmapType.Favourite);
                var c = g.User.GetUser("gust");
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("401"))
                {

                }
                else if (e.Message.Contains("404"))
                {

                }
            }
        }
    }
}
