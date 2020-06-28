using OSharp.Api.V1.Beatmap;

namespace OSharp.Api.V1.Score
{
    /// <summary>
    /// Score extension methods.
    /// </summary>
    public static class ScoreExtension
    {
        /// <summary>
        /// Get the specified score's accuracy.
        /// </summary>
        /// <param name="score">Play score.</param>
        /// <param name="beatmap">The same play's beatmap.</param>
        /// <returns></returns>
        public static double GetAccuracy(this IScore score, OsuBeatmap beatmap)
        {
            float accuracy = 0;
            var mapMode = beatmap.GameMode;
            float totalPointsOfHits;
            float totalNumberOfHits;

            // Logical switch for every mode
            if (mapMode == GameMode.Standard)
            {
                totalPointsOfHits = score.Count50 * 50 + score.Count100 * 100 + score.Count300 * 300;
                totalNumberOfHits = score.CountMiss + score.Count50 + score.Count100 + score.Count300;
                accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
            }
            else if (mapMode == GameMode.Taiko)
            {
                totalPointsOfHits = (score.Count100 * 0.5f + score.Count300 * 1) * 300;
                totalNumberOfHits = score.CountMiss + score.Count100 + score.Count300;
                accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
            }
            else if (mapMode == GameMode.CtB)
            {
                totalPointsOfHits = score.Count50 + score.Count100 + score.Count300;
                totalNumberOfHits = score.CountMiss + score.Count50 + score.Count100 + score.Count300 + score.CountKatu;
                accuracy = totalPointsOfHits / totalNumberOfHits;
            }
            else if (mapMode == GameMode.OsuMania)
            {
                totalPointsOfHits = score.Count50 * 50 + score.Count100 * 100 + score.CountKatu * 200 +
                                    (score.Count300 + score.CountGeki) * 300;
                totalNumberOfHits = score.CountMiss + score.Count50 + score.Count100 + score.CountKatu +
                                    score.Count300 + score.CountGeki;
                accuracy = totalPointsOfHits / (totalNumberOfHits * 300);
            }

            return accuracy;
        }
    }
}
