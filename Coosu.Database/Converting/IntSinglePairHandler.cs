using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Converting;

public sealed class IntSinglePairHandler : ValueHandler<IntSinglePair>
{
    public override IntSinglePair ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return binaryReader.ReadIntSinglePairA();
    }

    public override void Reset() { }
}