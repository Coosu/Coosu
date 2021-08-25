namespace Coosu.Storyboard.Common
{
    public interface ICameraUsable
    {
        public double DefaultX { get; set; }
        public double DefaultY { get; set; }
        public double DefaultZ { get; set; }
        public string CameraIdentifier { get; set; }
    }
}