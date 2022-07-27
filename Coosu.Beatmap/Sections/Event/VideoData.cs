using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;

namespace Coosu.Beatmap.Sections.Event;

public sealed class VideoData : SerializeWritableObject
{
    public double Offset { get; set; }
    public string Filename { get; set; } = "";

    public override string ToString() => $"Video,{Offset.ToEnUsFormatString()},\"{Filename}\"";

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write($"Video,");
        textWriter.Write($"{Offset.ToEnUsFormatString()},");
        textWriter.Write($"\"{Filename}\"");
        textWriter.WriteLine();
    }
}