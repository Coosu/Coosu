using System;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Internal;

internal static class StaticTypes
{
    public static readonly Type Byte = typeof(byte);
    public static readonly Type Int16 = typeof(short);
    public static readonly Type Int32 = typeof(int);
    public static readonly Type Int64 = typeof(long);
    public static readonly Type Single = typeof(float);
    public static readonly Type Double = typeof(double);
    public static readonly Type Boolean = typeof(bool);
    public static readonly Type String = typeof(string);
    public static readonly Type DateTime = typeof(DateTime);
    public static readonly Type TimeSpan = typeof(TimeSpan);
    public static readonly Type IntDoublePair = typeof(IntDoublePair);
    public static readonly Type TimingPoint = typeof(TimingPoint);
}