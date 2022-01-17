using System.IO;

namespace Coosu.Beatmap.Configurable
{
    public interface ISerializeWritable
    {
        string ToSerializedString();
        void AppendSerializedString(TextWriter textWriter);
    }
}
