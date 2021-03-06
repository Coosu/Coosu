﻿using Coosu.Beatmap.Configurable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Coosu.Beatmap
{
    public class ColorConverter : ValueConverter<Vector3<byte>>
    {
        public override Vector3<byte> ReadSection(string value)
        {
            var colors = value.Split(',').Select(byte.Parse).ToArray();
            return new Vector3<byte>(colors[0], colors[1], colors[2]);
        }

        public override string WriteSection(Vector3<byte> value)
        {
            return $"{value.X},{value.Y},{value.Z}";
        }
    }
}
