using System;
using System.Reflection;
using Coosu.Beatmap.Internal;

namespace Coosu.Beatmap.Configurable;

public class SectionInfo
{
    public SectionInfo(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
        Getter = DelegateHelper.CreateGetter(propertyInfo);
        Setter = DelegateHelper.CreateSetter(propertyInfo);
    }

    public PropertyInfo PropertyInfo { get; }
    public SectionPropertyAttribute? Attribute { get; set; }
    public bool UseSpecificFormat { get; set; }

    public Func<object, object?> Getter { get; }
    public Action<object, object?> Setter { get; }
}