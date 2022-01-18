using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coosu.Beatmap.Internal
{
    internal static class ValueConvert
    {
        private static readonly Dictionary<string, MethodInfo> MethodCache = new();

        public static bool ConvertValue(string value, Type propType, out object? converted)
        {
            object arg;
            if (propType == typeof(bool) && int.TryParse(value, out var parsed))
                arg = parsed;
            else
                arg = value;

            var type = typeof(Convert);
            var methodName = $"To{propType.Name}";
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
