using System.Runtime.CompilerServices;
using Coosu.Shared.Numerics;

namespace Coosu.Shared;

public static class NumericExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToEnUsFormatString(this float value)
    {
        return value.ToString(ParseHelper.EnUsNumberFormat);
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToEnUsFormatString(this double value)
    {
        return value.ToString(ParseHelper.EnUsNumberFormat);
    }
}