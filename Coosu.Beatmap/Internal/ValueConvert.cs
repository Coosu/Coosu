using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coosu.Beatmap.Internal
{
    internal static class ValueConvert
    {
        public static bool ConvertValue(string value, Type propType, out object converted)
        {
            object arg;
            if (propType == typeof(bool) && int.TryParse(value, out var parsed))
                arg = parsed;
            else
                arg = value;

            var type = typeof(System.Convert);
            var methodName = $"To{propType.Name}";
            var method = type.GetMethods().Where(t => t.Name == methodName).Where(t => t.GetParameters().Length == 1)
                .FirstOrDefault(t => t.GetParameters().First().ParameterType == typeof(object));

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
