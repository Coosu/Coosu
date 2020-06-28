namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Game mode with beatmap convert option.
    /// </summary>
    public class ConvertibleGameMode
    {
        /// <summary>
        /// Game mode.
        /// </summary>
        public GameMode GameMode { get; }
        /// <summary>
        /// Specify whether converted beatmaps are included.
        /// </summary>
        public BeatmapConvertOption ConvertOption { get; }

        /// <summary>
        /// Initialize a class which contains game mode and beatmap convert option.
        /// </summary>
        /// <param name="gameMode">Game mode.</param>
        /// <param name="convert">Specify whether converted beatmaps are included.</param>
        public ConvertibleGameMode(GameMode gameMode, bool convert = false)
        {
            GameMode = gameMode;
            ConvertOption = convert ? BeatmapConvertOption.Included : BeatmapConvertOption.NotIncluded;
        }

        /// <summary>
        /// Initialize game mode without convert option.
        /// </summary>
        /// <param name="gameMode">Game mode.</param>
        public static ConvertibleGameMode WithoutConvert(GameMode gameMode) => new ConvertibleGameMode(gameMode);

        /// <summary>
        /// Initialize game mode with convert option.
        /// </summary>
        /// <param name="gameMode">Game mode.</param>
        public static ConvertibleGameMode WithConvert(GameMode gameMode) => new ConvertibleGameMode(gameMode, true);
    }
}
