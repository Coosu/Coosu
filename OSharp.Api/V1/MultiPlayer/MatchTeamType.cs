namespace OSharp.Api.V1.MultiPlayer
{
    /// <summary>
    /// Team type of a match.
    /// </summary>
    public enum MatchTeamType
    {
        /// <summary>
        /// Head to head.
        /// </summary>
        HeadToHead = 0,
        /// <summary>
        /// Tag coop.
        /// </summary>
        TagCoop,
        /// <summary>
        /// Team vs.
        /// </summary>
        TeamVs,
        /// <summary>
        /// Tag coop and team vs.
        /// </summary>
        TagTeamVs
    }
}