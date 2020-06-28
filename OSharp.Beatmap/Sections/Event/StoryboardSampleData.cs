﻿using OSharp.Beatmap.Configurable;
using System.IO;

namespace OSharp.Beatmap.Sections.Event
{
    public class StoryboardSampleData : SerializeWritableObject
    {
        public int Offset { get; set; }
        public int MagicalInt { get; set; }
        public string Filename { get; set; }
        public int Volume { get; set; }

        public override string ToString() => $"Sample,{Offset},{MagicalInt},{Filename},{Volume}";

        public override void AppendSerializedString(TextWriter textWriter)
        {
            textWriter.Write($"Sample,");
            textWriter.Write($"{Offset},");
            textWriter.Write($"{MagicalInt},");
            textWriter.Write($"{Filename},");
            textWriter.Write(Volume);
        }
    }
}
