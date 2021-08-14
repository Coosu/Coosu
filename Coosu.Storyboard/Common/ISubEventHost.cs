namespace Coosu.Storyboard.Common
{
    public interface ISubEventHost : IEventHost
    {
        ISceneObject? BaseObject { get; internal set; }
    }
}