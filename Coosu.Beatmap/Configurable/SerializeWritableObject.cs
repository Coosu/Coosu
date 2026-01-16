using System.IO;

namespace Coosu.Beatmap.Configurable;

public abstract class SerializeWritableObject : ISerializeWritable
{
    public string ToSerializedString(int version)
    {
        using var sb = new StringWriter();
        AppendSerializedString(sb, version);
        return sb.ToString();
    }

    public abstract void AppendSerializedString(TextWriter textWriter, int version);
}