using System.IO;
using Coosu.Database.Annotations;

namespace Coosu.Database.Converting;

public abstract class ValueHandler<T> : IValueHandler
{
    public abstract T ReadValue(BinaryReader binaryReader, DataType targetType);
    public abstract void Reset();

    object IValueHandler.ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return ReadValue(binaryReader, targetType)!;
    }
}