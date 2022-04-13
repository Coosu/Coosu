using System;
using System.IO;

namespace Coosu.Database.Annotations;

public interface IValueHandler
{
    object ReadValue(BinaryReader binaryReader, Type targetType);
    void Reset();
}