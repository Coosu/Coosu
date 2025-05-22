using System.Collections.Generic;
using Coosu.Database.DataTypes;

namespace Coosu.Database.Converting;

public sealed class IntSinglePair2ModKeyValueConverter : ValueConverter<IntSinglePair, KeyValuePair<Mods, float>>
{
    public override KeyValuePair<Mods, float> Convert(IntSinglePair obj)
    {
        return new KeyValuePair<Mods, float>((Mods)obj.IntValue, obj.SingleValue);
    }

    public override void Reset() { }
}