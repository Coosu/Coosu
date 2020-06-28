using System.IO;

namespace OSharp.Beatmap.Configurable
{
    public interface ISerializeWritable
    {
        string ToSerializedString();
        void AppendSerializedString(TextWriter textWriter);
    }
}
