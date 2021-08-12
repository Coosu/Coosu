namespace Coosu.Storyboard.Common
{
    public interface IDefinedObject
    {
        ObjectType ObjectType { get; }
        int? RowInSource { get; internal set; }
    }
}