using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Internal;

internal static class ValueConvert
{
    private static readonly ConcurrentDictionary<string, MethodInfo?> s_methodCache = new();

    public static object? ConvertValue(ReadOnlySpan<char> value, Type propType, bool useSpecificFormat)
    {
        if (propType == StaticTypes.Boolean)
        {
            var result = ParseHelper.ParseByte(value);
            return result == 1;
        }

        if (propType == StaticTypes.String)
        {
            return value.ToString();
        }

        if (propType == StaticTypes.Int32)
        {
            var result = ParseHelper.ParseInt32(value);
            return result;
        }

        if (propType == StaticTypes.Single)
        {
            var result = ParseHelper.ParseSingle(value,
                useSpecificFormat ? ParseHelper.EnUsNumberFormat : null);
            return result;
        }

        if (propType == StaticTypes.Double)
        {
            var result = ParseHelper.ParseDouble(value,
                useSpecificFormat ? ParseHelper.EnUsNumberFormat : null);
            return result;
        }

        if (propType == StaticTypes.Byte)
        {
            var result = ParseHelper.ParseByte(value);
            return result;
        }

        if (propType == StaticTypes.Sbyte)
        {
            var result = ParseHelper.ParseSByte(value);
            return result;
        }

        if (propType == StaticTypes.Int16)
        {
            var result = ParseHelper.ParseInt16(value);
            return result;
        }

        if (propType == StaticTypes.UInt16)
        {
            var result = ParseHelper.ParseUInt16(value);
            return result;
        }

        if (propType == StaticTypes.UInt32)
        {
            var result = ParseHelper.ParseUInt32(value);
            return result;
        }

        if (propType == StaticTypes.Int64)
        {
            var result = ParseHelper.ParseInt64(value);
            return result;
        }

        if (propType == StaticTypes.UInt64)
        {
            var result = ParseHelper.ParseUInt64(value);
            return result;
        }

        object arg = value.ToString();
        var methodName = "To" + propType.Name;
        var method = s_methodCache.GetOrAdd(methodName, static key =>
            StaticTypes.SystemConvert.GetMethods()
                .FirstOrDefault(t =>
                {
                    if (t.Name != key) return false;
                    var parameters = t.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == StaticTypes.Object;
                }));

        if (method == null)
        {
            throw new MissingMethodException($"Target method not found in System.Convert: {methodName}");
        }

        object[] p = { arg };
        return method.Invoke(null, p);
    }
}