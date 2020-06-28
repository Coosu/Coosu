namespace OSharp.Api.V1.MultiPlayer
{
    /// <summary>
    /// Winning condition of a match.
    /// </summary>
    public enum MatchScoringType
    {
        /// <summary>
        /// Higher score to win.
        /// </summary>
        Score = 0,
        /// <summary>
        /// Higher accuracy to win.
        /// </summary>
        Accuracy,
        /// <summary>
        /// Higher combo to win.
        /// </summary>
        Combo,
        /// <summary>
        /// Higher score(v2) to win.
        /// </summary>
        ScoreV2
    }
}
