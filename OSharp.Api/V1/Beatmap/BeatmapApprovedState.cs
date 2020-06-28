namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Beatmap approved state option.
    /// </summary>
    public enum BeatmapApprovedState
    {
        /// <summary>
        /// Graveyard beatmap.
        /// </summary>
        Graveyard = -2,
        /// <summary>
        /// Pending and WIP beatmap.
        /// </summary>
        Pending = -1,
        /// <summary>
        /// Ranked beatmap.
        /// </summary>
        Ranked = 1,
        /// <summary>
        /// Approved beatmap.
        /// </summary>
        Approved = 2,
        /// <summary>
        /// Qualified beatmap.
        /// </summary>
        Qualified = 3,
        /// <summary>
        /// Loved beatmap.
        /// </summary>
        Loved = 4
    }
}
