using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Coosu.Beatmap.Configurable;
using Coosu.Shared;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

public sealed class DoubleSplitConverter : ValueConverter<List<double>>
{
    private readonly char _splitter;

    public DoubleSplitConverter(char splitter)
    {
        _splitter = splitter;
    }

    public override List<double> ReadSection(ReadOnlySpan<char> value)
    {
        var list = new List<double>();
        foreach (var subString in value.SpanSplit(_splitter))
        {
            list.Add(ParseHelper.ParseDouble(subString));
        }

        return list;
    }

    public override void WriteSection(TextWriter textWriter, List<double> value)
    {
        for (var i = 0; i < value.Count; i++)
        {
            var d = value[i];
            textWriter.Write(d.ToEnUsFormatString());
            if (i < value.Count - 1)
                textWriter.Write(_splitter);
        }
    }
}

public sealed class Int32SplitConverter : ValueConverter<List<int>>
{
    private readonly char _splitter;

    public Int32SplitConverter(char splitter)
    {
        _splitter = splitter;
    }

    public override List<int> ReadSection(ReadOnlySpan<char> value)
    {
        var list = new List<int>();
        foreach (var subString in value.SpanSplit(_splitter))
        {
            list.Add(ParseHelper.ParseInt32(subString));
        }

        return list;
    }

    public override void WriteSection(TextWriter textWriter, List<int> value)
    {
        for (var i = 0; i < value.Count; i++)
        {
            var d = value[i];
            textWriter.Write(d);
            if (i < value.Count - 1)
                textWriter.Write(_splitter);
        }
    }
}

public sealed class SplitConverter : ValueConverter<List<string>>
{
    private readonly char _splitter;

    public SplitConverter(char splitter)
    {
        _splitter = splitter;
    }

    public override List<string> ReadSection(ReadOnlySpan<char> value)
    {
        var list = new List<string>();
        foreach (var subString in value.SpanSplit(_splitter))
        {
            list.Add(subString.ToString());
        }

        return list;
    }

    public override void WriteSection(TextWriter textWriter, List<string> value)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < value.Count; i++)
        {
            var d = value[i];
            textWriter.Write(d);
            if (i < value.Count - 1)
                textWriter.Write(_splitter);
        }
    }
}