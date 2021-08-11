using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using Coosu.Beatmap;

namespace CoosuTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TestOsu();
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

        private static void TestOsu()
        {
            var o = OsuFile.ReadFromFileAsync(@"D:\ValveUnhandledExceptionFilter.txt").Result;

            var fileName = "op.avi";
            var fi = new FileInfo(@"F:\项目\GitHub\CoosuLocal\folder \" + fileName);
            if (fi.Exists)
            {
                Console.WriteLine(fi.FullName);
            }

            var directDirName = fi.DirectoryName;
            var dir = fi.Directory;
            var dirFullName = dir.FullName;
            var dirName = dir.Name;
            var toStringResult = dir.ToString();
            var pathWithCombine = Path.Combine(dirFullName, fi.Name);
            var pathWithCombine2 = Path.Combine(toStringResult, fi.Name);
            Console.WriteLine(pathWithCombine);
            Console.WriteLine(pathWithCombine2);

            var sb = OsuFile.ReadFromFileAsync(
                @"D:\Games\osu!\Songs\EastNewSound - Gensoukyou Matsuribayashi (Aki)\EastNewSound - Gensoukyou Matsuribayashi (Aki) (yf_bmp) [test].osu").Result;
            foreach (var rawHitObject in sb.HitObjects.HitObjectList)
            {
                var ticks = rawHitObject.SliderInfo.Ticks;
                var slids = rawHitObject.SliderInfo.BallTrail;
            }
        }
    }
}
