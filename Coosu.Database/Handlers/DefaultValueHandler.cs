using System;
using System.IO;
using Coosu.Database.Annotations;

namespace Coosu.Database.Handlers;

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