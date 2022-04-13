using System;

namespace Coosu.Database.Mapping;

public class OsuDbTypeAttribute : Attribute
{
    public DataType DataType { get; }

    public OsuDbTypeAttribute(DataType dataType)
    {
        DataType = dataType;
    }
}