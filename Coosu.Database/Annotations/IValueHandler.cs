using System.IO;

namespace Coosu.Database.Annotations;

public interface IValueHandler
{
    object ReadValue(BinaryReader binaryReader, DataType targetType);
    void Reset();
}