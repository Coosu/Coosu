using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Sections.Event
{
    public class VideoData : SerializeWritableObject
    {
        public double Offset { get; set; }
        public string Filename { get; set; }

        public override string ToString() => $"Video,{Offset.ToInvariantString()},\"{Filename}\"";

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write($"Video,");
            textWriter.Write($"{Offset.ToInvariantString()},");
            textWriter.Write($"\"{Filename}\"");
        }
    }
}