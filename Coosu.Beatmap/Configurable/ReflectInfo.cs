using System;
using System.Reflection;
using Coosu.Beatmap.Internal;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Configurable;

public sealed class ReflectInfo
{
#if NET6_0_OR_GREATER
    public ReflectInfo(PropertyInfo propertyInfo,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type type,
        string name)
#else
    public ReflectInfo(PropertyInfo propertyInfo, Type type, string name)
#endif
    {
        PropertyInfo = propertyInfo;
        Type = type;
        Name = name;
        Setter = DelegateHelper.CreateSetter(propertyInfo);
    }

    public PropertyInfo PropertyInfo { get; }

#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    public Type Type { get; }

    public string Name { get; }

    public Action<object, object?> Setter { get; }
}