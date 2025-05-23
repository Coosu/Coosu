﻿using System;
using System.IO;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

public sealed class ColorConverter : ValueConverter<ReadyOnlyVector3<byte>>
{
    public override ReadyOnlyVector3<byte> ReadSection(ReadOnlySpan<char> value)
    {
        byte x = default;
        byte y = default;
        byte z = default;

        var enumerator = value.SpanSplit(',');
        while (enumerator.MoveNext())
        {
            var span = enumerator.Current;
            switch (enumerator.CurrentIndex)
            {
                case 0: x = ParseHelper.ParseByte(span); break;
                case 1: y = ParseHelper.ParseByte(span); break;
                case 2: z = ParseHelper.ParseByte(span); break;
            }
        }

        return new ReadyOnlyVector3<byte>(x, y, z);
    }

    public override void WriteSection(TextWriter textWriter, ReadyOnlyVector3<byte> value)
    {
        textWriter.Write(value.X);
        textWriter.Write(",");
        textWriter.Write(value.Y);
        textWriter.Write(",");
        textWriter.Write(value.Z);
    }
}