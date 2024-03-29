﻿using System;
using System.Text;

namespace Coosu.Api.V2.RequestModels;

public record SearchOptions
{
    //q=string
    public string? Query { get; set; }

    //c=converts.follows.recommended
    public bool OnlyRecommended { get; set; }
    public bool IncludeConverted { get; set; }
    public bool OnlySubscribedMappers { get; set; }

    //s=ranked
    public BeatmapsetStatus BeatmapsetStatus { get; set; } = BeatmapsetStatus.Leaderboard;
    // e=storyboard.video
    public bool MustHasVideo { get; set; }
    public bool MustHasStoryboard { get; set; }
    //m=0
    public GameMode? Gamemode { get; set; }
    //g=1
    public GenreType Genre { get; set; }
    //l=1
    public LanguageType Language { get; set; }
    //nsfw=
    public bool Nsfw { get; set; }

    //r=XH.X.SH.S.A.B.C.D
    public bool ScoreHasXH { get; set; }
    public bool ScoreHasX { get; set; }
    public bool ScoreHasSH { get; set; }
    public bool ScoreHasS { get; set; }
    public bool ScoreHasA { get; set; }
    public bool ScoreHasB { get; set; }
    public bool ScoreHasC { get; set; }
    public bool ScoreHasD { get; set; }

    //played=played/unplayed
    public bool? IsPlayed { get; set; }

    //sort=xxx_desc
    public BeatmapsetSearchSort? Sort { get; set; }
    public bool IsSortAscending { get; set; }
    public string? CursorString { get; set; }

    public string GetQueryString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(Query))
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("q=" + (Query == null ? "" : HttpUtils.UrlEncode(Query)));
        }

        if (OnlyRecommended || IncludeConverted || OnlySubscribedMappers)
        {
            var subSb = new StringBuilder();
            if (IncludeConverted)
                subSb.Append("converts");
            if (OnlySubscribedMappers)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("follows");
            }

            if (OnlyRecommended)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("recommended");
            }

            if (sb.Length > 0) sb.Append('&');
            sb.Append("c=" + subSb);
        }

        if (BeatmapsetStatus != BeatmapsetStatus.Leaderboard)
        {
            var result = BeatmapsetStatus switch
            {
                BeatmapsetStatus.Any => "any",
                //BeatmapsetStatus.Leaderboard => "leaderboard",
                BeatmapsetStatus.Ranked => "ranked",
                BeatmapsetStatus.Qualified => "qualified",
                BeatmapsetStatus.Loved => "loved",
                BeatmapsetStatus.Favourites => "favourites",
                BeatmapsetStatus.Pending => "pending",
                BeatmapsetStatus.Wip => "wip",
                BeatmapsetStatus.Graveyard => "graveyard",
                BeatmapsetStatus.Mine => "mine",
                _ => ""
            };
            if (sb.Length > 0) sb.Append('&');
            sb.Append("s=" + result);
        }

        if (MustHasVideo || MustHasStoryboard)
        {
            var subSb = new StringBuilder();
            if (MustHasStoryboard)
                subSb.Append("storyboard");
            if (MustHasVideo)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("video");
            }

            if (sb.Length > 0) sb.Append('&');
            sb.Append("e=" + subSb);
        }

        if (Gamemode != null)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("m=" + (int)Gamemode);
        }

        if (Genre != GenreType.Any)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("g=" + (Genre == GenreType.Any ? "" : (int)Genre));
        }

        if (Language != LanguageType.Any)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("l=" + (Language == LanguageType.Any ? "" : (int)Language));
        }

        if (!Nsfw)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("nsfw=false");
        }

        if (ScoreHasXH || ScoreHasX || ScoreHasSH || ScoreHasS ||
            ScoreHasA || ScoreHasB || ScoreHasC || ScoreHasD)
        {
            var subSb = new StringBuilder();
            if (ScoreHasA)
                subSb.Append("A");
            if (ScoreHasB)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("B");
            }

            if (ScoreHasC)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("C");
            }

            if (ScoreHasD)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("D");
            }

            if (ScoreHasS)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("S");
            }

            if (ScoreHasSH)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("SH");
            }

            if (ScoreHasX)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("X");
            }

            if (ScoreHasXH)
            {
                if (subSb.Length > 0) subSb.Append('.');
                subSb.Append("XH");
            }

            if (sb.Length > 0) sb.Append('&');
            sb.Append("r=" + subSb);
        }

        if (IsPlayed != null)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("played=" + (IsPlayed == true ? "played" : "unplayed"));
        }

        if (Sort != null)
        {
            var result = Sort switch
            {
                BeatmapsetSearchSort.Artist => "artist",
                BeatmapsetSearchSort.Creator => "creator",
                BeatmapsetSearchSort.Difficulty => "difficulty",
                BeatmapsetSearchSort.Favourites => "favourites",
                BeatmapsetSearchSort.Nominations => "nominations",
                BeatmapsetSearchSort.Plays => "plays",
                BeatmapsetSearchSort.Ranked => "ranked",
                BeatmapsetSearchSort.Rating => "rating",
                BeatmapsetSearchSort.Relevance => "relevance",
                BeatmapsetSearchSort.Title => "title",
                BeatmapsetSearchSort.Updated => "updated",
                _ => throw new ArgumentOutOfRangeException()
            };

            if (sb.Length > 0) sb.Append('&');
            if (Sort is BeatmapsetSearchSort.Relevance && !IsSortAscending)
            {
                sb.Append("sort=");
            }
            else
            {
                sb.Append(("sort=" + result) + (IsSortAscending ? "_asc" : "_desc"));
            }
        }

        if (CursorString != null)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append("cursor_string=" + CursorString);
        }

        return sb.ToString();
    }
}