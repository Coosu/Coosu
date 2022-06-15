using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Converting;

public sealed class IntDoublePairHandler : ValueHandler<IntDoublePair>
{
    public override IntDoublePair ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return binaryReader.ReadIntDoublePairA();
    }

    public override void Reset() { }
}