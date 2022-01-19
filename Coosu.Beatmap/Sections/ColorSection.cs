using Coosu.Beatmap.Configurable;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Colours")]
    public sealed class ColorSection : KeyValueSection
    {
        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo1 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo2 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo3 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo4 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo5 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo6 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo7 { get; set; }

        [SectionConverter(typeof(ColorConverter))]
        public Vector3<byte>? Combo8 { get; set; }

        protected override string KeyValueFlag => " : ";
    }
}
