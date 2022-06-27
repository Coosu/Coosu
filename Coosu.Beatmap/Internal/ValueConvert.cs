using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Internal
{
    internal static class ValueConvert
    {
        private static readonly Dictionary<string, MethodInfo?> MethodCache = new();
        private static readonly Type TypeBoolean = typeof(bool);
        private static readonly Type TypeByte = typeof(byte);
        private static readonly Type TypeSbyte = typeof(sbyte);
        private static readonly Type TypeInt16 = typeof(short);
        private static readonly Type TypeUInt16 = typeof(ushort);
        private static readonly Type TypeInt32 = typeof(int);
        private static readonly Type TypeUInt32 = typeof(uint);
        private static readonly Type TypeInt64 = typeof(long);
        private static readonly Type TypeUInt64 = typeof(ulong);
        private static readonly Type TypeDouble = typeof(double);
        private static readonly Type TypeSingle = typeof(float);
        private static readonly Type TypeString = typeof(string);

        public static bool ConvertValue(ReadOnlySpan<char> value, Type propType, out object? converted)
        {
            if (propType == TypeBoolean)
            {
                var b = ParseHelper.TryParseByte(value, out var result);
                converted = result == 1;
                return b;
            }

            if (propType == TypeString)
            {
                converted = value.ToString();
                return true;
            }

            if (propType == TypeInt32)
            {
                var b = ParseHelper.TryParseInt32(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeSingle)
            {
                var b = ParseHelper.TryParseSingle(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeDouble)
            {
                var b = ParseHelper.TryParseDouble(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeByte)
            {
                var b = ParseHelper.TryParseByte(value, out var result);
                converted = result;
                return b;
            }
            if (propType == TypeSbyte)
            {
                var b = ParseHelper.TryParseSByte(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeInt16)
            {
                var b = ParseHelper.TryParseInt16(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeUInt16)
            {
                var b = ParseHelper.TryParseUInt16(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeUInt32)
            {
                var b = ParseHelper.TryParseUInt32(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeInt64)
            {
                var b = ParseHelper.TryParseInt64(value, out var result);
                converted = result;
                return b;
            }

            if (propType == TypeUInt64)
            {
                var b = ParseHelper.TryParseUInt64(value, out var result);
                converted = result;
                return b;
            }

            object arg = value.ToString();
            var type = typeof(Convert);
            var methodName = "To" + propType.Name;
            if (!MethodCache.TryGetValue(methodName, out var method))
            {
                method = type.GetMethods()
                    .FirstOrDefault(t =>
                    {
                        if (t.Name != methodName) return false;
                        var parameters = t.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(object);
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
