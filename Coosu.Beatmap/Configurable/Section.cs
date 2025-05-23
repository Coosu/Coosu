﻿using System;
using System.Reflection;

namespace Coosu.Beatmap.Configurable;

public abstract class Section : SerializeWritableObject, ISection
{
    [SectionIgnore]
    public string SectionName { get; }

    protected Section()
    {
        var type = GetType();
        var sb = type.GetCustomAttribute<SectionPropertyAttribute>();
        SectionName = sb?.Name ?? type.Name;
    }

    public abstract void Match(ReadOnlyMemory<char> memory);
}