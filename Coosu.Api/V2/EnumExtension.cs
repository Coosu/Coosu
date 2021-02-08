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
    }
}