namespace Coosu.Storyboard
{
    public interface ISubEventHost : IEventHost
    {
        ISceneObject? BaseObject { get; internal set; }
    }
}