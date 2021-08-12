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
    }
}