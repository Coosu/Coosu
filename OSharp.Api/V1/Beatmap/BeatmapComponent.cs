namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Beatmap ID.
    /// </summary>
    public sealed class BeatmapId : BeatmapComponent
    {
        /// <summary>
        /// Initialize a beatmap ID.
        /// </summary>
        /// <param name="mapId">Beatmap ID.</param>
        public BeatmapId(long mapId) : base(mapId, Type.Beatmap)
        {
        }

        /// <summary>
        /// Initialize a beatmap ID.
        /// </summary>
        /// <param name="mapId">Beatmap ID.</param>
        public BeatmapId(string mapId) : base(long.Parse(mapId), Type.Beatmap)
        {
        }
    }

    /// <summary>
    /// Beatmap-set ID.
    /// </summary>
    public sealed class BeatmapSetId : BeatmapComponent
    {
        /// <summary>
        /// Initialize a beatmap-set ID.
        /// </summary>
        /// <param name="setId">Beatmap-set ID.</param>
        public BeatmapSetId(long setId) : base(setId, Type.BeatmapSet)
        {
        }

        /// <summary>
        /// Initialize a beatmap-set ID.
        /// </summary>
        /// <param name="setId">Beatmap-set ID.</param>
        public BeatmapSetId(string setId) : base(long.Parse(setId), Type.BeatmapSet)
        {
        }
    }

    /// <summary>
    /// Presents a beatmap or a beatmap-set. It can be from beatmap ID and beatmap set ID.
    /// </summary>
    public abstract class BeatmapComponent
    {
        /// <summary>
        /// Initialize a beatmap or a beatmap set.
        /// </summary>
        /// <param name="id">Beatmap id or beatmap-set ID.</param>
        /// <param name="idType">Specify the ID which is beatmap ID or beatmap-set ID.</param>
        protected BeatmapComponent(long id, Type idType)
        {
            Id = id;
            IdType = idType;
        }

        /// <summary>
        /// Beatmap id or beatmap-set ID.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Get this ID type which is beatmap ID or beatmap-set ID.
        /// </summary>
        public Type IdType { get; }

        /// <summary>
        /// Initialize a beatmap ID.
        /// </summary>
        /// <param name="id">Beatmap ID.</param>
        /// <returns>Beatmap ID.</returns>
        public static BeatmapId FromMapId(long id) => new BeatmapId(id);

        /// <summary>
        /// Initialize a beatmap ID.
        /// </summary>
        /// <param name="id">Beatmap ID.</param>
        /// <returns>Beatmap ID.</returns>
        public static BeatmapId FromMapId(string id) => new BeatmapId(id);

        /// <summary>
        /// Initialize a beatmap-set ID.
        /// </summary>
        /// <param name="id">Beatmap-set ID.</param>
        /// <returns>Beatmap-set ID.</returns>
        public static BeatmapSetId FromSetId(long id) => new BeatmapSetId(id);

        /// <summary>
        /// Initialize a beatmap-set ID.
        /// </summary>
        /// <param name="id">Beatmap-set ID.</param>
        /// <returns>Beatmap-set ID.</returns>
        public static BeatmapSetId FromSetId(string id) => new BeatmapSetId(id);

        /// <summary>
        /// Beatmap type option.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Presents a beatmap.
            /// </summary>
            Beatmap,
            /// <summary>
            /// Presents a beatmap-set.
            /// </summary>
            BeatmapSet
        }

        /// <summary>
        /// Get the ID string value of the beatmap or beatmap-set.
        /// </summary>
        /// <returns>ID string.</returns>
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
