using System.Collections.Generic;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Converting;

public sealed class IntDoublePair2ModKeyValueConverter : ValueConverter<IntDoublePair, KeyValuePair<Mods, double>>
{
    public override KeyValuePair<Mods, double> Convert(IntDoublePair obj)
    {
        return new KeyValuePair<Mods, double>((Mods)obj.IntValue, obj.DoubleValue);
    }

    public override void Reset() { }
}