using System;

namespace Coosu.Api.V2
{
    public static class EnumExtension
    {
        public static string ToParamString(this GameMode gameMode)
        {
            switch (gameMode)
            {
                case GameMode.Osu:
                    return "osu";
                case GameMode.Taiko:
                    return "taiko";
                case GameMode.Fruits:
                    return "fruits";
                case GameMode.Mania:
                    return "mania";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }
        public static string ToParamString(this UserBeatmapType beatmapType)
        {
            switch (beatmapType)
            {
                case UserBeatmapType.Favourite:
                    return "favourite";
                case UserBeatmapType.Graveyard:
                    return "graveyard";
                case UserBeatmapType.Loved:
                    return "loved";
                case UserBeatmapType.MostPlayed:
                    return "most_played";
                case UserBeatmapType.RankedAndApproved:
                    return "ranked_and_approved";
                case UserBeatmapType.Unranked:
                    return "unranked";
                default:
                    throw new ArgumentOutOfRangeException(nameof(beatmapType), beatmapType, null);
            }
        }
    }
}