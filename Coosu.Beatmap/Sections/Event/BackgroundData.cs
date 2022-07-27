using System.Globalization;
using System.IO;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections.Event;

public sealed class BackgroundData : SerializeWritableObject
{
    public string Filename { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public override string ToString() =>
        string.Format("{0},{1},\"{2}\",{3},{4}",
            0,
            0,
            Filename,
            X.ToString(CultureInfo.InvariantCulture),
            Y.ToString(CultureInfo.InvariantCulture));

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.Write($"0,");
        textWriter.Write($"0,");
        textWriter.Write($"\"{Filename}\",");
        textWriter.Write($"{X.ToString(CultureInfo.InvariantCulture)},");
        textWriter.Write(Y.ToString(CultureInfo.InvariantCulture));
        textWriter.WriteLine();
    }
}