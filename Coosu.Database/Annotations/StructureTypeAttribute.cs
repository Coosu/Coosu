using System;

namespace Coosu.Database.Annotations;

public sealed class StructureTypeAttribute : Attribute
{
    public DataType DataType { get; }

    public StructureTypeAttribute(DataType dataType)
    {
        DataType = dataType;
    }
}