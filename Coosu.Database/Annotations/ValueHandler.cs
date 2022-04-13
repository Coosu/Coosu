using System;
using System.IO;

namespace Coosu.Database.Annotations;

public abstract class ValueHandler<T> : IValueHandler
{
    public abstract T ReadValue(BinaryReader binaryReader, Type targetType);
    public abstract void Reset();

    object IValueHandler.ReadValue(BinaryReader binaryReader, Type targetType)
    {
        return ReadValue(binaryReader, targetType)!;
    }
}