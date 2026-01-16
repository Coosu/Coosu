using System.IO;

namespace Coosu.Beatmap.Configurable;

public interface ISerializeWritable
{
    string ToSerializedString(int version);
    void AppendSerializedString(TextWriter textWriter, int version);
}