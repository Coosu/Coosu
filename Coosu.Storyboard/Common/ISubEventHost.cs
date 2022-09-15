namespace Coosu.Storyboard.Common;

public interface ISubEventHost : IDetailedEventHost
{
    ISceneObject? BaseObject { get; internal set; }
}