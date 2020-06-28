using OSharp.Beatmap.Configurable;
using System.Globalization;
using System.IO;

namespace OSharp.Beatmap.Sections.Event
{
    public class BackgroundData : SerializeWritableObject
    {
        public string Unknown1 { get; set; }
        public string Unknown2 { get; set; }
        public string Filename { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString() =>
            string.Format("{0},{1},\"{2}\",{3},{4}",
                Unknown1,
                Unknown2,
                Filename,
                X.ToString(CultureInfo.InvariantCulture),
                Y.ToString(CultureInfo.InvariantCulture));

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write($"{Unknown1},");
            textWriter.Write($"{Unknown2},");
            textWriter.Write($"\"{Filename}\",");
            textWriter.Write($"{X.ToString(CultureInfo.InvariantCulture)},");
            textWriter.Write(Y.ToString(CultureInfo.InvariantCulture));
        }
    }
}