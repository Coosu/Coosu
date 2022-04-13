using System;
using System.IO;

namespace Coosu.Database.Mapping.Converting;

internal class DefaultValueHandler : IValueHandler
{
    private DefaultValueHandler()
    {
    }

    public static DefaultValueHandler Instance { get; } = new();

    public object ReadValue(BinaryReader binaryReader, Type targetType)
    {
        return null;
    }

    public void Reset()
    {
    }
}