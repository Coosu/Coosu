using System;
using System.Linq;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap
{
    public class ColorConverter : ValueConverter<Vector3<byte>>
    {
        public override Vector3<byte> ReadSection(ReadOnlySpan<char> value)
        {
#if NETCOREAPP3_1_OR_GREATER
            int i = 0;
            byte x = 0;
            byte y = 0;
            byte z = 0;
            foreach (var span in value.SpanSplit(','))
            {
                if (i == 0) x = byte.Parse(span);
                else if (i == 1) y = byte.Parse(span);
                else if (i == 2) z = byte.Parse(span);
                i++;
            }

            return new Vector3<byte>(x, y, z);
#else
            var colors = value.ToString()
                .Split(',')
                .Select(byte.Parse)
                .ToArray();
            return new Vector3<byte>(colors[0], colors[1], colors[2]);
#endif

        }

        public override string WriteSection(Vector3<byte> value)
        {
            return $"{value.X},{value.Y},{value.Z}";
        }
    }
}
