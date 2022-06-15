namespace Coosu.Database.Annotations;

public interface IValueConverter
{
    object Convert(object obj);
    void Reset();
}