using System;
using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections;
using Coosu.Beatmap.Sections.HitObject;

namespace Coosu.Beatmap.Internal;

internal static class StaticTypes
{
    public static readonly Type ObjectSamplesetType = typeof(ObjectSamplesetType);
    public static readonly Type Section = typeof(Section);
    public static readonly Type Config = typeof(Config);
    public static readonly Type ValueConverter = typeof(ValueConverter);
    public static readonly Type ColorConverter = typeof(ColorConverter);

    public static readonly Type Object = typeof(object);
    public static readonly Type Enum = typeof(Enum);
    public static readonly Type SystemConvert = typeof(Convert);

    public static readonly Type Boolean = typeof(bool);
    public static readonly Type Byte = typeof(byte);
    public static readonly Type Sbyte = typeof(sbyte);
    public static readonly Type Int16 = typeof(short);
    public static readonly Type UInt16 = typeof(ushort);
    public static readonly Type Int32 = typeof(int);
    public static readonly Type UInt32 = typeof(uint);
    public static readonly Type Int64 = typeof(long);
    public static readonly Type UInt64 = typeof(ulong);
    public static readonly Type Double = typeof(double);
    public static readonly Type Single = typeof(float);
    public static readonly Type String = typeof(string);
}