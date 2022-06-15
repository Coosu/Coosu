using System;
using System.IO;
using Coosu.Database.Annotations;
using Coosu.Database.Internal;
using Coosu.Database.Utils;

namespace Coosu.Database.Converting;

internal sealed class DefaultValueHandler : IValueHandler
{
    private DefaultValueHandler()
    {
    }

    public static DefaultValueHandler Instance { get; } = new();

    public object ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return targetType switch
        {
            DataType.Byte => binaryReader.ReadByte(),
            DataType.Int16 => binaryReader.ReadInt16(),
            DataType.Int32 => binaryReader.ReadInt32(),
            DataType.Int64 => binaryReader.ReadInt64(),
            DataType.ULEB128 => binaryReader.BaseStream.ReadLEB128Unsigned(),
            DataType.Single => binaryReader.ReadSingle(),
            DataType.Double => binaryReader.ReadDouble(),
            DataType.Boolean => binaryReader.ReadBoolean(),
            DataType.String => binaryReader.ReadStringA(),
            DataType.IntDoublePair => binaryReader.ReadIntDoublePairA(),
            DataType.TimingPoint => binaryReader.ReadTimingPointA(),
            DataType.DateTime => binaryReader.ReadDateTimeA(),
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
        };

    }

    public void Reset()
    {
    }
}