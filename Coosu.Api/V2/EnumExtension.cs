using System;

namespace Coosu.Api.V2;

public static class EnumExtension
{
    public static string ToParamString(this GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.Osu => "osu",
            GameMode.Taiko => "taiko",
            GameMode.Fruits => "fruits",
            GameMode.Mania => "mania",
            _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };
    }

    public static string ToParamString(this ScoreType gameMode)
    {
        return gameMode switch
        {
            ScoreType.Best => "best",
            ScoreType.Firsts => "firsts",
            ScoreType.Recent => "recent",
            _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null)
        };
    }

    public static string ToParamString(this UserBeatmapType beatmapType)
    {
        return beatmapType switch
        {
            UserBeatmapType.Favourite => "favourite",
            UserBeatmapType.Graveyard => "graveyard",
            UserBeatmapType.Loved => "loved",
            UserBeatmapType.MostPlayed => "most_played",
            UserBeatmapType.RankedAndApproved => "ranked_and_approved",
            UserBeatmapType.Unranked => "unranked",
            _ => throw new ArgumentOutOfRangeException(nameof(beatmapType), beatmapType, null)
        };
    }
}