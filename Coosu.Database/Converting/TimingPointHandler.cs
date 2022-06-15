using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Converting;

public sealed class TimingPointHandler : ValueHandler<TimingPoint>
{
    public override TimingPoint ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return binaryReader.ReadTimingPointA();
    }

    public override void Reset() { }
}