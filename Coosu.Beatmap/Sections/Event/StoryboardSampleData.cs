using System.IO;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections.Event;

public sealed class StoryboardSampleData : SerializeWritableObject
{
    public int Offset { get; set; }
    public byte MagicalInt { get; set; }
    public string Filename { get; set; } = "";
    public byte Volume { get; set; }

    public override string ToString() => $"Sample,{Offset},{MagicalInt},\"{Filename}\",{Volume}";

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write("Sample," + Offset + "," + MagicalInt + ",\"" + Filename + "\"," + Volume);
    }
}