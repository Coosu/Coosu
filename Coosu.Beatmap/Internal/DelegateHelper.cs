using System;
using System.Reflection;

namespace Coosu.Beatmap.Internal;

internal static class DelegateHelper
{
    public static Func<object, object?> CreateGetter(PropertyInfo propertyInfo)
    {
        MethodInfo? getMethod = propertyInfo.GetGetMethod(true);
        if (getMethod == null)
        {
            // Should not happen if PropertyInfo is valid and has a getter
            return _ => throw new InvalidOperationException(
                $"Property '{propertyInfo.Name}' does not have a getter.");
        }

        Type targetType = propertyInfo.DeclaringType ??
                          throw new ArgumentNullException(nameof(propertyInfo.DeclaringType));
        Type valueType = propertyInfo.PropertyType;

        MethodInfo genericHelper = typeof(DelegateHelper)
            .GetMethod(nameof(CreateGetterHelper), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(targetType, valueType);

        return (Func<object, object?>)genericHelper.Invoke(null, new object[] { getMethod })!;
    }

    private static Func<object, object?> CreateGetterHelper<TTarget, TValue>(MethodInfo getMethod)
        where TTarget : class // Assuming target is a class for casting
    {
        var getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), getMethod);
        return instance => getter((TTarget)instance);
    }

    public static Action<object, object?> CreateSetter(PropertyInfo propertyInfo)
    {
        MethodInfo? setMethod = propertyInfo.GetSetMethod(true);
        if (setMethod == null)
        {
            // For properties that are read-only, or if setter should not be used.
            return (instance, value) => throw new InvalidOperationException(
                $"Property '{propertyInfo.Name}' does not have a public or non-public setter or it's not intended to be set this way.");
        }

        Type targetType = propertyInfo.DeclaringType ??
                          throw new ArgumentNullException(nameof(propertyInfo.DeclaringType));
        Type valueType = propertyInfo.PropertyType;

        MethodInfo genericHelper = typeof(DelegateHelper)
            .GetMethod(nameof(CreateSetterHelper), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(targetType, valueType);

        return (Action<object, object?>)genericHelper.Invoke(null, new object[] { setMethod })!;
    }

    private static Action<object, object?> CreateSetterHelper<TTarget, TValue>(MethodInfo setMethod)
    // No 'where TTarget : class' constraint here as it might be a struct, though less common for DeclaringType
    {
        var setter = (Action<TTarget, TValue?>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue?>), setMethod);
        return (instance, value) => setter((TTarget)instance, (TValue?)value);
    }
}