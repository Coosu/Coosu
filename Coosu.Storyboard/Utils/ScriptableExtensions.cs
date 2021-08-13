using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Utils
{
    public static class ScriptableExtensions
    {
        public static async Task<string> ToScriptStringAsync(this IScriptable scriptable)
        {
            using var sw = new StringWriter();
            await scriptable.WriteScriptAsync(sw);
            return sw.ToString();
        }
        public static string ToScriptString(this IScriptable scriptable)
        {
            using var sw = new StringWriter();
            scriptable.WriteScriptAsync(sw).Wait();
            return sw.ToString();
        }
        public static async Task<string> GetHeaderStringAsync(this IScriptable scriptable)
        {
            using var sw = new StringWriter();
            await scriptable.WriteHeaderAsync(sw);
            return sw.ToString();
        }
        public static string GetHeaderString(this IScriptable scriptable)
        {
            using var sw = new StringWriter();
            scriptable.WriteHeaderAsync(sw).Wait();
            return sw.ToString();
        }
    }
}