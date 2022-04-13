namespace Coosu.Database.Mapping.Converting;

public abstract class ValueConverter<TIn, TOut> : IValueConverter
{
    public abstract TOut Convert(TIn obj);
    public abstract void Reset();

    object IValueConverter.Convert(object obj)
    {
        return Convert((TIn)obj)!;
    }
}