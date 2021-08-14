using System.IO;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Common
{
    public interface IScriptable
    {
        Task WriteHeaderAsync(TextWriter writer);
        Task WriteScriptAsync(TextWriter writer);
    }
}