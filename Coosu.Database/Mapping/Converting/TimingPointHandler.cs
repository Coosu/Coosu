using System.IO;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Mapping.Converting;

public class TimingPointHandler : ValueHandler<TimingPoint>
{
    public override TimingPoint ReadValue(BinaryReader binaryReader, DataType targetType)
    {
        return binaryReader.ReadTimingPointA();
    }

    public override void Reset() { }
}