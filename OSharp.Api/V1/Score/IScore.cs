namespace OSharp.Api.V1.Score
{
    /// <summary>
    /// Contains score data, which will support extensional calculate method.
    /// </summary>
    public interface IScore
    {
        /// <summary>
        /// Numerical score.
        /// </summary>
        long Score { get; }

        /// <summary>
        /// Count of Hit-300.
        /// </summary>
        int Count300 { get; }

        /// <summary>
        /// Count of Hit-100.
        /// </summary>
        int Count100 { get; }

        /// <summary>
        /// Count of Hit-50.
        /// </summary>
        int Count50 { get; } 
        
        /// <summary>
        /// Count of Miss.
        /// </summary>
        int CountMiss { get; } 
        
        /// <summary>
        /// Max combo of the score.
        /// </summary>
        int MaxCombo { get; }

        /// <summary>
        /// Count of Hit-Katu.
        /// </summary>
        int CountKatu { get; }

        /// <summary>
        /// Count of Hit-Geki.
        /// </summary>
        int CountGeki { get; }

        /// <summary>
        /// Represents whether a score is perfect.
        /// </summary>
        bool IsPerfect { get; }

        /// <summary>
        /// User who played the score.
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// Rank of the score.
        /// </summary>
        string Rank { get; }
    }
}
