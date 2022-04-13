using System;
using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Mapping.Converting;

public class IntDoublePairHandler : ValueHandler<IntDoublePair>
{
    public override IntDoublePair ReadValue(BinaryReader binaryReader, Type targetType)
    {
        return binaryReader.ReadIntDoublePairA();
    }

    public override void Reset() { }
}