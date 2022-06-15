namespace Coosu.Storyboard.Common
{
    public interface ICameraUsable
    {
        /// <summary>
        /// Camera initial X.
        /// The default value is determined by the group's center x.
        /// </summary>
        public float DefaultX { get; set; }

        /// <summary>
        /// Camera initial Y.
        /// The default value is determined by the group's center y.
        /// </summary>
        public float DefaultY { get; set; }

        /// <summary>
        /// Camera initial Z.
        /// The default value is 1.
        /// </summary>
        public float DefaultZ { get; set; }

        /// <summary>
        /// Camera ID.
        /// </summary>
        public string CameraIdentifier { get; set; }
    }
}