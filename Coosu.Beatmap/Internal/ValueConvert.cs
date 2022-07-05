using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Internal
{
    internal static class ValueConvert
    {
        public static readonly NumberFormatInfo NumberFormatInfo =
            new CultureInfo(@"en-US", false).NumberFormat;

        private static readonly Dictionary<string, MethodInfo?> MethodCache = new();

        public static bool ConvertValue(ReadOnlySpan<char> value, Type propType, out object? converted,
            bool useSpecificFormat)
        {
            if (propType == StaticTypes.Boolean)
            {
                var b = ParseHelper.TryParseByte(value, out var result);
                converted = result == 1;
                return b;
            }

            if (propType == StaticTypes.String)
            {
                converted = value.ToString();
                return true;
            }

            if (propType == StaticTypes.Int32)
            {
                var b = ParseHelper.TryParseInt32(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.Single)
            {
                var b = ParseHelper.TryParseSingle(value, out var result,
                    !useSpecificFormat ? NumberFormatInfo.CurrentInfo : null);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.Double)
            {
                var b = ParseHelper.TryParseDouble(value, out var result,
                    !useSpecificFormat ? NumberFormatInfo.CurrentInfo : null);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.Byte)
            {
                var b = ParseHelper.TryParseByte(value, out var result);
                converted = result;
                return b;
            }
            if (propType == StaticTypes.Sbyte)
            {
                var b = ParseHelper.TryParseSByte(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.Int16)
            {
                var b = ParseHelper.TryParseInt16(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.UInt16)
            {
                var b = ParseHelper.TryParseUInt16(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.UInt32)
            {
                var b = ParseHelper.TryParseUInt32(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.Int64)
            {
                var b = ParseHelper.TryParseInt64(value, out var result);
                converted = result;
                return b;
            }

            if (propType == StaticTypes.UInt64)
            {
                var b = ParseHelper.TryParseUInt64(value, out var result);
                converted = result;
                return b;
            }

            object arg = value.ToString();
            var methodName = "To" + propType.Name;
            if (!MethodCache.TryGetValue(methodName, out var method))
            {
                method = StaticTypes.SystemConvert.GetMethods()
                    .FirstOrDefault(t =>
                    {
                        if (t.Name != methodName) return false;
                        var parameters = t.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == StaticTypes.Object;
                    });
                MethodCache.Add(methodName, method);
            }

            if (method == default)
            {
                converted = null;
                return false;
            }

            object[] p = { arg };
            converted = method.Invoke(null, p);
            return true;
        }
    }
}
