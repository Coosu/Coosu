namespace Coosu.Storyboard.Common
{
    public interface ISceneObject : IDefinedObject, IDetailedEventHost
    {
        public double DefaultY { get; set; }
        public double DefaultX { get; set; }
        public double ZDistance { get; }
        public string CameraIdentifier { get; }
        //List<Loop> LoopList { get; }
        //List<Trigger> TriggerList { get; }
    }
}