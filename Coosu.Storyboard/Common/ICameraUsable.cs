namespace Coosu.Storyboard.Common
{
    public interface ICameraUsable
    {
        public double DefaultY { get; set; }
        public double DefaultX { get; set; }
        public double ZDistance { get; set; }
        public string CameraIdentifier { get; set; }
    }
}