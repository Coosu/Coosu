using System.Reflection;

namespace Coosu.Beatmap.Configurable;

public class SectionInfo
{

    public SectionInfo(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }

    public PropertyInfo PropertyInfo { get; }
    public SectionPropertyAttribute? Attribute { get; set; }
    public bool UseSpecificFormat { get; set; }
}