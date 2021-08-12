using System.IO;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Common
{
    public interface IScriptable
    {
        Task WriteScriptAsync(TextWriter writer);
    }
}