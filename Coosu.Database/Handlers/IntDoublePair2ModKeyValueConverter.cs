using System.Collections.Generic;
using Coosu.Database.Annotations;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Handlers;

public class IntDoublePair2ModKeyValueConverter : ValueConverter<IntDoublePair, KeyValuePair<Mods, double>>
{
    public override KeyValuePair<Mods, double> Convert(IntDoublePair obj)
    {
        return new KeyValuePair<Mods, double>((Mods)obj.IntValue, obj.DoubleValue);
    }

    public override void Reset() { }
}