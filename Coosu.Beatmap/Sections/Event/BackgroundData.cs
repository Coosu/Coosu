using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections.Event;

public sealed class BackgroundData : SerializeWritableObject
{
    public string Filename { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }

    public override string ToString() =>
        string.Format("{0},{1},\"{2}\",{3},{4}",
            0,
            0,
            Filename,
            X.ToString(ParseHelper.EnUsNumberFormat),
            Y.ToString(ParseHelper.EnUsNumberFormat));

    public override void AppendSerializedString(TextWriter textWriter)
    {
        textWriter.WriteLine("0,0,\"" + Filename + "\"," +
                             X.ToString(ParseHelper.EnUsNumberFormat) + "," +
                             Y.ToString(ParseHelper.EnUsNumberFormat));
    }
}