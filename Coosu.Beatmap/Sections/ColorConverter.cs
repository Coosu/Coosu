using System;
using System.IO;
using System.Linq;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections
{
    public sealed class ColorConverter : ValueConverter<Vector3<byte>>
    {
        public override Vector3<byte> ReadSection(ReadOnlySpan<char> value)
        {
            byte x = default;
            byte y = default;
            byte z = default;

            int i = 0;
            foreach (var span in value.SpanSplit(','))
            {
                switch (i)
                {
#if NETCOREAPP3_1_OR_GREATER

                    case 0: x = byte.Parse(span); break;
                    case 1: y = byte.Parse(span); break;
                    case 2: z = byte.Parse(span); break;

#else
                    case 0: x = byte.Parse(span.ToString()); break;
                    case 1: y = byte.Parse(span.ToString()); break;
                    case 2: z = byte.Parse(span.ToString()); break;
#endif
                }

                i++;
            }

            return new Vector3<byte>(x, y, z);
        }

        public override void WriteSection(TextWriter textWriter, Vector3<byte> value)
        {
            textWriter.Write(value.X);
            textWriter.Write(",");
            textWriter.Write(value.Y);
            textWriter.Write(",");
            textWriter.Write(value.Z);
        }
    }
}
