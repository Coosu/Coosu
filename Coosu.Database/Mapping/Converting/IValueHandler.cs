using System;
using System.IO;

namespace Coosu.Database.Mapping.Converting;

public interface IValueHandler
{
    object ReadValue(BinaryReader binaryReader, DataType targetType);
    void Reset();
}