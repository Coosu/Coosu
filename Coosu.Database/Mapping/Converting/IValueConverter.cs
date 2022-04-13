namespace Coosu.Database.Mapping.Converting;

public interface IValueConverter
{
    object Convert(object obj);
    void Reset();
}