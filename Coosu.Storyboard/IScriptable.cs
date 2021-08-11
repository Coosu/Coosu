using System.IO;
using System.Threading.Tasks;

namespace Coosu.Storyboard
{
    public interface IScriptable
    {
        Task WriteScriptAsync(TextWriter writer);
    }
}